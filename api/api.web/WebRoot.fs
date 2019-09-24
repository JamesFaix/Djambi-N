namespace Djambi.Api.Web

open Serilog
open Djambi.Api.Logic.Interfaces
open Djambi.Api.Web
open Djambi.Api.Web.Controllers
open Djambi.Api.Web.Interfaces

type WebRoot(cookieDomain : string,
             managers : IManagerRoot,
             services : IServiceRoot,
             log : ILogger) =
    let _util = HttpUtility(cookieDomain, services.sessions, log)
    let _boards = BoardController(managers.boards, _util)
    let _events = EventController(managers.events, _util)
    let _games = GameController(managers.games, _util)
    let _notifications = NotificationController(_util, services.notifications, log)
    let _players = PlayerController(_util, managers.players)
    let _search = SearchController(managers.search, _util)
    let _sessions = SessionController(_util, managers.sessions)
    let _snapshots = SnapshotController(_util, managers.snapshots)
    let _turns = TurnController(_util, managers.turns)
    let _users = UserController(_util, managers.users)

    member x.util = _util
    member x.boards = _boards
    member x.events = _events
    member x.games = _games
    member x.notifications = _notifications
    member x.players = _players
    member x.search = _search
    member x.sessions = _sessions
    member x.snapshots = _snapshots
    member x.turns = _turns
    member x.users = _users

    interface IWebRoot with
        member x.boards = x.boards :> IBoardController
        member x.events = x.events :> IEventController
        member x.games = x.games :> IGameController
        member x.notifications = x.notifications :> INotificationsController
        member x.players = x.players :> IPlayerController
        member x.search = x.search :> ISearchController
        member x.sessions = x.sessions :> ISessionController
        member x.snapshots = x.snapshots :> ISnapshotController
        member x.turns = x.turns :> ITurnController
        member x.users = x.users :> IUserController