module Djambi.Api.WebClient.GameClient

open Djambi.Api.WebClient.Model
open Djambi.Api.WebClient.WebUtility
open Djambi.Api.Model

let createGame (parameters : GameParameters, token : string) : Game AsyncResponse =
    sendRequest(POST, "/games", 
        Some parameters,
        Some token)

let deleteGame (gameId : int, token : string) : Unit AsyncResponse =
    sendRequest(DELETE, sprintf "/games/%i" gameId, 
        None,
        Some token)
        
let getGames (query : GamesQuery, token : string) : Game list AsyncResponse =
    sendRequest(POST, "/games/query", 
        Some query,
        Some token)

let addPlayer (gameId : int, request : CreatePlayerRequest, token : string) : Unit AsyncResponse =
    sendRequest(POST, sprintf "/games/%i/players" gameId, 
        Some request,
        Some token)

let removePlayer (gameId : int, playerId : int, token : string) : Unit AsyncResponse =
    sendRequest(DELETE, sprintf "/games/%i/players/%i" gameId playerId, 
        None,
        Some token)