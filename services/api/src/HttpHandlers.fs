namespace djambi.api

module HttpHandlers =

    open Microsoft.AspNetCore.Http
    open Giraffe
    open Djambi.Api.Dtos
    open Djambi.Model
    open BoardGeometryExtensions
    open Djambi.Model.Users
    open Djambi.Api.Persistence

    type HttpHandler(repository : Repository) =
//Users
        member this.createUser =
            fun (next : HttpFunc)(ctx : HttpContext) ->
                task {
                    let! requestDto = ctx.BindJsonAsync<CreateUserRequestDto>()
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

                    let response : GameMetadataDto list = 
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
                    let! request = ctx.BindModelAsync<CreateGameRequestDto>()

                    let response : GameMetadataDto = {
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

    //Board
        member this.getBoard(regionCount : int) =
            fun (next : HttpFunc) (ctx : HttpContext) ->
                task {
                    let board = BoardRepository.getBoard(regionCount)
                    return! json board next ctx
                }
            
        member this.getCellPaths(regionCount : int, cellId : int) =
            fun (next : HttpFunc) (ctx : HttpContext) ->
                task {
                    let board = BoardRepository.getBoardMetadata(regionCount)
                    let cell = board.cells() |> Seq.find(fun c -> c.id = cellId)
                    let paths = board.paths(cell)
                                |> List.map (fun path -> 
                                    path |> List.map (fun c -> c.id))
                    return! json paths next ctx
                }

    //Gameplay
        member this.getGameState(gameId : int) =
            fun (next : HttpFunc) (ctx : HttpContext) ->
                task {
                    let response : GameDetailsDto = {
                        id = gameId
                        status = GameStatus.Open
                        boardRegionCount = 3
                        players = List.empty
                        pieces = List.empty
                        selectionOptions = List.empty
                    }

                    let placeHolderResponse = {
                        text = sprintf "Get game state %i not yet implemented" gameId
                    }
                    return! json placeHolderResponse next ctx
                }

        member this.makeSelection(gameId : int) =
            fun (next : HttpFunc) (ctx : HttpContext) ->
                task {
                    let! request = ctx.BindJsonAsync<CreateSelectionDto>()

                    //let response : GameDetailsDto = {
                    //    id = gameId
                    //    status = GameStatus.Open
                    //    boardRegionCount = 3
                    //    players = List.empty
                    //    pieces = List.empty
                    //    selectionOptions = landscape.paths(location) |> List.collect id
                    //}
                
                    let placeHolderResponse = {
                        text = sprintf "Make selection %i not yet implemented" gameId
                    }
                    return! json placeHolderResponse next ctx
                }

        member this.resetTurn(gameId : int) =
            fun (next : HttpFunc) (ctx : HttpContext) ->
                task {
                    let response : GameDetailsDto = {
                        id = gameId
                        status = GameStatus.Open
                        boardRegionCount = 3
                        players = List.empty
                        pieces = List.empty
                        selectionOptions = List.empty
                    }

                    let placeHolderResponse = {
                        text = sprintf "Reset turn %i not yet implemented" gameId
                    } 
                    return! json placeHolderResponse next ctx
                }

        member this.commitTurn(gameId : int) =
            fun (next : HttpFunc) (ctx : HttpContext) ->
                task {
                    let response : GameDetailsDto = {
                        id = gameId
                        status = GameStatus.Open
                        boardRegionCount = 3
                        players = List.empty
                        pieces = List.empty
                        selectionOptions = List.empty
                    }

                    let placeHolderResponse = {
                        text = sprintf "Commit turn %i not yet implemented" gameId
                    }
                    return! json placeHolderResponse next ctx
                }

        member this.sendMessage(gameId : int) =
            fun (next : HttpFunc) (ctx : HttpContext) ->
                task {
                    let! request = ctx.BindModelAsync<CreateMessageDto>()

                    let response = {
                        text = sprintf "Send message %i not yet implemented" gameId
                    }
                    return! json response next ctx
                }