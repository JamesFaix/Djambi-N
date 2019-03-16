namespace Djambi.Api.Logic.Managers

open Djambi.Api.Logic.Services
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Model
open Djambi.Api.Db.Repositories
open Djambi.Api.Logic.Interfaces
open Djambi.Api.Logic

type GameManager(eventServ : EventService,
                 gameCrudServ : GameCrudService,
                 gameStartServ : GameStartService,
                 playerServ : PlayerService,
                 playerStatusChangeServ : PlayerStatusChangeService,
                 selectionServ : SelectionService,
                 turnServ : TurnService) =

    let isGameViewableByActiveUser (session : Session) (game : Game) : bool =
        let self = session.user
        game.parameters.isPublic
        || game.createdByUserId = self.id
        || game.players |> List.exists(fun p -> p.userId = Some self.id)
        
    let processEvent (gameId : int) (getCreateEventRequest : Game -> CreateEventRequest HttpResult) : StateAndEventResponse AsyncHttpResult = 
        GameRepository.getGame gameId
        |> thenBindAsync (fun game -> 
            getCreateEventRequest game
            |> Result.bindAsync (fun eventRequest -> 
                let newGame = eventServ.applyEvent game eventRequest
                EventRepository.persistEvent (eventRequest, game, newGame)
            )
        )
    
    let processEventAsync (gameId : int) (getCreateEventRequest : Game -> CreateEventRequest AsyncHttpResult): StateAndEventResponse AsyncHttpResult = 
        GameRepository.getGame gameId
        |> thenBindAsync (fun game -> 
            getCreateEventRequest game
            |> thenBindAsync (fun eventRequest -> 
                let newGame = eventServ.applyEvent game eventRequest
                EventRepository.persistEvent (eventRequest, game, newGame)
            )
        )

    interface IEventManager with    
        member x.getEvents (gameId, query) session =
            GameRepository.getGame gameId
            |> thenBind (Security.ensurePlayerOrHas ViewGames session)
            |> thenBindAsync (fun _ -> EventRepository.getEvents (gameId, query))

    interface IGameManager with
        member x.getGames query session =
            GameRepository.getGames query
            |> thenMap (fun games ->
                if session.user.has ViewGames
                then games
                else games |> List.filter (isGameViewableByActiveUser session)
            )

        //TODO: Requires integration tests
        member x.getGame gameId session =
            GameRepository.getGame gameId
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