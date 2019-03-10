module Djambi.Api.Logic.Services.GameStartService

open System.Linq
open Djambi.Api.Common
open Djambi.Api.Common.Collections
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Logic.ModelExtensions
open Djambi.Api.Logic.ModelExtensions.BoardModelExtensions
open Djambi.Api.Logic.Services
open Djambi.Api.Model
open Djambi.Api.Logic
 
let getGameStartEvent (game : Game) (session : Session) : CreateEventRequest AsyncHttpResult =    
    SecurityService.ensureCreatorOrEditPendingGames session game
    |> Result.bindAsync (fun _ ->    
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
                        [Effect.GameStatusChanged { oldValue = GameStatus.Pending; newValue = GameStatus.Started }]

                {
                    kind = EventKind.GameStarted
                    effects = effects
                    createdByUserId = session.user.id
                    actingPlayerId = ContextService.getActingPlayerId session game
                }
            )
    )

let assignStartingConditions(players : Player list) : Player list =
    let colorIds = [0..(Constants.maxRegions-1)] |> List.shuffle |> Seq.take players.Length
    let regions = [0..(players.Length-1)] |> List.shuffle

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

    (* 
        At this point, neutral players may still have ID = 0, because
        they have not yet been persisted as part of the StartGame transaction.
        Index on name instead of id.
    *)
    let dict = Enumerable.ToDictionary (playersWithAssignments, (fun p -> p.name))

    let nonNeutralPlayers =
        players
        |> List.filter (fun p -> p.kind <> PlayerKind.Neutral)
        |> List.shuffle
        |> Seq.mapi (fun i p -> (i, p))

    for (i, p) in nonNeutralPlayers do
        dict.[p.name] <- { dict.[p.name] with startingTurnNumber = Some i }

    dict.Values 
    |> Seq.map (fun p -> { p with status = PlayerStatus.Alive })
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
    
let applyStartGame (game : Game) : Game =
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
                currentTurn = Some Turn.empty
        }

    let options = (SelectionOptionsService.getSelectableCellsFromState game) |> Result.value
    let turn = { game.currentTurn.Value with selectionOptions = options }  
    { game with  currentTurn =  Some turn }