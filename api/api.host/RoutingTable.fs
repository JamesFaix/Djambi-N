namespace Djambi.Api.Host

open Giraffe
open Microsoft.AspNetCore.Http
open Djambi.Api.Web.Interfaces
open Djambi.Api.Logic.Interfaces

type RoutingTable (web : IWebRoot) =
    member x.getHandler : HttpFunc -> HttpContext -> HttpFuncResult =
        choose [
            subRoute "/api"
                (choose [

                //Session
                    POST >=> route Routes.sessions >=> web.sessions.openSession
                    DELETE >=> route Routes.sessions >=> web.sessions.closeSession

                //Users
                    POST >=> route Routes.users >=> web.users.createUser
                    GET >=> routef Routes.userFormat web.users.getUser
                    GET >=> route Routes.currentUser >=> web.users.getCurrentUser
                    DELETE >=> routef Routes.userFormat web.users.deleteUser
                    
                //Board
                    GET >=> routef Routes.boardFormat web.boards.getBoard
                    GET >=> routef Routes.pathsFormat web.boards.getCellPaths

                //Game lobby
                    POST >=> route Routes.gamesQuery >=> web.games.getGames
                    POST >=> route Routes.games >=> web.games.createGame
                    GET >=> routef Routes.gameFormat web.games.getGame
                    PUT >=> routef Routes.gameParametersFormat web.games.updateGameParameters
                    POST >=> routef Routes.startGameFormat web.games.startGame

                //Players
                    POST >=> routef Routes.playersFormat web.players.addPlayer
                    DELETE >=> routef Routes.playerFormat web.players.removePlayer
                    PUT >=> routef Routes.playerStatusChangeFormat web.players.updatePlayerStatus

                //Turn actions
                    POST >=> routef Routes.selectCellFormat web.turns.selectCell
                    POST >=> routef Routes.resetTurnFormat web.turns.resetTurn
                    POST >=> routef Routes.commitTurnFormat web.turns.commitTurn

                //Events
                    POST >=> routef Routes.eventsQueryFormat web.events.getEvents

                //Snapshots
                    POST >=> routef Routes.snapshotsFormat web.snapshots.createSnapshot
                    GET >=> routef Routes.snapshotsFormat web.snapshots.getSnapshotsForGame
                    DELETE >=> routef Routes.snapshotFormat web.snapshots.deleteSnapshot
                    POST >=> routef Routes.snapshotLoadFormat web.snapshots.loadSnapshot

                //Notifications
                    GET >=> route Routes.notificationsForCurrentUser >=> web.notifications.getNotificationsForCurrentUser
                    GET >=> routef Routes.notificationsForCurrentUserForGameFormat web.notifications.getNotificationsForCurrentUserForGame
                ])
            setStatusCode 404 >=> text "Not Found" ]