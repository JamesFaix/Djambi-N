namespace Apex.Api.Web.Controllers

open Apex.Api.Common.Control.AsyncHttpResult
open Apex.Api.Model
open Apex.Api.Web.Interfaces
open Apex.Api.Web
open Apex.Api.Logic.Interfaces

type GameController(gameMan : IGameManager,
                    u : HttpUtility) =
    interface IGameController with
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