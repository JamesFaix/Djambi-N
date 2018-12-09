module Djambi.Api.Web.Controllers.PlayerController

open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Web.HttpUtility
open Djambi.Api.Web.Managers
open Djambi.Api.Model.PlayerModel

let addPlayerToLobby(lobbyId : int) =
    let func ctx =
        getSessionAndModelFromContext<CreatePlayerRequest> ctx
        |> thenBindAsync (fun (request, session) -> PlayerManager.addPlayerToLobby(request, lobbyId) session)
    handle func

let removePlayerFromLobby(lobbyId : int, playerId : int) =
    let func ctx =
        getSessionFromContext ctx
        |> thenBindAsync (PlayerManager.removePlayerFromLobby(lobbyId, playerId))
    handle func