module Djambi.Api.Logic.Services.GameStartService

open System.Linq
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.Logic.ModelExtensions
open Djambi.Api.Logic.ModelExtensions.BoardModelExtensions
open Djambi.Api.Model.BoardModel
open Djambi.Api.Model.GameModel
open Djambi.Api.Model.PlayerModel
open Djambi.Api.Model.SessionModel
open Djambi.Api.Logic.Services

let getStartingConditions(players : Player list) : PlayerStartConditions list =
    let colorIds = [0..(Constants.maxRegions-1)] |> Utilities.shuffle |> Seq.take players.Length
    let regions = [0..(players.Length-1)] |> Utilities.shuffle

    let startConds =
        players
        |> Seq.zip3 colorIds regions
        |> Seq.map (fun (c, r, p) ->
            {
                playerId = p.id
                region = r
                color = c
                turnNumber = None
            })

    let dict = Enumerable.ToDictionary (startConds, (fun startCond -> startCond.playerId))

    let nonVirtualPlayers =
        players
        |> List.filter (fun p -> p.playerType <> PlayerType.Virtual)
        |> Utilities.shuffle
        |> Seq.mapi (fun i p -> (i, p))

    for (i, p) in nonVirtualPlayers do
        dict.[p.id] <- { dict.[p.id] with turnNumber = Some i }

    dict.Values |> Seq.toList

let createPieces(board : BoardMetadata, startingConditions : PlayerStartConditions list) : Piece list =
    let createPlayerPieces(board : BoardMetadata, player : PlayerStartConditions, startingId : int) : Piece list =
        let getPiece(id : int, pieceType: PieceType, x : int, y : int) =
            {
                id = id
                pieceType = pieceType
                playerId = Some player.playerId
                originalPlayerId = player.playerId
                cellId = board.cellAt({ x = x; y = y; region = player.region}).id
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

    startingConditions
    |> List.mapi (fun i cond -> createPlayerPieces(board, cond, i*Constants.piecesPerPlayer))
    |> List.collect id

let startGame (lobbyId : int) (session : Session) : StartGameResponse AsyncHttpResult =
    LobbyRepository.getLobby lobbyId
    |> thenBind (fun lobby ->
        if session.isAdmin || session.userId = lobby.createdByUserId
        then Ok lobby
        else Error <| HttpException(403, "Cannot start game from lobby created by another user.")
    )
    |> thenBindAsync (fun lobby ->
        PlayerRepository.getPlayersForLobby lobbyId
        |> thenBind (fun players ->
            if players
                |> List.filter (fun p -> p.playerType <> PlayerType.Virtual)
                |> List.length = 1
            then Error <| HttpException(400, "Cannot start game with only one player.")
            else Ok players
        )
        |> thenBindAsync (PlayerService.fillEmptyPlayerSlots lobby)
        |> thenMap (fun players -> lobby.addPlayers players)
    )
    |> thenMap (fun lobby ->
        let startingConditions = getStartingConditions lobby.players
        let board = BoardModelUtility.getBoardMetadata lobby.regionCount
        let pieces = createPieces(board, startingConditions)
        let gameState : GameState =
            {
                players = lobby.players
                            |> List.map (fun p ->
                            {
                                id = p.id
                                isAlive = true
                            })
                pieces = pieces
                turnCycle = startingConditions
                            |> List.filter (fun cond -> cond.turnNumber.IsSome)
                            |> List.sortBy (fun cond -> cond.turnNumber.Value)
                            |> List.map (fun cond -> cond.playerId)
            }

        let gameWithoutSelectionOptions : Game =
            {
                regionCount = lobby.regionCount
                gameState = gameState
                turnState =
                    {
                        status = AwaitingSelection
                        selections = List.empty
                        selectionOptions = List.empty
                        requiredSelectionType = Some Subject
                    }
            }

        let (selectionOptions, _) = SelectionOptionsService.getSelectableCellsFromState gameWithoutSelectionOptions

        {
            lobbyId = lobby.id
            startingConditions = startingConditions
            gameState = gameState
            turnState = { gameWithoutSelectionOptions.turnState with selectionOptions = selectionOptions }
        }
    )
    |> thenBindAsync (fun startRequest ->
        GameRepository.startGame startRequest
        |> thenMap (fun gameId ->
            {
                gameId = gameId
                startingConditions = startRequest.startingConditions
                gameState = startRequest.gameState
                turnState = startRequest.turnState
            }
        )
    )