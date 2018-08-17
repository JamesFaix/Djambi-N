namespace Djambi.Api.Domain

open System
open System.Linq
open System.Threading.Tasks
open Giraffe

open Djambi.Api.Common
open Djambi.Api.Common.Enums
open Djambi.Api.Domain.LobbyModels
open Djambi.Api.Persistence
open PlayModels
open Djambi.Api.Domain.BoardModels
open Djambi.Api.Domain.BoardsExtensions

type GameStartService(repository : LobbyRepository) =
        
    member this.startGame(gameId : int) : GameState Task =
        
        let updateGameStatus(game : LobbyGameMetadata) : Unit Task =
            task {
                let updateRequest : UpdateGameRequest = 
                    {
                        id = gameId
                        description = game.description
                        status = GameStatus.Started
                    }                
                let! _ = repository.updateGame(updateRequest)
                return ()
            }

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
            
        let assignTurnOrderAndColors(gameId : int, lobbyPlayers : LobbyPlayer list) : Player list Task =
            task {
                let shuffledPlayers = lobbyPlayers |> Utilities.shuffle |> Seq.toList
                let colors = [A;B;C;D;E;F;G;H] |> Seq.take shuffledPlayers.Length

                let players : Player list = 
                    shuffledPlayers 
                    |> Seq.zip colors
                    |> Seq.map (fun (c, p) -> 
                        { 
                            id = p.id
                            userId = p.userId
                            name = p.name
                            isAlive = true
                            color = c
                        })
                    |> Seq.toList

                //for p in players do
                //    let! _ = repository.updatePlayer(gameId, p)
                //    ()

                return players
            }

        let assignRegions(players : Player list, regionCount : int) : (Player * int) list =
            let regions = [0..(regionCount-1)] |> Utilities.shuffle 
            regions |> Seq.zip players |> Seq.toList

        let createPlayerPieces(board : BoardMetadata, player : Player, region : int, startingId : int) : Piece list =            
            let getPiece(id : int, pieceType: PieceType, x : int, y : int) =
                {
                    id = id
                    pieceType = pieceType
                    playerId = Some player.id
                    originalPlayerId = player.id
                    isAlive = true
                    cellId = board.cellAt({ x = x; y = y; region = region}).id
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

        let createAndPlacePieces(board : BoardMetadata, playersWithRegions : (Player * int) list) : Piece list =
            playersWithRegions
            |> List.mapi (fun i (p, r) -> createPlayerPieces(board, p, r, i*9))
            |> List.collect id

        task {
            let! game = repository.getGame gameId
            let! _ = updateGameStatus game
            let! lobbyPlayers = createVirtualPlayers game
            let! turnOrder = assignTurnOrderAndColors(gameId, lobbyPlayers)
            let assignedRegions = assignRegions(turnOrder, game.boardRegionCount)
            let board = BoardUtility.getBoardMetadata(game.boardRegionCount)
            let pieces = createAndPlacePieces(board, assignedRegions)
            let gameState : GameState = 
                {
                    players = turnOrder
                    pieces = pieces
                    turnCycle = turnOrder |> List.map (fun p -> p.id)
                    log = List.empty
                }

            //Validate
            //Persist gamestate

            return gameState
        }

