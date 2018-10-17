module Djambi.Api.Web.Controllers.GameController

open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Logic.Services
open Djambi.Api.Web.HttpUtility
open Djambi.Api.Web.Mappings.GameWebMapping

let getGameState(gameId : int) =
    let func ctx =
        GameService.getGameState(gameId)
        |> thenMap mapGameStateToJsonModel
    handle func

let selectCell(gameId : int, cellId : int) =
    let func ctx = 
        GameService.selectCell(gameId, cellId)
        |> thenMap mapTurnStateToJsonModel
    handle func

let resetTurn(gameId : int) =
    let func ctx =
        GameService.resetTurn gameId
        |> thenMap mapTurnStateToJsonModel
    handle func

let commitTurn(gameId : int) =
    let func ctx =
        GameService.commitTurn gameId
        |> thenMap mapCommitTurnResponseToJsonModel
    handle func