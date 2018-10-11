namespace Djambi.Api.Db.Repositories

open System
open System.Threading.Tasks
open Dapper
open FSharp.Control.Tasks
open Djambi.Api.Db.Mappings.LobbyDbMapping
open Djambi.Api.Db.Model.LobbyDbModel
open Djambi.Api.Db.SqlUtility
open Djambi.Api.Model.LobbyModel

module UserRepository =

    let getUser(id : int) : User Task =
        let param = new DynamicParameters()
        param.Add("UserId", id)
        param.Add("Name", null)        
        let cmd = proc("Lobby.GetUsers", param)

        task {
            let! sqlModel = querySingle<UserSqlModel>(cmd, "User")
            return sqlModel |> mapUserResponse
        }
    
    let getUserByName(name : string) : User Task =
        let param = new DynamicParameters()
        param.Add("UserId", null)
        param.Add("Name", name)        
        let cmd = proc("Lobby.GetUsers", param)

        task {
            let! sqlModel = querySingle<UserSqlModel>(cmd, "User")
            return sqlModel |> mapUserResponse
        }

    let getUsers() : User list Task =
        let param = new DynamicParameters()
        param.Add("UserId", null)
        param.Add("Name", null)        
        let cmd = proc("Lobby.GetUsers", param)

        task {
            let! sqlModels = queryMany<UserSqlModel>(cmd, "User")
            return sqlModels 
                    |> Seq.map mapUserResponse
                    |> Seq.sortBy (fun u -> u.id)
                    |> Seq.toList
        }

    let createUser(request : CreateUserRequest) : User Task =
        let param = new DynamicParameters()
        param.Add("Name", request.name)
        param.Add("RoleId", request.role |> mapRoleToId)
        param.Add("Password", request.password)

        let cmd = proc("Lobby.CreateUser", param)

        task {
            let! userId = querySingle<int>(cmd, "User")
            return! getUser userId
        }

    let deleteUser(id : int) : Unit Task =
        let param = new DynamicParameters()
        param.Add("UserId", id)
        let cmd = proc("Lobby.DeleteUser", param)
        queryUnit(cmd, "User")

    let updateFailedLoginAttempts(userId : int, 
                                  failedLoginAttempts : int, 
                                  lastFailedLoginAttemptOn : DateTime option) 
                                  : Unit Task =
        let param = new DynamicParameters()
        param.Add("UserId", userId)
        param.Add("FailedLoginAttempts", failedLoginAttempts)
        param.AddOption("LastFailedLoginAttemptOn", lastFailedLoginAttemptOn)

        let cmd = proc("Lobby.UpdateUserFailedLoginAttempts", param)
        queryUnit(cmd, "User")