namespace Apex.Api.Db.Interfaces

open System
open Apex.Api.Common.Control
open Apex.Api.Model

type IEventRepository =
    abstract member getEvents : gameId:int * query:EventsQuery -> Event list AsyncHttpResult
    abstract member persistEvent : request:CreateEventRequest * oldGame:Game * newGame:Game -> StateAndEventResponse AsyncHttpResult

type IGameRepository =
    abstract member getGame : gameId:int -> Game AsyncHttpResult
    [<Obsolete("Only used for tests")>]
    abstract member createGame : request:CreateGameRequest -> int AsyncHttpResult   
    [<Obsolete("Only used for tests")>]
    abstract member addPlayer : gameId:int * request:CreatePlayerRequest -> Player AsyncHttpResult
    [<Obsolete("Only used for tests")>]
    abstract member removePlayer : gameID:int * playerId:int -> unit AsyncHttpResult
    abstract member getNeutralPlayerNames : unit -> string list AsyncHttpResult
    abstract member createGameAndAddPlayer : gameRequest:CreateGameRequest * playerRequest:CreatePlayerRequest -> int AsyncHttpResult

type ISearchRepository =
    abstract member searchGames : query:GamesQuery * currentUserId:int -> SearchGame list AsyncHttpResult

type ISessionRepository =
    abstract member getSession : query:SessionQuery -> Session AsyncHttpResult
    abstract member createSession : request:CreateSessionRequest -> Session AsyncHttpResult
    abstract member renewSessionExpiration : sessionId:int * expiresOn:DateTime -> Session AsyncHttpResult
    abstract member deleteSession : sessionId:int option * token:string option -> unit AsyncHttpResult

type ISnapshotRepository =
    abstract member getSnapshot : snapshotId:int -> Snapshot AsyncHttpResult
    abstract member getSnapshotsForGame : gameId:int -> SnapshotInfo list AsyncHttpResult
    abstract member deleteSnapshot : snapshotId:int -> unit AsyncHttpResult
    abstract member createSnapshot : request:InternalCreateSnapshotRequest -> int AsyncHttpResult
    abstract member loadSnapshot : gameId:int * snapshotId:int -> unit AsyncHttpResult

type IUserRepository =
    abstract member getUser : userId:int -> UserDetails AsyncHttpResult
    abstract member getUserByName : name:string -> UserDetails AsyncHttpResult
    abstract member createUser : request:CreateUserRequest -> UserDetails AsyncHttpResult
    abstract member deleteUser : id:int -> unit AsyncHttpResult
    abstract member updateFailedLoginAttempts : request:UpdateFailedLoginsRequest -> unit AsyncHttpResult
