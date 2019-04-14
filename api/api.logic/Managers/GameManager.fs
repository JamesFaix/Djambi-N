namespace Djambi.Api.Logic.Managers

open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Interfaces
open Djambi.Api.Logic
open Djambi.Api.Logic.Interfaces
open Djambi.Api.Logic.Services
open Djambi.Api.Model

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
        if isPublishable response.event
        then notificationServ.send response
             |> thenMap (fun _ -> response)
        else okTask response

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

    let processEventAsync (gameId : int) (getCreateEventRequest : Game -> CreateEventRequest AsyncHttpResult): StateAndEventResponse AsyncHttpResult =
        gameRepo.getGame gameId
        |> thenBindAsync (fun game ->
            getCreateEventRequest game
            |> thenBindAsync (fun eventRequest ->
                let newGame = eventServ.applyEvent game eventRequest
                eventRepo.persistEvent (eventRequest, game, newGame)
            )
        )
        |> thenBindAsync sendIfPublishable

    interface IEventManager with
        member x.getEvents (gameId, query) session =
            gameRepo.getGame gameId
            |> thenBind (Security.ensurePlayerOrHas ViewGames session)
            |> thenBindAsync (fun _ -> eventRepo.getEvents (gameId, query))

    interface IGameManager with
        member x.getGames query session =
            gameRepo.getGames query
            |> thenMap (fun games ->
                if session.user.has ViewGames
                then games
                else games |> List.filter (isGameViewableByActiveUser session)
            )

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
            processEventAsync gameId (fun game -> gameStartServ.getGameStartEvent game session)

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