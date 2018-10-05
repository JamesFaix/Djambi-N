namespace Djambi.Api.Persistence

open System
open System.Linq
open System.Threading.Tasks

open Dapper
open FSharp.Control.Tasks
    
open Djambi.Api.Persistence.LobbySqlModels
open Djambi.Api.Model.Lobby
open Djambi.Api.Persistence.LobbySqlMappings
open Djambi.Api.Persistence.SqlUtility
open Djambi.Api.Common

module UserRepository =

    let private getUsersInner(id : int option, name : string option) : User list Task =
        let param = new DynamicParameters()
        param.AddOption("UserId", id)
        param.AddOption("Name", name)        
        let cmd = proc("Lobby.Get_Users", param)

        task {
            use cn = getConnection()
            let! sqlModels = cn.QueryAsync<UserSqlModel>(cmd)
            return sqlModels 
                    |> Seq.map mapUserResponse
                    |> Seq.sortBy (fun u -> u.id)
                    |> Seq.toList
        }

    let getUser(id : int) : User Task =
        getUsersInner(Some id, None)
        |> Task.map (getSingle "User")
    
    let getUserByName(name : string) : User Task =
        getUsersInner(None, Some name)
        |> Task.map (getSingle "User")

    let getUsers() : User list Task =
        getUsersInner(None, None)

    let createUser(request : CreateUserRequest) : User Task =
        let param = new DynamicParameters()
        param.Add("Name", request.name)
        param.Add("RoleId", request.role |> mapRoleToId)
        param.Add("Password", request.password)
        let cmd = proc("Lobby.Insert_User", param)

        task {
            use cn = getConnection()
            let! id = cn.ExecuteScalarAsync<int>(cmd)
            return! getUser id
        }

    let deleteUser(id : int) : Unit Task =
        let param = new DynamicParameters()
        param.Add("UserId", id)
        let cmd = proc("Lobby.Delete_User", param)

        task {
            use cn = getConnection()
            let! _  = cn.ExecuteAsync(cmd) 
            return ()
        }

    let createSession(userId : int, token : string, expiresOn : DateTime) : Unit Task =
        let param = new DynamicParameters()
        param.Add("UserId", userId)
        param.Add("Token", token)
        param.Add("ExpiresOn", expiresOn)

        let cmd = proc("Lobby.Insert_Session", param)

        task {
            use cn = getConnection()
            let! _ = cn.ExecuteAsync(cmd)
            return ()
        }
    
    let deleteSession(userId : int) : Unit Task =
        let param = new DynamicParameters()
        param.Add("UserId", userId)
        param.Add("Token", null)

        let cmd = proc("Lobby.Delete_Session", param)

        task {
            use cn = getConnection()
            let! _ = cn.ExecuteAsync(cmd)
            return ()
        }
        
    let getSessionInner (userId : int option) (token : string option) =
        let param = new DynamicParameters()
        param.AddOption("UserId", userId)
        param.AddOption("Token", token)

        let cmd = proc("Lobby.Get_Session", param)
        
        task {
            use cn = getConnection()
            return! cn.QueryAsync<SessionSqlModel>(cmd)
        }

    let getSession(userId : int) : Session Task =
        getSessionInner (Some userId) None
        |> Task.map (Enumerable.Single >> mapSession)

    let getSessionFromToken(token : string) : Session Task =
        getSessionInner None (Some token)
        |> Task.map (Enumerable.Single >> mapSession)

    let userHasSession(userId : int) : bool Task =
        getSessionInner (Some userId) None
        |> Task.map Enumerable.Any

    let renewSession(userId : int, expiresOn : DateTime) : Unit Task =
        let param = new DynamicParameters()
        param.Add("UserId", userId)
        param.Add("ExpiresOn", expiresOn)

        let cmd = proc("Lobby.Update_Session", param)

        task {
            use cn = getConnection()
            let! _ = cn.ExecuteAsync(cmd)
            return ()
        }

    let updateFailedLoginAttempts(userId : int, 
                                  failedLoginAttempts : int, 
                                  lastFailedLoginAttemptOn : DateTime option) 
                                  : Unit Task =
        let param = new DynamicParameters()
        param.Add("UserId", userId)
        param.Add("FailedLoginAttempts", failedLoginAttempts)
        param.AddOption("LastFailedLoginAttemptOn", lastFailedLoginAttemptOn)

        let cmd = proc("Lobby.Update_FailedLoginAttempts", param)

        task {
            use cn = getConnection()
            let! _ = cn.ExecuteAsync(cmd)
            return ()
        }