namespace Djambi.Api.Db.Repositories

open System
open Dapper
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Mappings.LobbyDbMapping
open Djambi.Api.Db.Model.LobbyDbModel
open Djambi.Api.Db.SqlUtility
open Djambi.Api.Model.LobbyModel

module SessionRepository =

    let getSession(sessionId : int option, 
                   token : string option,
                   userId : int option) : Session AsyncHttpResult =

        let param = DynamicParameters()
                        .addOption("SessionId", sessionId)
                        .addOption("Token", token)
                        .addOption("UserId", userId)

        let cmd = proc("Lobby.GetSessionWithUsers", param)

        let sessionOrError (xs : SessionUserSqlModel list) = 
            match xs.Length with
            | 0 -> Error <| HttpException(404, "Session not found")
            | _ -> Ok <| (mapSessionUsers xs)
                    
        queryMany<SessionUserSqlModel>(cmd, "Session") 
        |> thenBind sessionOrError

    let createSessionWithUser(userId : int, token : string, expiresOn : DateTime) : Session AsyncHttpResult =
        let param = DynamicParameters()
                        .add("UserId", userId)
                        .add("Token", token)
                        .add("ExpiresOn", expiresOn)

        let cmd = proc("Lobby.CreateSessionWithUser", param)

        querySingle<int>(cmd, "Session")
        |> thenBindAsync(fun sessionId -> getSession(Some sessionId, None, None))
        
    let addUserToSession(sessionId : int, userId : int) : Session AsyncHttpResult =
        let param = DynamicParameters()
                        .add("UserId", userId)
                        .add("SessionId", sessionId)
        
        let cmd = proc("Lobby.AddUserToSession", param)

        queryUnit(cmd, "Session")
        |> thenBindAsync (fun _ -> getSession(Some sessionId, None, None))

    let removeUserFromSession(sessionId : int, userId : int) : Session AsyncHttpResult =
        let param = DynamicParameters()
                        .add("UserId", userId)
                        .add("SessionId", sessionId)
        
        let cmd = proc("Lobby.RemoveUserFromSession", param)
        
        queryUnit(cmd, "Session")
        |> thenBindAsync (fun _ -> getSession(Some sessionId, None, None))

    let renewSessionExpiration(sessionId : int, expiresOn : DateTime) : Session AsyncHttpResult =
        let param = DynamicParameters()
                        .add("SessionId", sessionId)
                        .add("ExpiresOn", expiresOn)
        
        let cmd = proc("Lobby.RenewSessionExpiration", param)

        queryUnit(cmd, "Session")
        |> thenBindAsync (fun _ -> getSession(Some sessionId, None, None))

    let deleteSession(sessionId : int option, token : string option) : Unit AsyncHttpResult =
        let param = DynamicParameters()
                        .addOption("SessionId", sessionId)
                        .addOption("Token", token)

        let cmd = proc("Lobby.DeleteSession", param)
        
        queryUnit(cmd, "Session")
        