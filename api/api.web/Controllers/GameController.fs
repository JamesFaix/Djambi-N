namespace Djambi.Api.Web.Controllers

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