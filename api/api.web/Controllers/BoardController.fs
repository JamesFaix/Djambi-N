namespace Apex.Api.Web.Controllers

open Apex.Api.Common.Control.AsyncHttpResult
open Apex.Api.Web.Interfaces
open Apex.Api.Web
open Apex.Api.Logic.Interfaces

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
