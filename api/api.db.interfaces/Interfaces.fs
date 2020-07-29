namespace Apex.Api.Db.Interfaces

open System
open System.Threading.Tasks
open Apex.Api.Model

type IEventRepository =
    abstract member getEvents : gameId:int * query:EventsQuery -> Task<list<Event>>
    abstract member persistEvent : request:CreateEventRequest * oldGame:Game * newGame:Game -> Task<StateAndEventResponse>

type IGameRepository =
    abstract member getGame : gameId:int -> Task<Game>
    [<Obsolete("Only used for tests")>]
    abstract member createGame : request:CreateGameRequest -> Task<int>   
    [<Obsolete("Only used for tests")>]
    abstract member updateGame : game:Game -> Task<unit>
    abstract member getNeutralPlayerNames : unit -> Task<list<string>>
    abstract member createGameAndAddPlayer : gameRequest:CreateGameRequest * playerRequest:CreatePlayerRequest -> Task<int>

type IPlayerRepository =
    abstract member addPlayer : gameId:int * player:Player -> Task<Player>
    abstract member removePlayer : gameId:int * playerId:int -> Task<unit>
    abstract member updatePlayer : gameId:int * player:Player -> Task<unit>

type ISearchRepository =
    abstract member searchGames : query:GamesQuery * currentUserId:int -> Task<list<SearchGame>>

type ISessionRepository =
    abstract member getSession : query:SessionQuery -> Task<Option<Session>>
    abstract member createSession : request:CreateSessionRequest -> Task<Session>
    abstract member renewSessionExpiration : sessionId:int * expiresOn:DateTime -> Task<Session>
    abstract member deleteSession : token:string -> Task<unit>

type ISnapshotRepository =
    abstract member getSnapshot : snapshotId:int -> Task<Snapshot>
    abstract member getSnapshotsForGame : gameId:int -> Task<list<SnapshotInfo>>
    abstract member deleteSnapshot : snapshotId:int -> Task<unit>
    abstract member createSnapshot : request:InternalCreateSnapshotRequest -> Task<int>
    abstract member loadSnapshot : gameId:int * snapshotId:int -> Task<unit>

type IUserRepository =
    abstract member getUser : userId:int -> Task<Option<UserDetails>>
    abstract member getUserByName : name:string -> Task<Option<UserDetails>>
    abstract member createUser : request:CreateUserRequest -> Task<UserDetails>
    abstract member deleteUser : id:int -> Task<unit>
    abstract member updateFailedLoginAttempts : request:UpdateFailedLoginsRequest -> Task<unit>
