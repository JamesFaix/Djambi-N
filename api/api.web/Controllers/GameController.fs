module Djambi.Api.Web.Controllers.GameController

open Giraffe
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Web.HttpUtility
open Djambi.Api.Web.Managers
open Djambi.Api.Model

let getGames : HttpHandler =
    let func ctx =
        getSessionAndModelFromContext<GamesQuery> ctx
        |> thenBindAsync (fun (jsonModel, session) -> GameManager.getGames jsonModel session)
    handle func

let getGame(gameId : int) : HttpHandler =
    let func ctx =
        getSessionFromContext ctx
        |> thenBindAsync (GameManager.getGame gameId)
    handle func

let createGame : HttpHandler =
    let func ctx =
        getSessionAndModelFromContext<CreateGameRequest> ctx
        |> thenBindAsync (fun (request, session) -> GameManager.createGame request session)
    handle func

let deleteGame(gameId : int) =
    let func ctx =
        getSessionFromContext ctx
        |> thenBindAsync (GameManager.deleteGame gameId)
    handle func

let startGame(gameId: int) =
    let func ctx =
        getSessionFromContext ctx
        |> thenBindAsync (GameManager.startGame gameId)
    handle func

let addPlayer(gameId : int) =
    let func ctx =
        getSessionAndModelFromContext<CreatePlayerRequest> ctx
        |> thenBindAsync (fun (request, session) -> GameManager.addPlayer(request, gameId) session)
    handle func

let removePlayer(gameId : int, playerId : int) =
    let func ctx =
        getSessionFromContext ctx
        |> thenBindAsync (GameManager.removePlayer(gameId, playerId))
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