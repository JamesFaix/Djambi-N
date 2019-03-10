module Djambi.Api.Db.Repositories.SessionRepository

open System
open Dapper
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Mapping
open Djambi.Api.Db.Model
open Djambi.Api.Db.SqlUtility
open Djambi.Api.Model

let getSession(query : SessionQuery) : Session AsyncHttpResult =

    let param = DynamicParameters()
                    .addOption("SessionId", query.sessionId)
                    .addOption("Token", query.token)
                    .addOption("UserId", query.userId)

    let cmd = proc("Sessions_Get", param)

    querySingle<SessionSqlModel>(cmd, "Session")
    |> thenBindAsync (fun sessionSqlModel -> 
        UserRepository.getUser sessionSqlModel.userId
        |> thenMap (fun userDetails ->
            let user = userDetails |> UserDetails.hideDetails
            mapSessionResponse sessionSqlModel user
        )
    )

let createSession(request : CreateSessionRequest) : Session AsyncHttpResult =
    let param = DynamicParameters()
                    .add("UserId", request.userId)
                    .add("Token", request.token)
                    .add("ExpiresOn", request.expiresOn)

    let cmd = proc("Sessions_Create", param)

    querySingle<int>(cmd, "Session")
    |> thenBindAsync(fun sessionId -> 
        let query = 
            {
                sessionId = Some sessionId
                token = None
                userId = None
            }    
        getSession query
    )

let renewSessionExpiration(sessionId : int, expiresOn : DateTime) : Session AsyncHttpResult =
    let param = DynamicParameters()
                    .add("SessionId", sessionId)
                    .add("ExpiresOn", expiresOn)

    let cmd = proc("Sessions_Renew", param)

    queryUnit(cmd, "Session")
    |> thenBindAsync (fun _ -> 
        let query = 
            {
                sessionId = Some sessionId
                token = None
                userId = None
            }    
        getSession query
    )

let deleteSession(sessionId : int option, token : string option) : Unit AsyncHttpResult =
    let param = DynamicParameters()
                    .addOption("SessionId", sessionId)
                    .addOption("Token", token)

    let cmd = proc("Sessions_Delete", param)

    queryUnit(cmd, "Session")