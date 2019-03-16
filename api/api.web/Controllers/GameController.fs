namespace Djambi.Api.Web.Controllers

open Djambi.Api.Common
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Web.HttpUtility
open Djambi.Api.Logic.Managers
open Djambi.Api.Model
open Djambi.Api.Web.Interfaces

type GameController() =
    interface IGameController with
        member x.getGames =
            let func ctx =
                getSessionAndModelFromContext<GamesQuery> ctx
                |> thenBindAsync (fun (jsonModel, session) -> GameManager.getGames jsonModel session)
            handle func

        member x.getGame gameId =
            let func ctx =
                getSessionFromContext ctx
                |> thenBindAsync (GameManager.getGame gameId)
            handle func

        member x.createGame =
            let func ctx =
                getSessionAndModelFromContext<GameParameters> ctx
                |> thenBindAsync (fun (request, session) -> GameManager.createGame request session)
            handle func
    
        member x.updateGameParameters gameId =
            let func ctx =
                getSessionAndModelFromContext<GameParameters> ctx
                |> thenBindAsync (fun (request, session) -> GameManager.updateGameParameters gameId request session)
            handle func

        member x.startGame gameId =
            let func ctx =
                getSessionFromContext ctx
                |> thenBindAsync (GameManager.startGame gameId)
            handle func

    interface IPlayerController with
        member x.addPlayer gameId =
            let func ctx =
                getSessionAndModelFromContext<CreatePlayerRequest> ctx
                |> thenBindAsync (fun (request, session) -> GameManager.addPlayer gameId request session)
            handle func

        member x.removePlayer (gameId, playerId) =
            let func ctx =
                getSessionFromContext ctx
                |> thenBindAsync (GameManager.removePlayer(gameId, playerId))
            handle func

        member x.updatePlayerStatus (gameId, playerId, statusName) =
            let func ctx =
                Enum.parseUnion<PlayerStatus> statusName
                |> Result.bindAsync (fun status -> 
                    getSessionFromContext ctx
                    |> thenBindAsync (GameManager.updatePlayerStatus (gameId, playerId, status))
                )
            handle func

    interface ITurnController with
        member x.selectCell (gameId, cellId) =
            let func ctx =
                getSessionFromContext ctx
                |> thenBindAsync (GameManager.selectCell(gameId, cellId))
            handle func

        member x.resetTurn gameId =
            let func ctx =
                getSessionFromContext ctx
                |> thenBindAsync (GameManager.resetTurn gameId)
            handle func

        member x.commitTurn gameId =
            let func ctx =
                getSessionFromContext ctx
                |> thenBindAsync (GameManager.commitTurn gameId)
            handle func

    interface IEventController with
        member x.getEvents gameId =
            let func ctx =
                getSessionAndModelFromContext<EventsQuery> ctx
                |> thenBindAsync (fun (query, session) -> 
                    GameManager.getEvents (gameId, query) session
                )
            handle func