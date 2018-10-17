﻿module Djambi.Api.WebClient.SessionClient

open Djambi.Api.Web.Model.SessionWebModel
open Djambi.Api.WebClient.Model
open Djambi.Api.WebClient.WebUtility

let createSession (request : LoginRequestJsonModel) : Unit AsyncResponse =
    sendRequest(POST, "/sessions", 
        Some request,
        None)

let tryToCreateSessionWithToken (request : LoginRequestJsonModel, token : string) : Unit AsyncResponse =
    sendRequest(POST, "/sessions",
        Some request,
        Some token)

let closeSession (token : string) : Unit AsyncResponse =
    sendRequest(DELETE, "/sessions",
        None,
        Some token)