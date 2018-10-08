namespace Djambi.Api.Db.Repositories

open System
open System.Threading.Tasks
open Dapper
open FSharp.Control.Tasks
open Djambi.Api.Common
open Djambi.Api.Db.Mappings.LobbyDbMapping
open Djambi.Api.Db.Model.LobbyDbModel
open Djambi.Api.Db.SqlUtility
open Djambi.Api.Model.LobbyModel

module SessionRepository =

    let getSession(sessionId : int option, 
                   token : string option,
                   userId : int option) : Session option Task =

        let param = new DynamicParameters()
        param.AddOption("SessionId", sessionId)
        param.AddOption("Token", token)
        param.AddOption("UserId", userId)

        let cmd = proc("Lobby.GetSessionWithUsers", param)

        task {
            use cn = getConnection()
            let! sqlModels = cn.QueryAsync<SessionUserSqlModel>(cmd) 
            let sqlModelList = sqlModels |> Seq.toList
            return match sqlModelList.Length with
                    | 0 -> None
                    | _ -> mapSessionUsers sqlModelList |> Some
        }

    let createSessionWithUser(userId : int, token : string, expiresOn : DateTime) : Session Task =
        let param = new DynamicParameters()
        param.Add("UserId", userId)
        param.Add("Token", token)
        param.Add("ExpiresOn", expiresOn)

        let cmd = proc("Lobby.CreateSessionWithUser", param)

        task {
            use cn = getConnection()
            let! sessionId = cn.QuerySingleAsync<int>(cmd)
            return! getSession(Some sessionId, None, None)
                    |> Task.map (fun o -> o.Value)
        }
        
    let addUserToSession(sessionId : int, userId : int) : Session option Task =
        let param = new DynamicParameters()
        param.Add("UserId", userId)
        param.Add("SessionId", sessionId)
        
        let cmd = proc("Lobby.AddUserToSession", param)

        task {
            use cn = getConnection()
            let! _ = cn.ExecuteAsync(cmd)
            return! getSession(Some sessionId, None, None)
        }

    let removeUserFromSession(sessionId : int, userId : int) : Session option Task =
        let param = new DynamicParameters()
        param.Add("UserId", userId)
        param.Add("SessionId", sessionId)
        
        let cmd = proc("Lobby.RemoveUserFromSession", param)

        task {
            use cn = getConnection()
            let! _ = cn.ExecuteAsync(cmd)
            return! getSession(Some sessionId, None, None)
        }

    let renewSessionExpiration(sessionId : int, expiresOn : DateTime) : Session option Task =
        let param = new DynamicParameters()
        param.Add("SessionId", sessionId)
        param.Add("ExpiresOn", expiresOn)
        
        let cmd = proc("Lobby.RenewSessionExpiration", param)

        task {
            use cn = getConnection()
            let! _ = cn.ExecuteAsync(cmd)
            return! getSession(Some sessionId, None, None)
        }

    let deleteSession(sessionId : int option, token : string option) : Unit Task =
        let param = new DynamicParameters()
        param.AddOption("SessionId", sessionId)
        param.AddOption("Token", token)

        let cmd = proc("Lobby.DeleteSession", param)

        task {
            use cn = getConnection()
            let! _ = cn.ExecuteAsync(cmd)
            return ()
        }