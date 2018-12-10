module Djambi.Api.Db.Repositories.SessionRepository

open System
open Dapper
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Mapping
open Djambi.Api.Db.Model
open Djambi.Api.Db.SqlUtility
open Djambi.Api.Model

let getSession(sessionId : int option,
               token : string option,
               userId : int option) : Session AsyncHttpResult =

    let param = DynamicParameters()
                    .addOption("SessionId", sessionId)
                    .addOption("Token", token)
                    .addOption("UserId", userId)

    let cmd = proc("Sessions_Get", param)

    querySingle<SessionSqlModel>(cmd, "Session")
    |> thenMap mapSessionResponse

let createSession(userId : int,
                  token : string,
                  expiresOn : DateTime) : Session AsyncHttpResult =
    let param = DynamicParameters()
                    .add("UserId", userId)
                    .add("Token", token)
                    .add("ExpiresOn", expiresOn)

    let cmd = proc("Sessions_Create", param)

    querySingle<int>(cmd, "Session")
    |> thenBindAsync(fun sessionId -> getSession(Some sessionId, None, None))

let renewSessionExpiration(sessionId : int, expiresOn : DateTime) : Session AsyncHttpResult =
    let param = DynamicParameters()
                    .add("SessionId", sessionId)
                    .add("ExpiresOn", expiresOn)

    let cmd = proc("Sessions_Renew", param)

    queryUnit(cmd, "Session")
    |> thenBindAsync (fun _ -> getSession(Some sessionId, None, None))

let deleteSession(sessionId : int option, token : string option) : Unit AsyncHttpResult =
    let param = DynamicParameters()
                    .addOption("SessionId", sessionId)
                    .addOption("Token", token)

    let cmd = proc("Sessions_Delete", param)

    queryUnit(cmd, "Session")