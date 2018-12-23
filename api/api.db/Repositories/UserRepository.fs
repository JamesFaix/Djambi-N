namespace Djambi.Api.Db.Repositories

open Dapper
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Mapping
open Djambi.Api.Db.Model
open Djambi.Api.Db.SqlUtility
open Djambi.Api.Model

module UserRepository =

    let getUser(id : int) : UserDetails AsyncHttpResult =
        let param = DynamicParameters()
                        .add("UserId", id)
                        .add("Name", null)        
        let cmd = proc("Users_Get", param)

        querySingle<UserSqlModel>(cmd, "User")
        |> thenMap mapUserResponse
    
    let getUserByName(name : string) : UserDetails AsyncHttpResult =
        let param = DynamicParameters()
                        .add("UserId", null)
                        .add("Name", name)        
        let cmd = proc("Users_Get", param)

        querySingle<UserSqlModel>(cmd, "User")
        |> thenMap mapUserResponse

    let getUsers() : UserDetails list AsyncHttpResult =
        let param = DynamicParameters()
                        .add("UserId", null)
                        .add("Name", null)        
        let cmd = proc("Users_Get", param)

        let mapUsers (xs : UserSqlModel list) =
            xs
            |> List.map mapUserResponse
            |> List.sortBy (fun u -> u.id)
            
        queryMany<UserSqlModel>(cmd, "User")
        |> thenMap mapUsers

    let createUser(request : CreateUserRequest) : UserDetails AsyncHttpResult =
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

    let updateFailedLoginAttempts(request : UpdateFailedLoginsRequest) : Unit AsyncHttpResult =
        let param = DynamicParameters()
                        .add("UserId", request.userId)
                        .add("FailedLoginAttempts", request.failedLoginAttempts)
                        .addOption("LastFailedLoginAttemptOn", request.lastFailedLoginAttemptOn)

        let cmd = proc("Users_UpdateFailedLoginAttempts", param)
        queryUnit(cmd, "User")