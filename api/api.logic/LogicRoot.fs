namespace Djambi.Api.Logic

open Djambi.Api.Logic.Interfaces
open Djambi.Api.Logic.Managers

type ManagerRoot() =
    member x.boards = BoardManager()
    member x.games = GameManager()
    member x.sessions = SessionManager()
    member x.snapshots = SnapshotManager()
    member x.users = UserManager()

    interface IManagerRoot with
        member x.boards = x.boards :> IBoardManager
        member x.events = x.games :> IEventManager
        member x.games = x.games :> IGameManager
        member x.players = x.games :> IPlayerManager
        member x.sessions = x.sessions :> ISessionManager
        member x.snapshots = x.snapshots :> ISnapshotManager
        member x.turns = x.games :> ITurnManager
        member x.users = x.users :> IUserManager