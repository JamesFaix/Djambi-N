namespace Djambi.Api.Db.Repositories

open System
open Dapper
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Model.UserDbModel
open Djambi.Api.Db.SqlUtility
open Djambi.Api.Model.UserModel

module UserRepository =

    let getUser(id : int) : User AsyncHttpResult =
        let param = DynamicParameters()
                        .add("UserId", id)
                        .add("Name", null)        
        let cmd = proc("Users_Get", param)

        querySingle<UserSqlModel>(cmd, "User")
        |> thenMap UserSqlModel.toModel
    
    let getUserByName(name : string) : User AsyncHttpResult =
        let param = DynamicParameters()
                        .add("UserId", null)
                        .add("Name", name)        
        let cmd = proc("Users_Get", param)

        querySingle<UserSqlModel>(cmd, "User")
        |> thenMap UserSqlModel.toModel

    let getUsers() : User list AsyncHttpResult =
        let param = DynamicParameters()
                        .add("UserId", null)
                        .add("Name", null)        
        let cmd = proc("Users_Get", param)

        let mapUsers (xs : UserSqlModel list) =
            xs
            |> List.map UserSqlModel.toModel
            |> List.sortBy (fun u -> u.id)
            
        queryMany<UserSqlModel>(cmd, "User")
        |> thenMap mapUsers

    let createUser(request : CreateUserRequest) : User AsyncHttpResult =
        let param = DynamicParameters()
                        .add("Name", request.name)
                        .add("Password", request.password)

        let cmd = proc("Users_Create", param)

        querySingle<int>(cmd, "User")
        |> thenBindAsync getUser

    let deleteUser(id : int) : Unit AsyncHttpResult =
        let param = DynamicParameters()
                        .add("UserId", id)
        let cmd = proc("Users_Delete", param)
        queryUnit(cmd, "User")

    let updateFailedLoginAttempts(userId : int, 
                                  failedLoginAttempts : int, 
                                  lastFailedLoginAttemptOn : DateTime option) 
                                  : Unit AsyncHttpResult =
        let param = DynamicParameters()
                        .add("UserId", userId)
                        .add("FailedLoginAttempts", failedLoginAttempts)
                        .addOption("LastFailedLoginAttemptOn", lastFailedLoginAttemptOn)

        let cmd = proc("Users_UpdateFailedLoginAttempts", param)
        queryUnit(cmd, "User")