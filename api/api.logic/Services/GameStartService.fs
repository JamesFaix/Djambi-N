module Djambi.Api.Logic.Services.GameStartService

open System.Linq
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.Logic.ModelExtensions
open Djambi.Api.Logic.ModelExtensions.BoardModelExtensions
open Djambi.Api.Model
open Djambi.Api.Logic.Services

type ArrayList<'a> = System.Collections.Generic.List<'a>
 
//TODO: Add integration tests
let getGameStartEvent (game : Game) (session : Session) : Event AsyncHttpResult =    
    if session.isAdmin || session.userId = game.createdByUserId
    then okTask game
    else errorTask <| HttpException(403, "Cannot start game created by another user.")
    |> thenBindAsync (fun _ ->
        if game.players
            |> List.filter (fun p -> p.kind <> PlayerKind.Neutral)
            |> List.length = 1
        then errorTask <| HttpException(400, "Cannot start game with only one player.")
        else 
           PlayerService.fillEmptyPlayerSlots game
            |> thenMap (fun addNeutralPlayerEffects ->
                let effects =
                    //The order is very important for effect processing. Neutral players must be created before the game start.                    
                    List.append 
                        addNeutralPlayerEffects 
                        [EventEffect.gameStatusChanged(GameStatus.Pending, GameStatus.Started)]
                Event.create(EventKind.GameStarted, effects)
            )
    )

let assignStartingConditions(players : Player list) : Player list =
    let colorIds = [0..(Constants.maxRegions-1)] |> Utilities.shuffle |> Seq.take players.Length
    let regions = [0..(players.Length-1)] |> Utilities.shuffle

    let playersWithAssignments =
        players
        |> Seq.zip3 colorIds regions
        |> Seq.map (fun (c, r, p) -> 
            { 
                p with 
                    startingRegion = Some r
                    startingTurnNumber = None
                    colorId = Some c 
            }
        )

    let dict = Enumerable.ToDictionary (playersWithAssignments, (fun p -> p.id))

    let nonNeutralPlayers =
        players
        |> List.filter (fun p -> p.kind <> PlayerKind.Neutral)
        |> Utilities.shuffle
        |> Seq.mapi (fun i p -> (i, p))

    for (i, p) in nonNeutralPlayers do
        dict.[p.id] <- { dict.[p.id] with startingTurnNumber = Some i }

    dict.Values 
    |> Seq.map (fun p -> { p with isAlive = Some true })
    |> Seq.toList

let createPieces(board : BoardMetadata, players : Player list) : Piece list =
    let createPlayerPieces(board : BoardMetadata, player : Player, startingId : int) : Piece list =
        let getPiece(id : int, pieceType: PieceKind, x : int, y : int) =
            {
                id = id
                kind = pieceType
                playerId = Some player.id
                originalPlayerId = player.id
                cellId = board.cellAt({x = x; y = y; region = player.startingRegion.Value}).id
            }
        let n = Constants.regionSize - 1
        [
            getPiece(startingId, Chief, n,n)
            getPiece(startingId+1, Reporter, n,n-1)
            getPiece(startingId+2, Assassin, n-1,n)
            getPiece(startingId+3, Diplomat, n-1,n-1)
            getPiece(startingId+4, Gravedigger, n-2,n-2)
            getPiece(startingId+5, Thug, n-2,n-1)
            getPiece(startingId+6, Thug, n-2,n)
            getPiece(startingId+7, Thug, n-1,n-2)
            getPiece(startingId+8, Thug, n,n-2)
        ]

    players
    |> List.mapi (fun i cond -> createPlayerPieces(board, cond, i*Constants.piecesPerPlayer))
    |> List.collect id

let startGame (game : Game) : Game AsyncHttpResult =
    let board = BoardModelUtility.getBoardMetadata game.parameters.regionCount
    let players = assignStartingConditions game.players

    let game = 
        { 
            game with 
                status = GameStatus.Started
                pieces = createPieces(board, players) //Starting conditions must first be assigned
                players = players
                turnCycle = players //Starting conditions must first be assigned
                    |> List.filter (fun p -> p.startingTurnNumber.IsSome)
                    |> List.sortBy (fun p -> p.startingTurnNumber.Value)
                    |> List.map (fun p -> p.id)
                currentTurn = 
                    Some {
                        status = AwaitingSelection
                        selections = List.empty
                        selectionOptions = List.empty
                        requiredSelectionKind = Some Subject
                    }
        }

    let (selectionOptions, _) = SelectionOptionsService.getSelectableCellsFromState game

    let game = 
        { 
            game with 
                currentTurn = 
                    Some {
                        game.currentTurn.Value with selectionOptions = selectionOptions
                    }        
        }

    game.players |> Seq.ofList |> okTask
    |> thenDoEachAsync (fun p -> 
        let request : SetPlayerStartConditionsRequest = 
            {
                playerId = p.id
                colorId = p.colorId.Value
                startingRegion = p.startingRegion.Value
                startingTurnNumber = p.startingTurnNumber
            }
        GameRepository.setPlayerStartConditions request        
    )
    |> thenBindAsync (fun _ ->
        let request : UpdateGameStateRequest = 
            {
                gameId = game.id
                status = game.status
                pieces = game.pieces
                currentTurn = game.currentTurn
                turnCycle = game.turnCycle
            }
        GameRepository.updateGameState request
    )
    |> thenMap (fun _ -> game)
   