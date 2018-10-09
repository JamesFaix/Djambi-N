module Djambi.Api.WebClient.SessionRepository

open System.Threading.Tasks
open Djambi.Api.Web.Model.LobbyWebModel
open Djambi.Api.WebClient.Model
open Djambi.Api.WebClient.WebUtility

let createSession (request : LoginRequestJsonModel) : SessionResponseJsonModel Response Task =
    sendRequest(POST, "/sessions", 
        Some request,
        None)

let closeSession (token : string) : Unit Response Task =
    sendRequest(DELETE, "/sessions",
        None,
        Some token)

let addUserToSession (request : LoginRequestJsonModel, token : string) : SessionResponseJsonModel Response Task =
    sendRequest(POST, "/sessions/users",
        Some request,
        Some token)

let removeUserFromSession (userId : int, token : string) : SessionResponseJsonModel Response Task =
    sendRequest(DELETE, sprintf "/sessions/users/%i" userId,
        None,
        Some token)