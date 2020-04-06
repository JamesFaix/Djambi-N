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
open System.Threading.Tasks

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

    let sendIfPublishable (response : StateAndEventResponse) : Task<unit> =
        task {
            if isPublishable response.event
            then return! notificationServ.send response
            else return ()
        }

    let processEvent (gameId : int) (getCreateEventRequest : Game -> CreateEventRequest) : Task<StateAndEventResponse> =
        task {
            let! game = gameRepo.getGame gameId
            let eventRequest = getCreateEventRequest game
            let newGame = eventServ.applyEvent game eventRequest
            let! response = eventRepo.persistEvent (eventRequest, game, newGame)
            let! _ = sendIfPublishable response
            return response
        }

    let processGameStartEventsAsync (gameId : int) (getCreateEventRequests : Game -> Task<(CreateEventRequest option * CreateEventRequest)>): Task<StateAndEventResponse> =
        task {
            let! game = gameRepo.getGame gameId
            let! eventRequests = getCreateEventRequests game
            (*
                This is really kind of ugly because of how inflexible the pattern for
                handling SQL transactions while persisting game events is.
                The problem is that neutral players must be persisted to get their IDs
                before pieces are created, so that the pieces can be assigned owners.
                The solution here puts player creation in a separate transaction before the 
                changes to the game (pieces, status, etc) are made.
            *)
            let (addNeutralPlayers, startGame) = eventRequests;
            let! response = 
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
                    Task.FromResult{ game = game; event = dummyEvent }
            
            let newGame = eventServ.applyEvent response.game startGame
            let! response = eventRepo.persistEvent (startGame, response.game, newGame)
            let! _ = sendIfPublishable response
            return response
        }

    interface IEventManager with
        member x.getEvents (gameId, query) session =
            task {
                let! game = gameRepo.getGame gameId
                match Security.ensurePlayerOrHas Privilege.ViewGames session game with
                | Error ex -> return raise ex
                | _ -> return! eventRepo.getEvents (gameId, query)            
            }

    interface IGameManager with
        //TODO: Requires integration tests
        member x.getGame gameId session =
            task {
                let! game = gameRepo.getGame gameId
                if isGameViewableByActiveUser session game
                then return game
                else return raise <| HttpException(404, "Game not found.")
            }

        member x.createGame parameters session =
            gameCrudServ.createGame parameters session

        //TODO: Requires integration tests
        member x.updateGameParameters gameId parameters session =
            processEvent gameId (fun game -> 
                gameCrudServ.getUpdateGameParametersEvent (game, parameters) session
            )

        member x.startGame gameId session =
            processGameStartEventsAsync gameId (fun game -> gameStartServ.getGameStartEvents game session |> thenExtract)

    interface IPlayerManager with
        member x.addPlayer gameId request session =
            processEvent gameId (fun game -> 
                match playerServ.getAddPlayerEvent (game, request) session with
                | Ok x -> x
                | Error ex -> raise ex
            )

        member x.removePlayer (gameId, playerId) session =
            processEvent gameId (fun game -> 
                match playerServ.getRemovePlayerEvent (game, playerId) session with
                | Ok x -> x
                | Error ex -> raise ex
            )

        member x.updatePlayerStatus (gameId, playerId, status) session =
            let request =
                {
                    gameId = gameId
                    playerId = playerId
                    status = status
                }
            processEvent request.gameId (fun game -> 
                match playerStatusChangeServ.getUpdatePlayerStatusEvent (game, request) session with
                | Ok x -> x
                | Error ex -> raise ex
            )

    interface ITurnManager with
        member x.selectCell (gameId, cellId) session =
            processEvent gameId (fun game -> 
                match selectionServ.getCellSelectedEvent (game, cellId) session with
                | Ok x -> x
                | Error ex -> raise ex
            )

        member x.resetTurn gameId session =
            processEvent gameId (fun game -> 
                match turnServ.getResetTurnEvent game session with
                | Ok x -> x
                | Error ex -> raise ex
            )

        member x.commitTurn gameId session =
            processEvent gameId (fun game -> 
                match turnServ.getCommitTurnEvent game session with
                | Ok x -> x
                | Error ex -> raise ex                
            )