namespace Djambi.Api.Http

open Microsoft.AspNetCore.Http

open Giraffe

open Djambi.Api.Domain.LobbyModels
open Djambi.Api.Http.LobbyJsonModels
open Djambi.Api.Http.LobbyJsonMappings
open Djambi.Api.Persistence
open Djambi.Api.Common.Enums

open System
open System.Linq
open Djambi.Api.Common

type LobbyController(repository : LobbyRepository) =
    //Users
    member this.createUser =
        fun (next : HttpFunc)(ctx : HttpContext) ->
            task {
                let! requestJson = ctx.BindJsonAsync<CreateUserJsonModel>()
                let request = requestJson |> mapCreateUserRequest
                let! response = repository.createUser(request)
                let responseJson = response |> mapUserResponse
                return! json responseJson next ctx
            }

    member this.deleteUser(userId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let! _ = repository.deleteUser(userId)
            return! json () next ctx
        }

    member this.getUser(userId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! response = repository.getUser(userId)
                let responseJson = response |> mapUserResponse
                return! json responseJson next ctx
            }

    member this.updateUser(userId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let placeHolderResponse = {
                    text = sprintf "Update user %i not yet implemented" userId
                }
                return! json placeHolderResponse next ctx
            }

//Game lobby
    member this.getOpenGames =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! response = repository.getOpenGames()
                let responseJson = response |> List.map mapLobbyGameResponse
                return! json responseJson next ctx
            }

    member this.createGame =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! requestJson = ctx.BindModelAsync<CreateGameJsonModel>()
                let request = requestJson |> mapCreateGameRequest
                let! response = repository.createGame(request)
                let responseJson = response |> mapLobbyGameResponse
                return! json responseJson next ctx
            }

    member this.deleteGame(gameId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! _ = repository.deleteGame(gameId)
                return! json () next ctx
            }

    member this.addPlayerToGame(gameId : int, userId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! _ = repository.addPlayerToGame(gameId, userId)
                return! json () next ctx
            }

    member this.removePlayerFromGame(gameId : int, userId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! _ = repository.removePlayerFromGame(gameId, userId)
                return! json () next ctx
            }

    member this.startGame(gameId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! game = repository.getGame(gameId)

                //Update game status
                let updateRequest : UpdateGameRequest = 
                    {
                        id = gameId
                        description = game.description
                        status = GameStatus.Started
                    }                
                let! _ = repository.updateGame(updateRequest)

                //Create virtual players if required
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

                //Assign players colors, corners, turn order

                //Create pieces; place on board

                //Validate game state

                let placeHolderResponse = {
                    text = sprintf "Start game %i not yet implemented" gameId
                }
                return! json placeHolderResponse next ctx
            }