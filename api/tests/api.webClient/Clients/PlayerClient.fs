module Djambi.Api.WebClient.PlayerClient

open Djambi.Api.WebClient.Model
open Djambi.Api.WebClient.WebUtility
open Djambi.Api.Model.PlayerModel

let addPlayer (lobbyId : int, request : CreatePlayerRequest, token : string) : Unit AsyncResponse =
    sendRequest(POST, sprintf "/lobbies/%i/players" lobbyId, 
        Some request,
        Some token)

let removePlayer (lobbyId : int, playerId : int, token : string) : Unit AsyncResponse =
    sendRequest(DELETE, sprintf "/lobbies/%i/players/%i" lobbyId playerId, 
        None,
        Some token)