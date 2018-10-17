module Djambi.Api.WebClient.LobbyClient

open Djambi.Api.Web.Model.LobbyWebModel
open Djambi.Api.WebClient.Model
open Djambi.Api.WebClient.WebUtility

let createLobby (request : CreateLobbyJsonModel, token : string) : LobbyResponseJsonModel AsyncResponse =
    sendRequest(POST, "/lobbies", 
        Some request,
        Some token)

let deleteLobby (lobbyId : int, token : string) : Unit AsyncResponse =
    sendRequest(DELETE, sprintf "/lobbies/%i" lobbyId, 
        None,
        Some token)
        
let getLobbies (query : LobbiesQueryJsonModel, token : string) : LobbyResponseJsonModel list AsyncResponse =
    sendRequest(POST, "/lobbies/query", 
        Some query,
        Some token)