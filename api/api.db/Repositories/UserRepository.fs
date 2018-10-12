namespace Djambi.Api.Db.Repositories

open System
open Dapper
open Djambi.Api.Common
open Djambi.Api.Db.Mappings.LobbyDbMapping
open Djambi.Api.Db.Model.LobbyDbModel
open Djambi.Api.Db.SqlUtility
open Djambi.Api.Model.LobbyModel

module UserRepository =

    let getUser(id : int) : User AsyncHttpResult =
        let param = DynamicParameters()
                        .add("UserId", id)
                        .add("Name", null)        
        let cmd = proc("Lobby.GetUsers", param)

        querySingle<UserSqlModel>(cmd, "User")
        |> Task.thenMap mapUserResponse
    
    let getUserByName(name : string) : User AsyncHttpResult =
        let param = DynamicParameters()
                        .add("UserId", null)
                        .add("Name", name)        
        let cmd = proc("Lobby.GetUsers", param)

        querySingle<UserSqlModel>(cmd, "User")
        |> Task.thenMap mapUserResponse

    let getUsers() : User list AsyncHttpResult =
        let param = DynamicParameters()
                        .add("UserId", null)
                        .add("Name", null)        
        let cmd = proc("Lobby.GetUsers", param)

        let mapUsers (xs : UserSqlModel list) =
            xs
            |> List.map mapUserResponse
            |> List.sortBy (fun u -> u.id)


        queryMany<UserSqlModel>(cmd, "User")
        |> Task.thenMap mapUsers

    let createUser(request : CreateUserRequest) : User AsyncHttpResult =
        let param = DynamicParameters()
                        .add("Name", request.name)
                        .add("RoleId", request.role |> mapRoleToId)
                        .add("Password", request.password)

        let cmd = proc("Lobby.CreateUser", param)

        querySingle<int>(cmd, "User")
        |> Task.thenBindAsync getUser

    let deleteUser(id : int) : Unit AsyncHttpResult =
        let param = DynamicParameters()
                        .add("UserId", id)
        let cmd = proc("Lobby.DeleteUser", param)
        queryUnit(cmd, "User")

    let updateFailedLoginAttempts(userId : int, 
                                  failedLoginAttempts : int, 
                                  lastFailedLoginAttemptOn : DateTime option) 
                                  : Unit AsyncHttpResult =
        let param = DynamicParameters()
                        .add("UserId", userId)
                        .add("FailedLoginAttempts", failedLoginAttempts)
                        .addOption("LastFailedLoginAttemptOn", lastFailedLoginAttemptOn)

        let cmd = proc("Lobby.UpdateUserFailedLoginAttempts", param)
        queryUnit(cmd, "User")