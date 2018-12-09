module Djambi.Api.Web.Managers.SessionManager

open System
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Logic.Services
open Djambi.Api.Web.Mappings
open Djambi.Api.Web.Model
open Djambi.Api.Model

let openSession (jsonModel : LoginRequestJsonModel, appendCookie : string * DateTime -> Unit) : User AsyncHttpResult=
    let model = mapLoginRequestFromJson jsonModel
    SessionService.openSession model
    |> thenBindAsync (fun session ->
        appendCookie (session.token, session.expiresOn)
        UserService.getUser session.userId session
    )
    |> thenMap UserDetails.hideDetails
    |> thenReplaceError 409 (HttpException(409, "Already signed in."))

let closeSession (session : Session) : Unit AsyncHttpResult =
    SessionService.closeSession session
    |> thenMap ignore