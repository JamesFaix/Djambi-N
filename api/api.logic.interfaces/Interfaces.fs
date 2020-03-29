namespace Apex.Api.Logic.Interfaces

open System
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
    abstract member openSession : request:LoginRequest -> Session AsyncHttpResult
    abstract member renewSession : token:string -> Session AsyncHttpResult
    abstract member getSession : token:string -> Session AsyncHttpResult
    abstract member closeSession : session:Session -> Unit AsyncHttpResult

type IBoardManager =
        abstract member getBoard : regionCount:int -> session:Session -> Board AsyncHttpResult

        abstract member getCellPaths : regionCount:int * cellId:int -> session:Session -> int list list AsyncHttpResult

type IEventManager =
        abstract member getEvents : gameId:int * query:EventsQuery -> session:Session -> Event list AsyncHttpResult

type IGameManager =
        abstract member getGame : gameId:int -> session:Session -> Game AsyncHttpResult

        abstract member createGame : parameters:GameParameters -> session:Session -> Game AsyncHttpResult

        abstract member updateGameParameters : gameId:int -> parameters:GameParameters -> session:Session -> StateAndEventResponse AsyncHttpResult

        abstract member startGame : gameId:int -> session:Session -> StateAndEventResponse AsyncHttpResult

type ISearchManager =
        abstract member searchGames : query:GamesQuery -> session:Session -> SearchGame list AsyncHttpResult

type IPlayerManager =
        abstract member addPlayer : gameId:int -> request:CreatePlayerRequest -> session:Session -> StateAndEventResponse AsyncHttpResult

        abstract member removePlayer : gameId:int * playerId:int -> session:Session -> StateAndEventResponse AsyncHttpResult

        abstract member updatePlayerStatus : gameId:int * playerId:int * status:PlayerStatus -> session:Session -> StateAndEventResponse AsyncHttpResult

type ISessionManager =
        abstract member login : request:LoginRequest -> Session AsyncHttpResult

        abstract member logout : session:Session -> unit AsyncHttpResult

type ISnapshotManager =
        abstract member createSnapshot : gameId:int -> request:CreateSnapshotRequest -> session:Session -> SnapshotInfo AsyncHttpResult

        abstract member getSnapshotsForGame : gameId:int -> session:Session -> SnapshotInfo list AsyncHttpResult

        abstract member deleteSnapshot : gameId:int -> snapshotId:int -> session:Session -> unit AsyncHttpResult

        abstract member loadSnapshot : gameId:int -> snapshotId:int -> session:Session -> unit AsyncHttpResult

type ITurnManager =
        abstract member selectCell : gameId:int * cellId:int -> session:Session -> StateAndEventResponse AsyncHttpResult

        abstract member resetTurn : gameId:int -> session:Session -> StateAndEventResponse AsyncHttpResult

        abstract member commitTurn : gameId:int -> session:Session -> StateAndEventResponse AsyncHttpResult

type IUserManager =
        abstract member createUser : request:CreateUserRequest -> sessionOption:Session option -> User AsyncHttpResult

        abstract member deleteUser : userId:int -> session:Session -> unit AsyncHttpResult

        abstract member getUser : userId:int -> session:Session -> User AsyncHttpResult

        abstract member getCurrentUser : session:Session -> User AsyncHttpResult
