namespace Djambi.Api.Web.Controllers

open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Web.Interfaces
open Djambi.Api.Web
open Djambi.Api.Logic.Interfaces

type BoardController(boardMan : IBoardManager,
                     u : HttpUtility) =
    interface IBoardController with

        member x.getBoard regionCount =
            let func ctx =
                u.getSessionFromContext ctx
                |> thenBindAsync (boardMan.getBoard regionCount)
            u.handle func

        member x.getCellPaths (regionCount, cellId) =
            let func ctx =
                u.getSessionFromContext ctx
                |> thenBindAsync (boardMan.getCellPaths (regionCount, cellId))
            u.handle func
