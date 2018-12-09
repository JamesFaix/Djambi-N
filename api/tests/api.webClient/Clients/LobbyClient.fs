module Djambi.Api.WebClient.LobbyClient

open Djambi.Api.WebClient.Model
open Djambi.Api.WebClient.WebUtility
open Djambi.Api.Model.LobbyModel

let createLobby (request : CreateLobbyRequest, token : string) : Lobby AsyncResponse =
    sendRequest(POST, "/lobbies", 
        Some request,
        Some token)

let deleteLobby (lobbyId : int, token : string) : Unit AsyncResponse =
    sendRequest(DELETE, sprintf "/lobbies/%i" lobbyId, 
        None,
        Some token)
        
let getLobbies (query : LobbiesQuery, token : string) : Lobby list AsyncResponse =
    sendRequest(POST, "/lobbies/query", 
        Some query,
        Some token)