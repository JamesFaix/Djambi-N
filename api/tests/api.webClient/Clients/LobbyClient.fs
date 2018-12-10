module Djambi.Api.WebClient.LobbyClient

open Djambi.Api.WebClient.Model
open Djambi.Api.WebClient.WebUtility
open Djambi.Api.Model

let createLobby (request : CreateLobbyRequest, token : string) : GameParameters AsyncResponse =
    sendRequest(POST, "/lobbies", 
        Some request,
        Some token)

let deleteLobby (lobbyId : int, token : string) : Unit AsyncResponse =
    sendRequest(DELETE, sprintf "/lobbies/%i" lobbyId, 
        None,
        Some token)
        
let getLobbies (query : GamesQuery, token : string) : GameParameters list AsyncResponse =
    sendRequest(POST, "/lobbies/query", 
        Some query,
        Some token)