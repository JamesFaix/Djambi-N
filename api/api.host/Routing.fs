namespace Djambi.Api.Http

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
                    DELETE >=> routef Routes.userFormat UserController.deleteUser
                    
                //Board
                    GET >=> routef Routes.boardFormat BoardController.getBoard
                    GET >=> routef Routes.pathsFormat BoardController.getCellPaths

                //Game lobby
                    POST >=> route Routes.gamesQuery >=> GameController.getGames
                    POST >=> route Routes.games >=> GameController.createGame
                    GET >=> routef Routes.gameFormat GameController.getGame
                    PUT >=> routef Routes.gameParametersFormat GameController.updateGameParameters
                    POST >=> routef Routes.startGameFormat GameController.startGame

                //Players
                    POST >=> routef Routes.playersFormat GameController.addPlayer
                    DELETE >=> routef Routes.playerFormat GameController.removePlayer
                    PUT >=> routef Routes.playerStatusChangeFormat GameController.updatePlayerStatus

                //Turn actions
                    POST >=> routef Routes.selectCellFormat GameController.selectCell
                    POST >=> routef Routes.resetTurnFormat GameController.resetTurn
                    POST >=> routef Routes.commitTurnFormat GameController.commitTurn

                //Events
                    POST >=> routef Routes.eventsQueryFormat GameController.getEvents

                //Snapshots
                    POST >=> routef Routes.snapshotsFormat SnapshotController.createSnapshot
                    GET >=> routef Routes.snapshotsFormat SnapshotController.getSnapshotsForGame
                    DELETE >=> routef Routes.snapshotFormat SnapshotController.deleteSnapshot
                    POST >=> routef Routes.snapshotLoadFormat SnapshotController.loadSnapshot
                ])
            setStatusCode 404 >=> text "Not Found" ]