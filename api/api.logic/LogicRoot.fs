namespace Djambi.Api.Logic

open Djambi.Api.Logic.Interfaces
open Djambi.Api.Logic.Managers
open Djambi.Api.Logic.Services
open Djambi.Api.Db.Interfaces

type ServiceRoot(db : IDbRoot) =
    member x.boards = BoardService()
    member x.gameCrud = GameCrudService(db.games)
    member x.players = PlayerService(db.games)
    member x.selectionOptions = SelectionOptionsService()
    member x.sessions = SessionService(db.sessions, db.users)
    member x.users = UserService(db.users)
    member x.gameStart = GameStartService(x.players, x.selectionOptions)
    member x.events = EventService(x.gameStart)
    member x.indirectEffects = IndirectEffectsService(x.events, x.selectionOptions)
    member x.playerStatusChanges = PlayerStatusChangeService(x.events, x.indirectEffects)
    member x.selections = SelectionService(x.selectionOptions)
    member x.turns = TurnService(x.events, x.indirectEffects, x.selectionOptions)

    interface IServiceRoot with
        member x.sessions = x.sessions :> ISessionService

type ManagerRoot(db : IDbRoot, services : ServiceRoot) =
    member x.boards = BoardManager(services.boards)
    member x.games = GameManager(db.events,
                                 services.events, 
                                 services.gameCrud, 
                                 db.games,
                                 services.gameStart, 
                                 services.players, 
                                 services.playerStatusChanges,
                                 services.selections,
                                 services.turns)
    member x.sessions = SessionManager(services.sessions)
    member x.snapshots = SnapshotManager(db.events, db.games, db.snapshots)
    member x.users = UserManager(services.users)

    interface IManagerRoot with
        member x.boards = x.boards :> IBoardManager
        member x.events = x.games :> IEventManager
        member x.games = x.games :> IGameManager
        member x.players = x.games :> IPlayerManager
        member x.sessions = x.sessions :> ISessionManager
        member x.snapshots = x.snapshots :> ISnapshotManager
        member x.turns = x.games :> ITurnManager
        member x.users = x.users :> IUserManager