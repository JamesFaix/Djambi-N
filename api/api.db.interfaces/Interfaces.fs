namespace Apex.Api.Db.Interfaces

open System
open System.Threading.Tasks
open Apex.Api.Common.Control
open Apex.Api.Model

type IEventRepository =
    abstract member getEvents : gameId:int * query:EventsQuery -> Task<list<Event>>
    abstract member persistEvent : request:CreateEventRequest * oldGame:Game * newGame:Game -> StateAndEventResponse AsyncHttpResult

type IGameRepository =
    abstract member getGame : gameId:int -> Game AsyncHttpResult
    [<Obsolete("Only used for tests")>]
    abstract member createGame : request:CreateGameRequest -> int AsyncHttpResult   
    [<Obsolete("Only used for tests")>]
    abstract member addPlayer : gameId:int * request:CreatePlayerRequest -> Player AsyncHttpResult
    [<Obsolete("Only used for tests")>]
    abstract member removePlayer : gameID:int * playerId:int -> unit AsyncHttpResult
    [<Obsolete("Only used for tests")>]
    abstract member updateGame : game:Game -> unit AsyncHttpResult
    abstract member getNeutralPlayerNames : unit -> string list AsyncHttpResult
    abstract member createGameAndAddPlayer : gameRequest:CreateGameRequest * playerRequest:CreatePlayerRequest -> int AsyncHttpResult

type ISearchRepository =
    abstract member searchGames : query:GamesQuery * currentUserId:int -> Task<list<SearchGame>>

type ISessionRepository =
    abstract member getSession : query:SessionQuery -> Task<Session>
    abstract member createSession : request:CreateSessionRequest -> Task<Session>
    abstract member renewSessionExpiration : sessionId:int * expiresOn:DateTime -> Task<Session>
    abstract member deleteSession : sessionId:int option * token:string option -> Task<unit>

type ISnapshotRepository =
    abstract member getSnapshot : snapshotId:int -> Task<Snapshot>
    abstract member getSnapshotsForGame : gameId:int -> Task<list<SnapshotInfo>>
    abstract member deleteSnapshot : snapshotId:int -> Task<unit>
    abstract member createSnapshot : request:InternalCreateSnapshotRequest -> Task<int>
    abstract member loadSnapshot : gameId:int * snapshotId:int -> Task<unit>

type IUserRepository =
    abstract member getUser : userId:int -> Task<UserDetails>
    abstract member getUserByName : name:string -> Task<UserDetails>
    abstract member createUser : request:CreateUserRequest -> Task<UserDetails>
    abstract member deleteUser : id:int -> Task<unit>
    abstract member updateFailedLoginAttempts : request:UpdateFailedLoginsRequest -> Task<unit>
