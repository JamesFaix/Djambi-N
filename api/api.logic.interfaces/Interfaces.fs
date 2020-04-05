namespace Apex.Api.Logic.Interfaces

open System
open System.Threading.Tasks
open Apex.Api.Common.Control
open Apex.Api.Model
open Apex.Api.Enums

type ISubscriber =
    inherit IDisposable
    abstract member userId : int
    abstract member send : response:StateAndEventResponse -> unit AsyncHttpResult

type INotificationService =
    abstract member add : subscriber:ISubscriber -> unit
    abstract member remove : userId:int -> unit
    abstract member send : response:StateAndEventResponse -> unit AsyncHttpResult

type ISessionService =
    abstract member openSession : request:LoginRequest -> Task<Session>
    abstract member renewSession : token:string -> Task<Session>
    abstract member getSession : token:string -> Task<Session>
    abstract member closeSession : session:Session -> Task<unit>

type IBoardManager =
    abstract member getBoard : regionCount:int -> session:Session -> Task<Board>
    abstract member getCellPaths : regionCount:int * cellId:int -> session:Session -> Task<List<List<int>>>

type IEventManager =
    abstract member getEvents : gameId:int * query:EventsQuery -> session:Session -> Event list AsyncHttpResult

type IGameManager =
    abstract member getGame : gameId:int -> session:Session -> Game AsyncHttpResult
    abstract member createGame : parameters:GameParameters -> session:Session -> Game AsyncHttpResult
    abstract member updateGameParameters : gameId:int -> parameters:GameParameters -> session:Session -> StateAndEventResponse AsyncHttpResult
    abstract member startGame : gameId:int -> session:Session -> StateAndEventResponse AsyncHttpResult

type ISearchManager =
    abstract member searchGames : query:GamesQuery -> session:Session -> Task<list<SearchGame>>

type IPlayerManager =
    abstract member addPlayer : gameId:int -> request:CreatePlayerRequest -> session:Session -> StateAndEventResponse AsyncHttpResult
    abstract member removePlayer : gameId:int * playerId:int -> session:Session -> StateAndEventResponse AsyncHttpResult
    abstract member updatePlayerStatus : gameId:int * playerId:int * status:PlayerStatus -> session:Session -> StateAndEventResponse AsyncHttpResult

type ISessionManager =
    abstract member login : request:LoginRequest -> Task<Session>
    abstract member logout : session:Session -> Task<unit>

type ISnapshotManager =
    abstract member createSnapshot : gameId:int -> request:CreateSnapshotRequest -> session:Session -> Task<SnapshotInfo>
    abstract member getSnapshotsForGame : gameId:int -> session:Session -> Task<list<SnapshotInfo>>
    abstract member deleteSnapshot : gameId:int -> snapshotId:int -> session:Session -> Task<unit>
    abstract member loadSnapshot : gameId:int -> snapshotId:int -> session:Session -> Task<unit>

type ITurnManager =
    abstract member selectCell : gameId:int * cellId:int -> session:Session -> StateAndEventResponse AsyncHttpResult
    abstract member resetTurn : gameId:int -> session:Session -> StateAndEventResponse AsyncHttpResult
    abstract member commitTurn : gameId:int -> session:Session -> StateAndEventResponse AsyncHttpResult

type IUserManager =
    abstract member createUser : request:CreateUserRequest -> sessionOption:Session option -> Task<User>
    abstract member deleteUser : userId:int -> session:Session -> Task<unit>
    abstract member getUser : userId:int -> session:Session -> Task<User>
    abstract member getCurrentUser : session:Session -> Task<User>
