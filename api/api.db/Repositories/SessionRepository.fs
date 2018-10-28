module Djambi.Api.Db.Repositories.SessionRepository

open System
open Dapper
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Model.SessionDbModel
open Djambi.Api.Db.SqlUtility
open Djambi.Api.Model.SessionModel

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

let createSession(request : CreateSessionRequest) : Session AsyncHttpResult =
    let param = DynamicParameters()
                    .add("UserId", request.userId)
                    .add("Token", request.token)
                    .add("ExpiresOn", request.expiresOn)

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