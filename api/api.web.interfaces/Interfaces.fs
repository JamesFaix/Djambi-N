namespace Apex.Api.Web.Interfaces

open Giraffe

type IBoardController =
    abstract member getBoard : regionCount:int -> HttpHandler
    abstract member getCellPaths : regionCount:int * cellId:int -> HttpHandler

type IEventController =
    abstract member getEvents : gameId:int -> HttpHandler

type IGameController =
    abstract member getGame : gameId:int -> HttpHandler
    abstract member createGame : HttpHandler
    abstract member updateGameParameters : gameId:int -> HttpHandler
    abstract member startGame : gameId:int -> HttpHandler

type ISearchController =
    abstract member searchGames : HttpHandler

type INotificationsController =
    abstract member connectWebSockets : HttpHandler
    abstract member connectSse : HttpHandler

type IPlayerController =
    abstract member addPlayer : gameId:int -> HttpHandler
    abstract member removePlayer : gameId:int * playerId:int -> HttpHandler
    abstract member updatePlayerStatus : gameId:int * playerId:int * statusName:string -> HttpHandler

type ISessionController =
    abstract member openSession : HttpHandler
    abstract member closeSession : HttpHandler

type ISnapshotController =
    abstract member createSnapshot : gameId:int -> HttpHandler
    abstract member getSnapshotsForGame : gameId:int -> HttpHandler
    abstract member deleteSnapshot : gameId:int * snapshotId:int -> HttpHandler
    abstract member loadSnapshot : gameId:int * snapshotId:int -> HttpHandler

type ITurnController =
    abstract member selectCell : gameId:int * cellId:int -> HttpHandler
    abstract member resetTurn : gameId:int -> HttpHandler
    abstract member commitTurn : gameId:int -> HttpHandler

type IUserController =
    abstract member createUser : HttpHandler
    abstract member deleteUser : userId:int -> HttpHandler
    abstract member getUser : userId:int -> HttpHandler
    abstract member getCurrentUser : HttpHandler
