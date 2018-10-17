module Djambi.Api.Web.Controllers.PlayerController

open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Logic.Services
open Djambi.Api.Web.HttpUtility
open Djambi.Api.Web.Mappings.PlayerWebMapping
open Djambi.Api.Web.Model.PlayerWebModel
 
let addPlayerToLobby(lobbyId : int) =
    //Error if not logged in
    //Must be either
        //Admin
        //User adding self
        //User adding guest of self
    //Must fail if adding guest and guests not allowed in lobby
    let func ctx =
        getSessionAndModelFromContext<CreatePlayerJsonModel> ctx
        |> thenBindAsync (fun (requestJsonModel, session) -> 
            let request = mapCreatePlayerRequest(requestJsonModel, lobbyId)
            PlayerService.addPlayerToLobby(request, session)
        )
    handle func

let removePlayerFromLobby(lobbyId : int, playerId : int) =
    //Error if not logged in
    //Must be either
        //Admin
        //User removing self
        //User removing guest of self
    //Removing a user must remove all guests
    let func ctx =
        getSessionFromContext ctx
        |> thenBindAsync (fun session -> 
            PlayerService.removePlayerFromLobby(playerId, session)
        )
    handle func