namespace djambi.api

module HttpHandlers =

    open Microsoft.AspNetCore.Http
    open Giraffe
    open djambi.api.Models

//Users
    let handleCreateUser =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let response = {
                    Text = "Create user not yet implemented"
                }
                return! json response next ctx
            }

    let handleDeleteUser(userId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let response = {
                    Text = sprintf "Delete user %i not yet implemented" userId
                }
                return! json response next ctx
            }

    let handleGetUser(userId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let response = {
                    Text = sprintf "Get user %i not yet implemented" userId
                }
                return! json response next ctx
            }

    let handleUpdateUser(userId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let response = {
                    Text = sprintf "Update user %i not yet implemented" userId
                }
                return! json response next ctx
            }

//Game lobby
    let handleGetOpenGames =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let response = {
                    Text = "Get open games not yet implemented"
                }
                return! json response next ctx
            }

    let handleCreateGame =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let response = {
                    Text = "Create game not yet implemented"
                }
                return! json response next ctx
            }

    let handleDeleteGame(gameId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let response = {
                    Text = sprintf "Delete game %i not yet implemented" gameId
                }
                return! json response next ctx
            }

    let handleAddPlayerToGame(gameId : int, userId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let response = {
                    Text = sprintf "Add player %i to game %i not yet implemented" userId gameId
                }
                return! json response next ctx
            }

    let handleDeletePlayerFromGame(gameId : int, userId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let response = {
                    Text = sprintf "Delete player %i from game %i not yet implemented" userId gameId
                }
                return! json response next ctx
            }

    let handleStartGame(gameId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let response = {
                    Text = sprintf "Start game %i not yet implemented" gameId
                }
                return! json response next ctx
            }

//Gameplay
    let handleGetGameState(gameId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let response = {
                    Text = sprintf "Get game state %i not yet implemented" gameId
                }
                return! json response next ctx
            }

    let handleMakeSelection(gameId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let response = {
                    Text = sprintf "Make selection %i not yet implemented" gameId
                }
                return! json response next ctx
            }

    let handleResetTurn(gameId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let response = {
                    Text = sprintf "Reset turn %i not yet implemented" gameId
                } 
                return! json response next ctx
            }

    let handleCommitTurn(gameId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let response = {
                    Text = sprintf "Commit turn %i not yet implemented" gameId
                }
                return! json response next ctx
            }

    let handleSendMessage(gameId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let response = {
                    Text = sprintf "Send message %i not yet implemented" gameId
                }
                return! json response next ctx
            }