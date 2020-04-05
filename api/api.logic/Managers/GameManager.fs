namespace Apex.Api.Logic.Managers

open System
open Apex.Api.Common.Control
open Apex.Api.Common.Control.AsyncHttpResult
open Apex.Api.Db.Interfaces
open Apex.Api.Logic
open Apex.Api.Logic.Interfaces
open Apex.Api.Logic.Services
open Apex.Api.Model
open Apex.Api.Enums
open FSharp.Control.Tasks

type GameManager(eventRepo : IEventRepository,
                 eventServ : EventService,
                 gameCrudServ : GameCrudService,
                 gameRepo : IGameRepository,
                 gameStartServ : GameStartService,
                 notificationServ : INotificationService,
                 playerServ : PlayerService,
                 playerStatusChangeServ : PlayerStatusChangeService,
                 selectionServ : SelectionService,
                 turnServ : TurnService) =

    let isGameViewableByActiveUser (session : Session) (game : Game) : bool =
        let self = session.user
        game.parameters.isPublic
        || game.createdBy.userId = self.id
        || game.players |> List.exists(fun p -> p.userId = Some self.id)

    let isPublishable (event : Event) : bool =
        match event.kind with
        | EventKind.GameCanceled
        | EventKind.GameParametersChanged
        | EventKind.GameStarted
        | EventKind.PlayerJoined
        | EventKind.PlayerRemoved
        | EventKind.PlayerStatusChanged
        | EventKind.TurnCommitted
            -> true
        | _ -> false

    let sendIfPublishable (response : StateAndEventResponse) : StateAndEventResponse AsyncHttpResult =
        task {
            if isPublishable response.event
            then
                let! _ = notificationServ.send response
                return Ok response
            else return Ok response
        }

    let processEvent (gameId : int) (getCreateEventRequest : Game -> CreateEventRequest HttpResult) : StateAndEventResponse AsyncHttpResult =
        gameRepo.getGame gameId
        |> thenBindAsync (fun game ->
            getCreateEventRequest game
            |> Result.bindAsync (fun eventRequest ->
                let newGame = eventServ.applyEvent game eventRequest
                eventRepo.persistEvent (eventRequest, game, newGame)
            )
        )
        |> thenBindAsync sendIfPublishable

    let processGameStartEventsAsync (gameId : int) (getCreateEventRequests : Game -> (CreateEventRequest option * CreateEventRequest) AsyncHttpResult): StateAndEventResponse AsyncHttpResult =
        gameRepo.getGame gameId
        |> thenBindAsync (fun game ->
            getCreateEventRequests game
            |> thenBindAsync (fun eventRequests ->
                (*
                    This is really kind of ugly because of how inflexible the pattern for
                    handling SQL transactions while persisting game events is.
                    The problem is that neutral players must be persisted to get their IDs
                    before pieces are created, so that the pieces can be assigned owners.
                    The solution here puts player creation in a separate transaction before the 
                    changes to the game (pieces, status, etc) are made.
                *)
                let (addNeutralPlayers, startGame) = eventRequests;
                match addNeutralPlayers with
                | Some er -> 
                    let newGame = eventServ.applyEvent game er
                    eventRepo.persistEvent (er, game, newGame)
                | None ->
                    let dummyEvent : Event = {
                        id = 0
                        createdBy = {
                            userId = 0
                            userName = ""
                            time = DateTime.MinValue
                        }
                        actingPlayerId = None
                        kind = EventKind.PlayerJoined
                        effects = []
                    }
                    okTask { game = game; event = dummyEvent }
                |> thenBindAsync (fun resp -> 
                    let newGame = eventServ.applyEvent resp.game startGame
                    eventRepo.persistEvent (startGame, resp.game, newGame)                
                )
            )
        )
        |> thenBindAsync sendIfPublishable

    interface IEventManager with
        member x.getEvents (gameId, query) session =
            gameRepo.getGame gameId
            |> thenBind (Security.ensurePlayerOrHas Privilege.ViewGames session)
            |> thenBindAsync (fun _ -> eventRepo.getEvents (gameId, query))

    interface IGameManager with
        //TODO: Requires integration tests
        member x.getGame gameId session =
            gameRepo.getGame gameId
            |> thenBind (fun game ->
                if isGameViewableByActiveUser session game
                then Ok <| game
                else Error <| HttpException(404, "Game not found.")
            )

        member x.createGame parameters session =
            gameCrudServ.createGame parameters session

        //TODO: Requires integration tests
        member x.updateGameParameters gameId parameters session =
            processEvent gameId (fun game -> gameCrudServ.getUpdateGameParametersEvent (game, parameters) session)

        member x.startGame gameId session =
            processGameStartEventsAsync gameId (fun game -> gameStartServ.getGameStartEvents game session)

    interface IPlayerManager with
        member x.addPlayer gameId request session =
            processEvent gameId (fun game -> playerServ.getAddPlayerEvent (game, request) session)

        member x.removePlayer (gameId, playerId) session =
            processEvent gameId (fun game -> playerServ.getRemovePlayerEvent (game, playerId) session)

        member x.updatePlayerStatus (gameId, playerId, status) session =
            let request =
                {
                    gameId = gameId
                    playerId = playerId
                    status = status
                }
            processEvent request.gameId (fun game -> playerStatusChangeServ.getUpdatePlayerStatusEvent (game, request) session)

    interface ITurnManager with
        member x.selectCell (gameId, cellId) session =
            processEvent gameId (fun game -> selectionServ.getCellSelectedEvent (game, cellId) session)

        member x.resetTurn gameId session =
            processEvent gameId (fun game -> turnServ.getResetTurnEvent game session)

        member x.commitTurn gameId session =
            processEvent gameId (fun game -> turnServ.getCommitTurnEvent game session)