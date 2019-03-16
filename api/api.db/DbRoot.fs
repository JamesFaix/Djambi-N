namespace Djambi.Api.Db

open Djambi.Api.Db.Repositories
open Djambi.Api.Db.Interfaces

type DbRoot() =
    member x.games = GameRepository()
    member x.users = UserRepository()
    member x.events = EventRepository(x.games)
    member x.sessions = SessionRepository(x.users)
    member x.snapshots = SnapshotRepository(x.games)

    interface IDbRoot with  
        member x.events = x.events :> IEventRepository
        member x.games = x.games :> IGameRepository
        member x.sessions = x.sessions :> ISessionRepository
        member x.snapshots = x.snapshots :> ISnapshotRepository
        member x.users = x.users :> IUserRepository