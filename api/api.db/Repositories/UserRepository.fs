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

module UserRepository =

    let private getUsersInner(id : int option, name : string option) : User list Task =
        let param = new DynamicParameters()
        param.AddOption("UserId", id)
        param.AddOption("Name", name)        
        let cmd = proc("Lobby.GetUsers", param)

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
        param.AddOutput("UserId", DbType.Int32)

        let cmd = proc("Lobby.CreateUser", param)

        task {
            use cn = getConnection()
            let _ = cn.ExecuteAsync(cmd)
            let userId = param.Get<int>("UserId")
            return! getUser userId
        }

    let deleteUser(id : int) : Unit Task =
        let param = new DynamicParameters()
        param.Add("UserId", id)
        let cmd = proc("Lobby.DeleteUser", param)

        task {
            use cn = getConnection()
            let! _  = cn.ExecuteAsync(cmd) 
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

        let cmd = proc("Lobby.UpdateUserFailedLoginAttempts", param)

        task {
            use cn = getConnection()
            let! _ = cn.ExecuteAsync(cmd)
            return ()
        }