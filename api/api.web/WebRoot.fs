namespace Djambi.Api.Web

open Djambi.Api.Web.Controllers
open Djambi.Api.Web.Interfaces

type WebRoot() =
    let games = GameController()

    interface IWebRoot with
        member x.boards = BoardController() :> IBoardController
        member x.events = games :> IEventController
        member x.games = games :> IGameController
        member x.players = games :> IPlayerController
        member x.sessions = SessionController() :> ISessionController
        member x.snapshots = SnapshotController() :> ISnapshotController
        member x.turns = games :> ITurnController
        member x.users = UserController() :> IUserController