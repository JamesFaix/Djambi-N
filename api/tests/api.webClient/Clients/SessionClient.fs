module Apex.Api.WebClient.SessionClient

open Apex.Api.Model
open Apex.Api.WebClient.Model
open Apex.Api.WebClient.WebUtility

let createSession (request : LoginRequest) : Unit AsyncResponse =
    sendRequest(POST, "/sessions",
        Some request,
        None)

let tryToCreateSessionWithToken (request : LoginRequest, token : string) : Unit AsyncResponse =
    sendRequest(POST, "/sessions",
        Some request,
        Some token)

let closeSession (token : string) : Unit AsyncResponse =
    sendRequest(DELETE, "/sessions",
        None,
        Some token)