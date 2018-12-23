module Djambi.Api.Logic.Managers.SessionManager

open System
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Logic.Services
open Djambi.Api.Model

let openSession (request : LoginRequest, appendCookie : string * DateTime -> Unit) : User AsyncHttpResult=
    SessionService.openSession request
    |> thenBindAsync (fun session ->
        appendCookie (session.token, session.expiresOn)
        UserService.getUser session.userId session
    )
    |> thenMap UserDetails.hideDetails
    |> thenReplaceError 409 (HttpException(409, "Already signed in."))

let closeSession (session : Session) : Unit AsyncHttpResult =
    SessionService.closeSession session
    |> thenMap ignore