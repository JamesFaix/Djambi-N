namespace Djambi.Api.Web.Controllers

open Djambi.Api.Common
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Logic.Managers
open Djambi.Api.Model
open Djambi.Api.Web.Interfaces
open Djambi.Api.Web

type GameController(u : HttpUtility) =
    interface IGameController with
        member x.getGames =
            let func ctx =
                u.getSessionAndModelFromContext<GamesQuery> ctx
                |> thenBindAsync (fun (jsonModel, session) -> GameManager.getGames jsonModel session)
            u.handle func

        member x.getGame gameId =
            let func ctx =
                u.getSessionFromContext ctx
                |> thenBindAsync (GameManager.getGame gameId)
            u.handle func

        member x.createGame =
            let func ctx =
                u.getSessionAndModelFromContext<GameParameters> ctx
                |> thenBindAsync (fun (request, session) -> GameManager.createGame request session)
            u.handle func
    
        member x.updateGameParameters gameId =
            let func ctx =
                u.getSessionAndModelFromContext<GameParameters> ctx
                |> thenBindAsync (fun (request, session) -> GameManager.updateGameParameters gameId request session)
            u.handle func

        member x.startGame gameId =
            let func ctx =
                u.getSessionFromContext ctx
                |> thenBindAsync (GameManager.startGame gameId)
            u.handle func

    interface IPlayerController with
        member x.addPlayer gameId =
            let func ctx =
                u.getSessionAndModelFromContext<CreatePlayerRequest> ctx
                |> thenBindAsync (fun (request, session) -> GameManager.addPlayer gameId request session)
            u.handle func

        member x.removePlayer (gameId, playerId) =
            let func ctx =
                u.getSessionFromContext ctx
                |> thenBindAsync (GameManager.removePlayer(gameId, playerId))
            u.handle func

        member x.updatePlayerStatus (gameId, playerId, statusName) =
            let func ctx =
                Enum.parseUnion<PlayerStatus> statusName
                |> Result.bindAsync (fun status -> 
                    u.getSessionFromContext ctx
                    |> thenBindAsync (GameManager.updatePlayerStatus (gameId, playerId, status))
                )
            u.handle func

    interface ITurnController with
        member x.selectCell (gameId, cellId) =
            let func ctx =
                u.getSessionFromContext ctx
                |> thenBindAsync (GameManager.selectCell(gameId, cellId))
            u.handle func

        member x.resetTurn gameId =
            let func ctx =
                u.getSessionFromContext ctx
                |> thenBindAsync (GameManager.resetTurn gameId)
            u.handle func

        member x.commitTurn gameId =
            let func ctx =
                u.getSessionFromContext ctx
                |> thenBindAsync (GameManager.commitTurn gameId)
            u.handle func

    interface IEventController with
        member x.getEvents gameId =
            let func ctx =
                u.getSessionAndModelFromContext<EventsQuery> ctx
                |> thenBindAsync (fun (query, session) -> 
                    GameManager.getEvents (gameId, query) session
                )
            u.handle func