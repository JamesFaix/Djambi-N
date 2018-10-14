module Djambi.Api.WebClient.SessionRepository

open Djambi.Api.Web.Model.LobbyWebModel
open Djambi.Api.WebClient.Model
open Djambi.Api.WebClient.WebUtility

let createSession (request : LoginRequestJsonModel) : SessionResponseJsonModel AsyncResponse =
    sendRequest(POST, "/sessions", 
        Some request,
        None)

let tryToCreateSessionWithToken (request : LoginRequestJsonModel, token : string) : SessionResponseJsonModel AsyncResponse =
    sendRequest(POST, "/sessions",
        Some request,
        Some token)

let closeSession (token : string) : Unit AsyncResponse =
    sendRequest(DELETE, "/sessions",
        None,
        Some token)

let addUserToSession (request : LoginRequestJsonModel, token : string) : SessionResponseJsonModel AsyncResponse =
    sendRequest(POST, "/sessions/users",
        Some request,
        Some token)

let removeUserFromSession (userId : int, token : string) : SessionResponseJsonModel AsyncResponse =
    sendRequest(DELETE, sprintf "/sessions/users/%i" userId,
        None,
        Some token)