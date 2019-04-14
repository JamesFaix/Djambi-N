namespace Djambi.Api.Web.Controllers

open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Model
open Djambi.Api.Web.Interfaces
open Djambi.Api.Web
open Djambi.Api.Logic.Interfaces

type GameController(gameMan : IGameManager,
                    u : HttpUtility) =
    interface IGameController with
        member x.getGames =
            let func ctx =
                u.getSessionAndModelFromContext<GamesQuery> ctx
                |> thenBindAsync (fun (jsonModel, session) -> gameMan.getGames jsonModel session)
            u.handle func

        member x.getGame gameId =
            let func ctx =
                u.getSessionFromContext ctx
                |> thenBindAsync (gameMan.getGame gameId)
            u.handle func

        member x.createGame =
            let func ctx =
                u.getSessionAndModelFromContext<GameParameters> ctx
                |> thenBindAsync (fun (request, session) -> gameMan.createGame request session)
            u.handle func

        member x.updateGameParameters gameId =
            let func ctx =
                u.getSessionAndModelFromContext<GameParameters> ctx
                |> thenBindAsync (fun (request, session) -> gameMan.updateGameParameters gameId request session)
            u.handle func

        member x.startGame gameId =
            let func ctx =
                u.getSessionFromContext ctx
                |> thenBindAsync (gameMan.startGame gameId)
            u.handle func