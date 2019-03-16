namespace Djambi.Api.Web

open Djambi.Api.Web.Controllers
open Djambi.Api.Web.Interfaces
open Djambi.Api.Web
open Djambi.Api.Logic.Interfaces

type WebRoot(cookieDomain : string,
             managers : IManagerRoot) =
    member x.util = HttpUtility(cookieDomain)
    member x.boards = BoardController(managers.boards, x.util)
    member x.events = EventController(managers.events, x.util)
    member x.games = GameController(managers.games, x.util)
    member x.players = PlayerController(x.util, managers.players)
    member x.sessions = SessionController(x.util, managers.sessions)
    member x.snapshots = SnapshotController(x.util, managers.snapshots)
    member x.turns = TurnController(x.util, managers.turns)
    member x.users = UserController(x.util, managers.users)

    interface IWebRoot with
        member x.boards = x.boards :> IBoardController
        member x.events = x.events :> IEventController
        member x.games = x.games :> IGameController
        member x.players = x.players :> IPlayerController
        member x.sessions = x.sessions :> ISessionController
        member x.snapshots = x.snapshots :> ISnapshotController
        member x.turns = x.turns :> ITurnController
        member x.users = x.users :> IUserController