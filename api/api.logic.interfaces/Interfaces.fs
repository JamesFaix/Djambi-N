namespace Djambi.Api.Logic.Interfaces

open Djambi.Api.Common.Control
open Djambi.Api.Model
open Djambi.ClientGenerator.Annotations

type ISessionService =
    abstract member openSession : request:LoginRequest -> Session AsyncHttpResult
    abstract member renewSession : token:string -> Session AsyncHttpResult
    abstract member getSession : token:string -> Session AsyncHttpResult
    abstract member closeSession : session:Session -> Unit AsyncHttpResult

type IBoardManager =    
    [<ClientFunction(HttpMethod.Get, Routes.board, ClientSection.Board)>]
    abstract member getBoard : regionCount:int -> session:Session -> Board AsyncHttpResult
            
    [<ClientFunction(HttpMethod.Get, Routes.paths, ClientSection.Board)>]
    abstract member getCellPaths : regionCount:int * cellId:int -> session:Session -> int list list AsyncHttpResult

type IEventManager =
    [<ClientFunction(HttpMethod.Post, Routes.eventsQuery, ClientSection.Events)>]
    abstract member getEvents : gameId:int * query:EventsQuery -> session:Session -> Event list AsyncHttpResult

type IGameManager =    
    [<ClientFunction(HttpMethod.Post, Routes.gamesQuery, ClientSection.Game)>]
    abstract member getGames : query:GamesQuery -> session:Session -> Game list AsyncHttpResult

    [<ClientFunction(HttpMethod.Get, Routes.game, ClientSection.Game)>]
    abstract member getGame : gameId:int -> session:Session -> Game AsyncHttpResult
    
    [<ClientFunction(HttpMethod.Post, Routes.games, ClientSection.Game)>]
    abstract member createGame : parameters:GameParameters -> session:Session -> Game AsyncHttpResult
    
    [<ClientFunction(HttpMethod.Put, Routes.gameParameters, ClientSection.Game)>]
    abstract member updateGameParameters : gameId:int -> parameters:GameParameters -> session:Session -> StateAndEventResponse AsyncHttpResult
    
    [<ClientFunction(HttpMethod.Post, Routes.startGame, ClientSection.Game)>]
    abstract member startGame : gameId:int -> session:Session -> StateAndEventResponse AsyncHttpResult

type IPlayerManager =
    [<ClientFunction(HttpMethod.Post, Routes.players, ClientSection.Player)>]
    abstract member addPlayer : gameId:int -> request:CreatePlayerRequest -> session:Session -> StateAndEventResponse AsyncHttpResult

    [<ClientFunction(HttpMethod.Delete, Routes.player, ClientSection.Player)>]
    abstract member removePlayer : gameId:int * playerId:int -> session:Session -> StateAndEventResponse AsyncHttpResult
    
    [<ClientFunction(HttpMethod.Put, Routes.playerStatusChange, ClientSection.Player)>]
    abstract member updatePlayerStatus : gameId:int * playerId:int * status:PlayerStatus -> session:Session -> StateAndEventResponse AsyncHttpResult

type ISessionManager =
    [<ClientFunction(HttpMethod.Post, Routes.sessions, ClientSection.Session)>]
    abstract member login : request:LoginRequest -> Session AsyncHttpResult
      
    [<ClientFunction(HttpMethod.Delete, Routes.sessions, ClientSection.Session)>]
    abstract member logout : session:Session -> unit AsyncHttpResult

type ISnapshotManager =
    [<ClientFunction(HttpMethod.Post, Routes.snapshots, ClientSection.Snapshots)>]
    abstract member createSnapshot : gameId:int -> request:CreateSnapshotRequest -> session:Session -> SnapshotInfo AsyncHttpResult
    
    [<ClientFunction(HttpMethod.Get, Routes.snapshots, ClientSection.Snapshots)>]
    abstract member getSnapshotsForGame : gameId:int -> session:Session -> SnapshotInfo list AsyncHttpResult
    
    [<ClientFunction(HttpMethod.Delete, Routes.snapshot, ClientSection.Snapshots)>]
    abstract member deleteSnapshot : gameId:int -> snapshotId:int -> session:Session -> unit AsyncHttpResult
   
    [<ClientFunction(HttpMethod.Post, Routes.snapshotLoad, ClientSection.Snapshots)>]
    abstract member loadSnapshot : gameId:int -> snapshotId:int -> session:Session -> unit AsyncHttpResult
       
type ITurnManager =
    [<ClientFunction(HttpMethod.Post, Routes.selectCell, ClientSection.Turn)>]
    abstract member selectCell : gameId:int * cellId:int -> session:Session -> StateAndEventResponse AsyncHttpResult
       
    [<ClientFunction(HttpMethod.Post, Routes.resetTurn, ClientSection.Turn)>]
    abstract member resetTurn : gameId:int -> session:Session -> StateAndEventResponse AsyncHttpResult
        
    [<ClientFunction(HttpMethod.Post, Routes.commitTurn, ClientSection.Turn)>]
    abstract member commitTurn : gameId:int -> session:Session -> StateAndEventResponse AsyncHttpResult
  
type IUserManager =
    [<ClientFunction(HttpMethod.Post, Routes.users, ClientSection.User)>]
    abstract member createUser : request:CreateUserRequest -> sessionOption:Session option -> User AsyncHttpResult

    [<ClientFunction(HttpMethod.Delete, Routes.user, ClientSection.User)>]
    abstract member deleteUser : userId:int -> session:Session -> unit AsyncHttpResult
    
    [<ClientFunction(HttpMethod.Get, Routes.user, ClientSection.User)>]
    abstract member getUser : userId:int -> session:Session -> User AsyncHttpResult
    
    [<ClientFunction(HttpMethod.Get, Routes.currentUser, ClientSection.User)>]
    abstract member getCurrentUser : session:Session -> User AsyncHttpResult

type IServiceRoot =
    abstract member sessions : ISessionService

type IManagerRoot =
    abstract member boards : IBoardManager
    abstract member events : IEventManager
    abstract member games : IGameManager
    abstract member players : IPlayerManager
    abstract member sessions : ISessionManager
    abstract member snapshots : ISnapshotManager
    abstract member turns : ITurnManager
    abstract member users : IUserManager