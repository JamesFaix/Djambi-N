namespace Djambi.Api.Web.Controllers

open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Logic.Managers
open Djambi.Api.Web.Interfaces
open Djambi.Api.Web

type BoardController(u : HttpUtility) =
    interface IBoardController with

        member x.getBoard regionCount =
            let func ctx =
                u.getSessionFromContext ctx
                |> thenBindAsync (BoardManager.getBoard regionCount)
            u.handle func
            
        member x.getCellPaths (regionCount, cellId) =
            let func ctx = 
                u.getSessionFromContext ctx
                |> thenBindAsync (BoardManager.getCellPaths (regionCount, cellId))
            u.handle func
