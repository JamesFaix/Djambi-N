module Djambi.Api.Logic.Managers.SessionManager

open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Logic.Services
open Djambi.Api.Model
open Djambi.ClientGenerator.Annotations

[<ClientFunction(HttpMethod.Post, "/sessions")>]
let login (request : LoginRequest) : Session AsyncHttpResult =
    SessionService.openSession request
    |> thenReplaceError 409 (HttpException(409, "Already signed in."))

[<ClientFunction(HttpMethod.Delete, "/sessions")>]
let logout (session : Session) : Unit AsyncHttpResult =
    SessionService.closeSession session
    |> thenMap ignore