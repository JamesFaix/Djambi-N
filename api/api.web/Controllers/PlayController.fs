module Djambi.Api.Web.Controllers.PlayController

open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Logic.Services
open Djambi.Api.Web.HttpUtility
open Djambi.Api.Web.Mappings.PlayWebMapping

let getGameState(gameId : int) =
    let func ctx =
        PlayService.getGameState(gameId)
        |> thenMap mapGameStateToJsonModel
    handle func

let selectCell(gameId : int, cellId : int) =
    let func ctx = 
        PlayService.selectCell(gameId, cellId)
        |> thenMap mapTurnStateToJsonModel
    handle func

let resetTurn(gameId : int) =
    let func ctx =
        PlayService.resetTurn gameId
        |> thenMap mapTurnStateToJsonModel
    handle func

let commitTurn(gameId : int) =
    let func ctx =
        PlayService.commitTurn gameId
        |> thenMap mapCommitTurnResponseToJsonModel
    handle func