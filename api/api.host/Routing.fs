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
                    
                //Board
                    GET >=> routef "/boards/%i" BoardController.getBoard
                    GET >=> routef "/boards/%i/cells/%i/paths" BoardController.getCellPaths

                //Game
                    POST >=> route "/games/query" >=> GameController.getGames
                    POST >=> route "/games" >=> GameController.createGame
                    GET >=> routef "/games/%i" GameController.getGame
                    PUT >=> routef "/games/%i/parameters" GameController.updateGameParameters

                    POST >=> routef "/games/%i/players" GameController.addPlayer
                    DELETE >=> routef "/games/%i/players/%i" GameController.removePlayer

                    POST >=> routef "/games/%i/start-request" GameController.startGame

                    POST >=> routef "/games/%i/current-turn/selection-request/%i" GameController.selectCell
                    POST >=> routef "/games/%i/current-turn/reset-request" GameController.resetTurn
                    POST >=> routef "/games/%i/current-turn/commit-request" GameController.commitTurn

                    POST >=> routef "/games/%i/events/query" GameController.getEvents
                ])
            setStatusCode 404 >=> text "Not Found" ]