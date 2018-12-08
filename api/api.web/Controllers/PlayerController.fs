module Djambi.Api.Web.Controllers.PlayerController

open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Web.HttpUtility
open Djambi.Api.Web.Model.PlayerWebModel
open Djambi.Api.Web.Managers

let addPlayerToLobby(lobbyId : int) =
    let func ctx =
        getSessionAndModelFromContext<CreatePlayerJsonModel> ctx
        |> thenBindAsync (fun (jsonModel, session) -> PlayerManager.addPlayerToLobby(jsonModel, lobbyId) session)
    handle func

let removePlayerFromLobby(lobbyId : int, playerId : int) =
    let func ctx =
        getSessionFromContext ctx
        |> thenBindAsync (PlayerManager.removePlayerFromLobby(lobbyId, playerId))
    handle func