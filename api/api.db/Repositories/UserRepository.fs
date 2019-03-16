namespace Djambi.Api.Db.Repositories

open Dapper
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Mapping
open Djambi.Api.Db.Model
open Djambi.Api.Db.SqlUtility
open Djambi.Api.Model
open Djambi.Api.Db.Interfaces

type UserRepository() =
    let getUserPrivileges (userId : int) : Privilege list AsyncHttpResult =
        let param = DynamicParameters()
                        .add("UserId", userId)
                        .add("Name", null)
        let cmd = proc("Users_GetPrivileges", param)

        queryMany<byte>(cmd, "Privilege")
        |> thenMap (List.map mapPrivilegeId)

    interface IUserRepository with
        member x.getUser userId =
            let param = DynamicParameters()
                            .add("UserId", userId)
                            .add("Name", null)        
            let cmd = proc("Users_Get", param)

            querySingle<UserSqlModel>(cmd, "User")
            |> thenBindAsync (fun userSqlModel -> 
                getUserPrivileges userId
                |> thenMap (mapUserResponse userSqlModel)        
            )
    
        member x.getUserByName name =
            let param = DynamicParameters()
                            .add("UserId", null)
                            .add("Name", name)        
            let cmd = proc("Users_Get", param)

            querySingle<UserSqlModel>(cmd, "User")
            |> thenBindAsync (fun userSqlModel -> 
                getUserPrivileges userSqlModel.userId
                |> thenMap (mapUserResponse userSqlModel)        
            )

        member x.createUser request =
            let param = DynamicParameters()
                            .add("Name", request.name)
                            .add("Password", request.password)

            let cmd = proc("Users_Create", param)

            querySingle<int>(cmd, "User")
            |> thenBindAsync (x :> IUserRepository).getUser

        member x.deleteUser id =
            let param = DynamicParameters()
                            .add("UserId", id)
            let cmd = proc("Users_Delete", param)
            queryUnit(cmd, "User")

        member x.updateFailedLoginAttempts request =
            let param = DynamicParameters()
                            .add("UserId", request.userId)
                            .add("FailedLoginAttempts", request.failedLoginAttempts)
                            .addOption("LastFailedLoginAttemptOn", request.lastFailedLoginAttemptOn)

            let cmd = proc("Users_UpdateFailedLoginAttempts", param)
            queryUnit(cmd, "User")