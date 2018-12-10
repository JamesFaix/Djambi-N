module Djambi.Api.Web.Controllers.LobbyController

open Giraffe
open Microsoft.AspNetCore.Http
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Web.HttpUtility
open Djambi.Api.Web.Managers
open Djambi.Api.Model

let getLobbies : HttpHandler =
    let func ctx =
        getSessionAndModelFromContext<LobbiesQuery> ctx
        |> thenBindAsync (fun (jsonModel, session) -> LobbyManager.getLobbies jsonModel session)
    handle func

let getLobby(lobbyId : int) : HttpHandler =
    let func ctx =
        getSessionFromContext ctx
        |> thenBindAsync (LobbyManager.getLobby lobbyId)
    handle func

let createLobby : HttpHandler =
    let func (ctx : HttpContext) : Lobby AsyncHttpResult =
        getSessionAndModelFromContext<CreateLobbyRequest> ctx
        |> thenBindAsync (fun (jsonModel, session) -> LobbyManager.createLobby jsonModel session)
    handle func

let deleteLobby(lobbyId : int) =
    let func ctx =
        getSessionFromContext ctx
        |> thenBindAsync (LobbyManager.deleteLobby lobbyId)
    handle func

let startGame(lobbyId: int) =
    let func ctx =
        getSessionFromContext ctx
        |> thenBindAsync (LobbyManager.startGame lobbyId)
    handle func
