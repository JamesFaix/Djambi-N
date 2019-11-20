namespace Apex.Api.Web.Controllers

open Apex.Api.Common.Control.AsyncHttpResult
open Apex.Api.Web.Interfaces
open Apex.Api.Web
open Apex.Api.Logic.Interfaces

type TurnController(u : HttpUtility,
                    turnMan : ITurnManager) =
    interface ITurnController with
        member x.selectCell (gameId, cellId) =
            let func ctx =
                u.getSessionFromContext ctx
                |> thenBindAsync (turnMan.selectCell(gameId, cellId))
            u.handle func

        member x.resetTurn gameId =
            let func ctx =
                u.getSessionFromContext ctx
                |> thenBindAsync (turnMan.resetTurn gameId)
            u.handle func

        member x.commitTurn gameId =
            let func ctx =
                u.getSessionFromContext ctx
                |> thenBindAsync (turnMan.commitTurn gameId)
            u.handle func