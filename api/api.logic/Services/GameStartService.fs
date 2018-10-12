namespace Djambi.Api.Logic.Services

open System
open System.Linq
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Common.Enums
open Djambi.Api.Db.Repositories
open Djambi.Api.Logic.ModelExtensions
open Djambi.Api.Logic.ModelExtensions.BoardModelExtensions
open Djambi.Api.Model.BoardModel
open Djambi.Api.Model.LobbyModel
open Djambi.Api.Model.PlayModel

module GameStartService =
                
    let addVirtualPlayers(game : LobbyGameMetadata) : LobbyPlayer list AsyncHttpResult =
        let missingPlayerCount = game.boardRegionCount - game.players.Length

        let getVirtualNamesToUse (possibleNames : string list) = 
            Enumerable.Except(
                possibleNames, 
                game.players |> Seq.map (fun p -> p.name), 
                StringComparer.OrdinalIgnoreCase) 
            |> Utilities.shuffle
            |> Seq.take missingPlayerCount

        if missingPlayerCount = 0
        then LobbyRepository.getGame(game.id)
             |> thenMap (fun g -> g.players)
        else
            LobbyRepository.getVirtualPlayerNames()
            |> thenMap getVirtualNamesToUse
            |> thenDoEachAsync (fun name -> LobbyRepository.addVirtualPlayerToGame(game.id, name))
            |> thenBindAsync (fun _ -> LobbyRepository.getGame(game.id))
            |> thenMap (fun g -> g.players)

    let getStartingConditions(players : LobbyPlayer list) : PlayerStartConditions list =
        let colorIds = [0..(Constants.maxRegions-1)] |> Utilities.shuffle |> Seq.take players.Length
        let regions = [0..(players.Length-1)] |> Utilities.shuffle
        let turnOrder = players |> Utilities.shuffle

        turnOrder 
        |> Seq.zip3 colorIds regions
        |> Seq.mapi (fun i (c, r, p) -> 
            {
                playerId = p.id
                turnNumber = i
                region = r
                color = c
            })
        |> Seq.toList
 
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

    let startGame(gameId : int) : GameStartResponse AsyncHttpResult =
        LobbyRepository.getGame gameId
        |> thenBindAsync (fun game -> 
            addVirtualPlayers game
            |> thenMap (fun lobbyPlayers -> (game, lobbyPlayers)))

        |> thenMap (fun (game, lobbyPlayers) -> 
            let startingConditions = getStartingConditions(lobbyPlayers)
            let board = BoardModelUtility.getBoardMetadata(game.boardRegionCount)
            let pieces = createPieces(board, startingConditions)
            let gameState : GameState = 
                {
                    players = lobbyPlayers 
                              |> List.map (fun p -> 
                                { 
                                    id = p.id
                                    userId = p.userId
                                    name = p.name
                                    isAlive = true
                                })
                    pieces = pieces
                    turnCycle = startingConditions 
                                |> List.sortBy (fun cond -> cond.turnNumber)
                                |> List.map (fun cond -> cond.playerId)
                }                

            let gameWithoutSelectionOptions : Game = 
                {
                    boardRegionCount = game.boardRegionCount
                    currentGameState = gameState
                    currentTurnState = 
                        {
                            status = AwaitingSelection
                            selections = List.empty
                            selectionOptions = List.empty
                            requiredSelectionType = Some Subject
                        }
                }

            let (selectionOptions, _) = PlayService.getSelectableCellsFromState gameWithoutSelectionOptions
            {
                id = gameId
                startingConditions = startingConditions
                currentGameState = gameState
                currentTurnState = { gameWithoutSelectionOptions.currentTurnState with selectionOptions = selectionOptions }
            })
        |> thenDoAsync (fun updateRequest -> PlayRepository.startGame(updateRequest))
        |> thenMap (fun updateRequest -> 
            {
                startingConditions = updateRequest.startingConditions
                gameState = updateRequest.currentGameState
                turnState = updateRequest.currentTurnState
            })