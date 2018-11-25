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
                    POST >=> route "/sessions" >=> SessionController.openSession
                    DELETE >=> route "/sessions" >=> SessionController.closeSession

                //Users
                    POST >=> route "/users" >=> UserController.createUser
                    GET >=> routef "/users/%i" UserController.getUser
                    GET >=> route "/users/current" >=> UserController.getCurrentUser
                    GET >=> route "/users" >=> UserController.getUsers
                    DELETE >=> routef "/users/%i" UserController.deleteUser

                //Lobby
                    POST >=> route "/lobbies/query" >=> LobbyController.getLobbies
                    POST >=> route "/lobbies" >=> LobbyController.createLobby
                    DELETE >=> routef "/lobbies/%i" LobbyController.deleteLobby

                    POST >=> routef "/lobbies/%i/players" PlayerController.addPlayerToLobby
                    DELETE >=> routef "/lobbies/%i/players/%i" PlayerController.removePlayerFromLobby

                    POST >=> routef "/lobbies/%i/start-request" LobbyController.startGame

                //Board
                    GET >=> routef "/boards/%i" BoardController.getBoard
                    GET >=> routef "/boards/%i/cells/%i/paths" BoardController.getCellPaths

                //Game
                    GET >=> routef "/games/%i/state" GameController.getGameState

                    POST >=> routef "/games/%i/current-turn/selection-request/%i" GameController.selectCell
                    POST >=> routef "/games/%i/current-turn/reset-request" GameController.resetTurn
                    POST >=> routef "/games/%i/current-turn/commit-request" GameController.commitTurn

                ])
            setStatusCode 404 >=> text "Not Found" ]