namespace Djambi.Api.Web

open Djambi.Api.Web.Controllers
open Djambi.Api.Web.Interfaces
open Djambi.Api.Web

type WebRoot(cookieDomain : string) =
    member x.util = HttpUtility(cookieDomain)
    member x.boards = BoardController(x.util)
    member x.events = EventController(x.util)
    member x.games = GameController(x.util)
    member x.players = PlayerController(x.util)
    member x.sessions = SessionController(x.util)
    member x.snapshots = SnapshotController(x.util)
    member x.turns = TurnController(x.util)
    member x.users = UserController(x.util)

    interface IWebRoot with
        member x.boards = x.boards :> IBoardController
        member x.events = x.events :> IEventController
        member x.games = x.games :> IGameController
        member x.players = x.players :> IPlayerController
        member x.sessions = x.sessions :> ISessionController
        member x.snapshots = x.snapshots :> ISnapshotController
        member x.turns = x.turns :> ITurnController
        member x.users = x.users :> IUserController