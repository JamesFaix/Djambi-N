module Djambi.Api.Web.Controllers.PlayerController

open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Logic.Services
open Djambi.Api.Web.HttpUtility
open Djambi.Api.Web.Mappings.PlayerWebMapping
open Djambi.Api.Web.Model.PlayerWebModel
 
let addPlayerToLobby(lobbyId : int) =
    let func ctx =
        getSessionAndModelFromContext<CreatePlayerJsonModel> ctx
        |> thenBindAsync (fun (requestJsonModel, session) -> 
            let request = mapCreatePlayerRequest(requestJsonModel, lobbyId)
            PlayerService.addPlayerToLobby request session
        )
    handle func

let removePlayerFromLobby(lobbyId : int, playerId : int) =
    let func ctx =
        getSessionFromContext ctx
        |> thenBindAsync (PlayerService.removePlayerFromLobby(lobbyId, playerId))
    handle func