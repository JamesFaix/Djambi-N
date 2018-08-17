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
        
    member this.startGame(gameId : int) : GameState Task =
        
        let createVirtualPlayers(game : LobbyGameMetadata) : LobbyPlayer list Task =
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
                        let! _ = repository.addVirtualPlayerToGame(gameId, name)
                        ()
                else ()
                let! updated = repository.getGame(game.id)
                return updated.players
            }
            
        let getStartingConditions(players : LobbyPlayer list) : PlayerStartConditions list =
            let colorIds = [0..7] |> Utilities.shuffle |> Seq.take players.Length
            let regions = [0..(players.Length-1)] |> Utilities.shuffle
            let turnOrder = players |> Utilities.shuffle

            turnOrder 
            |> Seq.zip3 colorIds regions
            |> Seq.mapi (fun i (r, c, p) -> 
                {
                    playerId = p.id
                    turnNumber = i
                    region = r
                    color = c
                })
            |> Seq.toList
 
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
            [
                getPiece(startingId, Chief, 4,4)
                getPiece(startingId+1, Reporter, 4,3)
                getPiece(startingId+2, Assassin, 3,4)
                getPiece(startingId+3, Diplomat, 3,3)
                getPiece(startingId+4, Gravedigger, 2,2)
                getPiece(startingId+5, Thug, 2,3)
                getPiece(startingId+6, Thug, 2,4)
                getPiece(startingId+7, Thug, 3,2)
                getPiece(startingId+8, Thug, 4,2)
            ]

        let createAndPlacePieces(board : BoardMetadata, startingConditions : PlayerStartConditions list) : Piece list =
            startingConditions
            |> List.mapi (fun i cond -> createPlayerPieces(board, cond, i*9))
            |> List.collect id

        task {
            let! game = repository.getGame gameId
            let! lobbyPlayers = createVirtualPlayers game
            let startingConditions = getStartingConditions(lobbyPlayers)
            let board = BoardUtility.getBoardMetadata(game.boardRegionCount)
            let pieces = createAndPlacePieces(board, startingConditions)
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

