namespace Djambi.Api.Db

open Djambi.Api.Db.Repositories
open Djambi.Api.Db.Interfaces

type DbRoot(connectionString : string) =
    let ctxProvider = CommandContextProvider(connectionString)
    let util = SqlUtility(ctxProvider)
    member x.games = GameRepository(util)
    member x.users = UserRepository(util)
    member x.events = EventRepository(util, x.games)
    member x.sessions = SessionRepository(util, x.users)
    member x.snapshots = SnapshotRepository(util)

    interface IDbRoot with  
        member x.events = x.events :> IEventRepository
        member x.games = x.games :> IGameRepository
        member x.sessions = x.sessions :> ISessionRepository
        member x.snapshots = x.snapshots :> ISnapshotRepository
        member x.users = x.users :> IUserRepository