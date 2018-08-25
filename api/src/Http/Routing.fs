namespace Djambi.Api.Http

open Giraffe

module Routing = 
    open Microsoft.AspNetCore.Http

    let getRoutingTable : HttpFunc -> HttpContext -> HttpFuncResult =
        choose [
            subRoute "/api"
                (choose [  
            
                //Lobby
                    POST >=> route "/users" >=> LobbyController.createUser
                    GET >=> routef "/users/%i" LobbyController.getUser
                    GET >=> route "/users" >=> LobbyController.getUsers
                    DELETE >=> routef "/users/%i" LobbyController.deleteUser
                    PATCH >=> routef "/users/%i" LobbyController.updateUser

                    GET >=> route "/games/open" >=> LobbyController.getOpenGames
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