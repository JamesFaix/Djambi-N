namespace Djambi.Api.Http

open Giraffe

module Routing = 

    let getRoutingTable (controllers : ControllerRegistry) =
        choose [
            subRoute "/api"
                (choose [  
            
                //Lobby
                    POST >=> route "/users" >=> controllers.lobby.createUser
                    GET >=> routef "/users/%i" controllers.lobby.getUser
                    GET >=> route "/users" >=> controllers.lobby.getUsers
                    DELETE >=> routef "/users/%i" controllers.lobby.deleteUser
                    PATCH >=> routef "/users/%i" controllers.lobby.updateUser

                    GET >=> route "/games/open" >=> controllers.lobby.getOpenGames
                    GET >=> route "/games" >=> controllers.lobby.getGames
                    POST >=> route "/games" >=> controllers.lobby.createGame
                    DELETE >=> routef "/games/%i" controllers.lobby.deleteGame

                    POST >=> routef "/games/%i/users/%i" controllers.lobby.addPlayerToGame
                    DELETE >=> routef "/games/%i/users/%i" controllers.lobby.removePlayerFromGame

                    POST >=> routef "/games/%i/start-request" controllers.play.startGame

                //Play
                    GET >=> routef "/boards/%i" controllers.play.getBoard
                    GET >=> routef "/boards/%i/cells/%i/paths" controllers.play.getCellPaths
                                    
                    GET >=> routef "/games/%i/state" controllers.play.getGameState

                    POST >=> routef "/games/%i/current-turn/selection-request/%i" controllers.play.selectCell
                    POST >=> routef "/games/%i/current-turn/reset-request" controllers.play.resetTurn
                    POST >=> routef "/games/%i/current-turn/commit-request" controllers.play.commitTurn

                    POST >=> routef "/games/%id/messages" controllers.play.sendMessage
                ])
            setStatusCode 404 >=> text "Not Found" ]