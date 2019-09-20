namespace Djambi.Api.Logic

open Serilog
open Djambi.Api.Db.Interfaces
open Djambi.Api.Logic.Interfaces
open Djambi.Api.Logic.Managers
open Djambi.Api.Logic.Services

type ServiceRoot(db : IDbRoot, log : ILogger) =
    let _boards = BoardService()
    let _gameCrud = GameCrudService(db.games)
    let _notifications = NotificationService(log)
    let _players = PlayerService(db.games)
    let _selectionOptions = SelectionOptionsService()
    let _sessions = SessionService(db.sessions, db.users)
    let _users = UserService(db.users)
    let _gameStart = GameStartService(_players, _selectionOptions)
    let _events = EventService(_gameStart)
    let _indirectEffects = IndirectEffectsService(_events, _selectionOptions)
    let _playerStatusChanges = PlayerStatusChangeService(_events, _indirectEffects)
    let _selections = SelectionService(_selectionOptions)
    let _turns = TurnService(_events, _indirectEffects, _selectionOptions)

    member x.boards = _boards
    member x.events = _events
    member x.gameCrud = _gameCrud
    member x.gameStart = _gameStart
    member x.indirectEffects = _indirectEffects
    member x.notifications = _notifications
    member x.players = _players
    member x.playerStatusChanges = _playerStatusChanges
    member x.selectionOptions = _selectionOptions
    member x.selections = _selections
    member x.sessions = _sessions
    member x.turns = _turns
    member x.users = _users

    interface IServiceRoot with
        member x.notifications = x.notifications :> INotificationService
        member x.sessions = x.sessions :> ISessionService

type ManagerRoot(db : IDbRoot, services : ServiceRoot) =
    let _boards = BoardManager(services.boards)
    let _games = GameManager(db.events,
                             services.events,
                             services.gameCrud,
                             db.games,
                             services.gameStart,
                             services.notifications,
                             services.players,
                             services.playerStatusChanges,
                             services.selections,
                             services.turns)
    let _sessions = SessionManager(services.sessions)
    let _snapshots = SnapshotManager(db.events, db.games, db.snapshots)
    let _users = UserManager(services.users)

    member x.boards = _boards
    member x.games = _games
    member x.sessions = _sessions
    member x.snapshots = _snapshots
    member x.users = _users

    interface IManagerRoot with
        member x.boards = x.boards :> IBoardManager
        member x.events = x.games :> IEventManager
        member x.games = x.games :> IGameManager
        member x.players = x.games :> IPlayerManager
        member x.sessions = x.sessions :> ISessionManager
        member x.snapshots = x.snapshots :> ISnapshotManager
        member x.turns = x.games :> ITurnManager
        member x.users = x.users :> IUserManager