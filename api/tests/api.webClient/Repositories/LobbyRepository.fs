module Djambi.Api.WebClient.LobbyRepository

open Djambi.Api.Web.Model.LobbyWebModel
open Djambi.Api.WebClient.Model
open Djambi.Api.WebClient.WebUtility

let createGame (request : CreateGameJsonModel, token : string) : LobbyGameJsonModel AsyncResponse =
    sendRequest(POST, "/games", 
        Some request,
        Some token)

let deleteGame (gameId : int, token : string) : Unit AsyncResponse =
    sendRequest(DELETE, sprintf "/games/%i" gameId, 
        None,
        Some token)
        
let getOpenGames (token : string) : LobbyGameJsonModel list AsyncResponse =
    sendRequest(GET, "/games/open", 
        None,
        Some token)
        
let getGames (token : string) : LobbyGameJsonModel list AsyncResponse =
    sendRequest(GET, "/games", 
        None,
        Some token)

let getUserGames (userId : int, token : string) : LobbyGameJsonModel list AsyncResponse =
    sendRequest(GET, sprintf "/users/%i/games" userId, 
        None,
        Some token)

let addPlayerToGame (gameId : int, userId : int, token : string) : Unit AsyncResponse =
    sendRequest(POST, sprintf "/games/%i/users/%i" gameId userId, 
        None,
        Some token)

let removePlayerFromGame (gameId : int, userId : int, token : string) : Unit AsyncResponse =
    sendRequest(DELETE, sprintf "/games/%i/users/%i" gameId userId, 
        None,
        Some token)