﻿namespace Djambi.Api.Http

open Giraffe
open Microsoft.AspNetCore.Http
open Djambi.Api.Web.Controllers
open Djambi.Api.Logic

module Routing =

    let getRoutingTable : HttpFunc -> HttpContext -> HttpFuncResult =
        choose [
            subRoute "/api"
                (choose [

                //Session
                    POST >=> route Routes.sessions >=> SessionController.openSession
                    DELETE >=> route Routes.sessions >=> SessionController.closeSession

                //Users
                    POST >=> route Routes.users >=> UserController.createUser
                    GET >=> routef Routes.userFormat UserController.getUser
                    GET >=> route Routes.currentUser >=> UserController.getCurrentUser
                    GET >=> route Routes.users >=> UserController.getUsers
                    DELETE >=> routef Routes.userFormat UserController.deleteUser
                    
                //Board
                    GET >=> routef Routes.boardFormat BoardController.getBoard
                    GET >=> routef Routes.pathsFormat BoardController.getCellPaths

                //Game
                    POST >=> route Routes.gamesQuery >=> GameController.getGames
                    POST >=> route Routes.games >=> GameController.createGame
                    GET >=> routef Routes.gameFormat GameController.getGame
                    PUT >=> routef Routes.gameParametersFormat GameController.updateGameParameters

                    POST >=> routef Routes.playersFormat GameController.addPlayer
                    DELETE >=> routef Routes.playerFormat GameController.removePlayer

                    POST >=> routef Routes.startGameFormat GameController.startGame

                    POST >=> routef Routes.selectCellFormat GameController.selectCell
                    POST >=> routef Routes.resetTurnFormat GameController.resetTurn
                    POST >=> routef Routes.commitTurnFormat GameController.commitTurn

                    POST >=> routef Routes.eventsQueryFormat GameController.getEvents
                ])
            setStatusCode 404 >=> text "Not Found" ]