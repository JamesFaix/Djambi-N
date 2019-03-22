namespace Djambi.Api.Db

open Djambi.Api.Db.Repositories
open Djambi.Api.Db.Interfaces

type DbRoot(connectionString : string) =
    let ctxProvider = CommandContextProvider(connectionString)
    member x.games = GameRepository(ctxProvider)
    member x.users = UserRepository(ctxProvider)
    member x.events = EventRepository(ctxProvider, x.games)
    member x.sessions = SessionRepository(ctxProvider, x.users)
    member x.snapshots = SnapshotRepository(ctxProvider)

    interface IDbRoot with  
        member x.events = x.events :> IEventRepository
        member x.games = x.games :> IGameRepository
        member x.sessions = x.sessions :> ISessionRepository
        member x.snapshots = x.snapshots :> ISnapshotRepository
        member x.users = x.users :> IUserRepository