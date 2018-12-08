module Djambi.Api.Web.Controllers.GameController

open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Web.HttpUtility
open Djambi.Api.Web.Managers

let getGameState(gameId : int) =
    let func ctx =
        getSessionFromContext ctx
        |> thenBindAsync (GameManager.getGameState gameId)
    handle func

let selectCell(gameId : int, cellId : int) =
    let func ctx =
        getSessionFromContext ctx
        |> thenBindAsync (GameManager.selectCell(gameId, cellId))
    handle func

let resetTurn(gameId : int) =
    let func ctx =
        getSessionFromContext ctx
        |> thenBindAsync (GameManager.resetTurn gameId)
    handle func

let commitTurn(gameId : int) =
    let func ctx =
        getSessionFromContext ctx
        |> thenBindAsync (GameManager.commitTurn gameId)
    handle func