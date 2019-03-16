namespace Djambi.Api.Web.Controllers

open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Logic.Managers
open Djambi.Api.Web.Interfaces
open Djambi.Api.Web

type TurnController(u : HttpUtility) =
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