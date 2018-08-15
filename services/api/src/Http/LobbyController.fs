namespace Djambi.Api.Http

open Microsoft.AspNetCore.Http

open Giraffe

open Djambi.Api.JsonModels
open Djambi.Api.Persistence
open Djambi.Model.Games
open Djambi.Model.Users

type LobbyController(repository : LobbyRepository) =
    //Users
    member this.createUser =
        fun (next : HttpFunc)(ctx : HttpContext) ->
            task {
                let! requestDto = ctx.BindJsonAsync<CreateUserJsonModel>()
                let request : CreateUserRequest = 
                    {
                        name = requestDto.name
                    }

                let! response = repository.createUser(request)
                return! json response next ctx
            }

    member this.deleteUser(userId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let! response = repository.deleteUser(userId)
            return! json response next ctx
        }

    member this.getUser(userId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! response = repository.getUser(userId)
                return! json response next ctx
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

                let response : GameJsonModel list = 
                    [
                        {
                            id = 1
                            status = GameStatus.Open
                            boardRegionCount = 3
                            players = 
                            [
                                {
                                    id = 1
                                    name = "TestUser"
                                }
                            ]
                        }
                    ]

                let placeHolderResponse = {
                    text = "Get open games not yet implemented"
                }
                return! json placeHolderResponse next ctx
            }

    member this.createGame =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! request = ctx.BindModelAsync<CreateGameJsonModel>()

                let response : GameJsonModel = {
                    id = 1
                    status = GameStatus.Open
                    boardRegionCount = request.boardRegionCount
                    players = List.empty
                }

                let placeHolderResponse = {
                    text = "Create game not yet implemented"
                }
                return! json placeHolderResponse next ctx
            }

    member this.deleteGame(gameId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let placeHolderResponse = {
                    text = sprintf "Delete game %i not yet implemented" gameId
                }
                return! json placeHolderResponse next ctx
            }

    member this.addPlayerToGame(gameId : int, userId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let placeHolderResponse = {
                    text = sprintf "Add player %i to game %i not yet implemented" userId gameId
                }
                return! json placeHolderResponse next ctx
            }

    member this.removePlayerFromGame(gameId : int, userId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let placeHolderResponse = {
                    text = sprintf "Delete player %i from game %i not yet implemented" userId gameId
                }
                return! json placeHolderResponse next ctx
            }

    member this.startGame(gameId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let placeHolderResponse = {
                    text = sprintf "Start game %i not yet implemented" gameId
                }
                return! json placeHolderResponse next ctx
            }