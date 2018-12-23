module Djambi.Api.Web.Controllers.BoardController

open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Web.HttpUtility
open Djambi.Api.Logic.Managers

let getBoard(regionCount : int) =
    let func ctx =
        getSessionFromContext ctx
        |> thenBindAsync (BoardManager.getBoard regionCount)
    handle func
            
let getCellPaths(regionCount : int, cellId : int) =
    let func ctx = 
        getSessionFromContext ctx
        |> thenBindAsync (BoardManager.getCellPaths (regionCount, cellId))
    handle func
