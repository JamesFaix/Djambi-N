namespace djambi.api

module HttpHandlers =

    open Microsoft.AspNetCore.Http
    open Giraffe
    open djambi.api.Models

//Users
    let handleCreateUser =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! request = ctx.BindModelAsync<CreateUserRequestDto>()

                let response : UserDto = {
                    Id = 1
                    Name = request.Name
                }

                let placeHolderResponse = {
                    Text = "Create user not yet implemented"
                }
                return! json placeHolderResponse next ctx
            }

    let handleDeleteUser(userId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let placeHolderResponse = {
                    Text = sprintf "Delete user %i not yet implemented" userId
                }
                return! json placeHolderResponse next ctx
            }

    let handleGetUser(userId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let response : UserDto = {
                    Id = userId
                    Name = "Test"
                }

                let placeHolderResponse = {
                    Text = sprintf "Get user %i not yet implemented" userId
                }
                return! json placeHolderResponse next ctx
            }

    let handleUpdateUser(userId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! request = ctx.BindModelAsync<CreateUserRequestDto>()

                let response : UserDto = {
                    Id = userId
                    Name = request.Name
                }

                let placeHolderResponse = {
                    Text = sprintf "Update user %i not yet implemented" userId
                }
                return! json placeHolderResponse next ctx
            }

//Game lobby
    let handleGetOpenGames =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {

                let response : GameMetadataDto list = 
                    [
                        {
                            Id = 1
                            Status = GameStatus.Open
                            BoardRegionCount = 3
                            Players = 
                            [
                                {
                                    Id = 1
                                    Name = "TestUser"
                                }
                            ]
                        }
                    ]

                let placeHolderResponse = {
                    Text = "Get open games not yet implemented"
                }
                return! json placeHolderResponse next ctx
            }

    let handleCreateGame =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! request = ctx.BindModelAsync<CreateGameRequestDto>()

                let response : GameMetadataDto = {
                    Id = 1
                    Status = GameStatus.Open
                    BoardRegionCount = request.BoardRegionCount
                    Players = List.empty
                }

                let placeHolderResponse = {
                    Text = "Create game not yet implemented"
                }
                return! json placeHolderResponse next ctx
            }

    let handleDeleteGame(gameId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let placeHolderResponse = {
                    Text = sprintf "Delete game %i not yet implemented" gameId
                }
                return! json placeHolderResponse next ctx
            }

    let handleAddPlayerToGame(gameId : int, userId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let placeHolderResponse = {
                    Text = sprintf "Add player %i to game %i not yet implemented" userId gameId
                }
                return! json placeHolderResponse next ctx
            }

    let handleDeletePlayerFromGame(gameId : int, userId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let placeHolderResponse = {
                    Text = sprintf "Delete player %i from game %i not yet implemented" userId gameId
                }
                return! json placeHolderResponse next ctx
            }

    let handleStartGame(gameId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let placeHolderResponse = {
                    Text = sprintf "Start game %i not yet implemented" gameId
                }
                return! json placeHolderResponse next ctx
            }

//Gameplay
    let handleGetGameState(gameId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let response : GameDetailsDto = {
                    Id = gameId
                    Status = GameStatus.Open
                    BoardRegionCount = 3
                    Players = List.empty
                    Pieces = List.empty
                }

                let placeHolderResponse = {
                    Text = sprintf "Get game state %i not yet implemented" gameId
                }
                return! json placeHolderResponse next ctx
            }

    let handleMakeSelection(gameId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! request = ctx.BindModelAsync<CreateSelectionDto>()

                let response : GameDetailsDto = {
                    Id = gameId
                    Status = GameStatus.Open
                    BoardRegionCount = 3
                    Players = List.empty
                    Pieces = List.empty
                }
                
                let placeHolderResponse = {
                    Text = sprintf "Make selection %i not yet implemented" gameId
                }
                return! json placeHolderResponse next ctx
            }

    let handleResetTurn(gameId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let response : GameDetailsDto = {
                    Id = gameId
                    Status = GameStatus.Open
                    BoardRegionCount = 3
                    Players = List.empty
                    Pieces = List.empty
                }

                let placeHolderResponse = {
                    Text = sprintf "Reset turn %i not yet implemented" gameId
                } 
                return! json placeHolderResponse next ctx
            }

    let handleCommitTurn(gameId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let response : GameDetailsDto = {
                    Id = gameId
                    Status = GameStatus.Open
                    BoardRegionCount = 3
                    Players = List.empty
                    Pieces = List.empty
                }

                let placeHolderResponse = {
                    Text = sprintf "Commit turn %i not yet implemented" gameId
                }
                return! json placeHolderResponse next ctx
            }

    let handleSendMessage(gameId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! request = ctx.BindModelAsync<CreateMessageDto>()

                let response = {
                    Text = sprintf "Send message %i not yet implemented" gameId
                }
                return! json response next ctx
            }