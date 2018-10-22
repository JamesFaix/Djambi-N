module Djambi.Api.Web.Controllers.BoardController

open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Logic.Services
open Djambi.Api.Web.HttpUtility

let getBoard(regionCount : int) =
    let func ctx =
        getSessionFromContext ctx
        |> thenMap (BoardService.getBoard regionCount)
    handle func
            
let getCellPaths(regionCount : int, cellId : int) =
    let func ctx = 
        getSessionFromContext ctx
        |> thenMap (BoardService.getCellPaths(regionCount, cellId))
    handle func
