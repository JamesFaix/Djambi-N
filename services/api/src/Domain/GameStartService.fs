namespace Djambi.Api.Domain

open System
open System.Linq
open System.Threading.Tasks
open Giraffe

open Djambi.Api.Common
open Djambi.Api.Domain.LobbyModels
open Djambi.Api.Persistence
open PlayModels
open Djambi.Api.Domain.BoardModels
open Djambi.Api.Domain.BoardsExtensions

type GameStartService(repository : GameStartRepository) =
                
    member this.addVirtualPlayers(game : LobbyGameMetadata) : LobbyPlayer list Task =
        task {
            let missingPlayerCount = game.boardRegionCount - game.players.Length
            if missingPlayerCount > 0
            then
                let! virtualPlayerNames = repository.getVirtualPlayerNames()
                let namesToUse = 
                    Enumerable.Except(
                        virtualPlayerNames, 
                        game.players |> Seq.map (fun p -> p.name), 
                        StringComparer.OrdinalIgnoreCase) 
                    |> Utilities.shuffle
                    |> Seq.take missingPlayerCount
                    |> Seq.toList

                for name in namesToUse do
                    let! _ = repository.addVirtualPlayerToGame(game.id, name)
                    ()
            else ()
            let! updated = repository.getGame(game.id)
            return updated.players
        }

    member this.getStartingConditions(players : LobbyPlayer list) : PlayerStartConditions list =
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
 
    member this.createPieces(board : BoardMetadata, startingConditions : PlayerStartConditions list) : Piece list =
        let createPlayerPieces(board : BoardMetadata, player : PlayerStartConditions, startingId : int) : Piece list =            
            let getPiece(id : int, pieceType: PieceType, x : int, y : int) =
                {
                    id = id
                    pieceType = pieceType
                    playerId = Some player.playerId
                    originalPlayerId = player.playerId
                    isAlive = true
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

    member this.startGame(gameId : int) : GameState Task =
        task {
            let! game = repository.getGame gameId
            let! lobbyPlayers = this.addVirtualPlayers game
            let startingConditions = this.getStartingConditions(lobbyPlayers)
            let board = BoardUtility.getBoardMetadata(game.boardRegionCount)
            let pieces = this.createPieces(board, startingConditions)
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
                    turnCycle = startingConditions |> List.map (fun cond -> cond.turnNumber)
                    log = List.empty
                }
                
            let updateRequest : UpdateGameForStartRequest = 
                {
                    id = gameId
                    startingConditions = startingConditions
                    currentState = gameState
                }

            let! _ = repository.startGame(updateRequest)

            //Validate

            return gameState
        }

