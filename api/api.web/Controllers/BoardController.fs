namespace Djambi.Api.Web.Controllers

open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Web.HttpUtility
open Djambi.Api.Logic.Managers
open Djambi.Api.Web.Interfaces

type BoardController() =
    interface IBoardController with

        member x.getBoard regionCount =
            let func ctx =
                getSessionFromContext ctx
                |> thenBindAsync (BoardManager.getBoard regionCount)
            handle func
            
        member x.getCellPaths (regionCount, cellId) =
            let func ctx = 
                getSessionFromContext ctx
                |> thenBindAsync (BoardManager.getCellPaths (regionCount, cellId))
            handle func
