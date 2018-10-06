namespace Djambi.Api.Db.Repositories

open System
open System.Data
open System.Threading.Tasks
open Dapper
open FSharp.Control.Tasks
open Djambi.Api.Common
open Djambi.Api.Db.Mappings.LobbyDbMapping
open Djambi.Api.Db.Model.LobbyDbModel
open Djambi.Api.Db.SqlUtility
open Djambi.Api.Model.LobbyModel

module SessionRepository =

    let getSession(sessionId : int option, token : string option) : Session Task =
        let param = new DynamicParameters()
        param.AddOption("SessionId", sessionId)
        param.AddOption("Token", token)
        
        let cmd = proc("Lobby.GetSessionWithUsers", param)

        task {
            use cn = getConnection()
            return! cn.QueryAsync<SessionUserSqlModel>(cmd)
                    |> Task.map mapSessionUsers
        }

    let createSessionWithUser(userId : int, token : string, expiresOn : DateTime) : Session Task =
        let param = new DynamicParameters()
        param.Add("UserId", userId)
        param.Add("Token", token)
        param.Add("ExpiresOn", expiresOn)
        param.AddOutput("SessionId", DbType.Int32)

        let cmd = proc("Lobby.CreateSessionWithUser", param)

        task {
            use cn = getConnection()
            let _ = cn.ExecuteAsync(cmd)
            let sessionId = param.Get<int>("SessionId")
            return! getSession(Some sessionId, None)
        }
        
    let addUserToSession(sessionId : int, userId : int) : Session Task =
        let param = new DynamicParameters()
        param.Add("UserId", userId)
        param.Add("SessionId", sessionId)
        
        let cmd = proc("Lobby.AddUserToSession", param)

        task {
            use cn = getConnection()
            let! _ = cn.ExecuteAsync(cmd)
            return! getSession(Some sessionId, None)
        }

    let removeUserFromSession(sessionId : int, userId : int) : Session Task =
        let param = new DynamicParameters()
        param.Add("UserId", userId)
        param.Add("SessionId", sessionId)
        
        let cmd = proc("Lobby.RemoveUserFromSession", param)

        task {
            use cn = getConnection()
            let! _ = cn.ExecuteAsync(cmd)
            return! getSession(Some sessionId, None)
        }

    let renewSessionExpiration(sessionId : int, expiresOn : DateTime) : Unit Task =
        let param = new DynamicParameters()
        param.Add("SessionId", sessionId)
        param.Add("ExpiresOn", expiresOn)
        
        let cmd = proc("Lobby.RenewSessionExpiration", param)

        task {
            use cn = getConnection()
            let! _ = cn.ExecuteAsync(cmd)
            return ()
        }