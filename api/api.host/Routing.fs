namespace Djambi.Api.Http

open Giraffe
open Microsoft.AspNetCore.Http
open Djambi.Api.Web.Controllers

module Routing = 

    let getRoutingTable : HttpFunc -> HttpContext -> HttpFuncResult =
        choose [
            subRoute "/api"
                (choose [  
            
                //Session                    
                    POST >=> route "/sessions" >=> SessionController.createSessionWithUser
                    DELETE >=> route "/sessions" >=> SessionController.closeSession
                    POST >=> route "/sessions/users" >=> SessionController.addUserToSession
                    DELETE >=> routef "/sessions/users/%i" SessionController.removeUserFromSession

                //Users
                    POST >=> route "/users" >=> UserController.createUser
                    GET >=> routef "/users/%i" UserController.getUser
                    GET >=> route "/users" >=> UserController.getUsers
                    DELETE >=> routef "/users/%i" UserController.deleteUser
                    PATCH >=> routef "/users/%i" UserController.updateUser

                //Lobby
                    GET >=> route "/games/open" >=> LobbyController.getOpenGames
                    GET >=> routef "/users/%i/games" LobbyController.getUserGames
                    GET >=> route "/games" >=> LobbyController.getGames
                    POST >=> route "/games" >=> LobbyController.createGame
                    DELETE >=> routef "/games/%i" LobbyController.deleteGame

                    POST >=> routef "/games/%i/users/%i" LobbyController.addPlayerToGame
                    DELETE >=> routef "/games/%i/users/%i" LobbyController.removePlayerFromGame

                    POST >=> routef "/games/%i/start-request" PlayController.startGame

                //Play
                    GET >=> routef "/boards/%i" PlayController.getBoard
                    GET >=> routef "/boards/%i/cells/%i/paths" PlayController.getCellPaths
                                    
                    GET >=> routef "/games/%i/state" PlayController.getGameState

                    POST >=> routef "/games/%i/current-turn/selection-request/%i" PlayController.selectCell
                    POST >=> routef "/games/%i/current-turn/reset-request" PlayController.resetTurn
                    POST >=> routef "/games/%i/current-turn/commit-request" PlayController.commitTurn

                    POST >=> routef "/games/%id/messages" PlayController.sendMessage
                ])
            setStatusCode 404 >=> text "Not Found" ]