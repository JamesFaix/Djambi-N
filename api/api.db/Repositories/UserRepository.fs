namespace Djambi.Api.Db.Repositories

open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Model
open Djambi.Api.Db.SqlUtility
open Djambi.Api.Model
open Djambi.Api.Db.Interfaces
open Djambi.Api.Db

type UserRepository() =
    let getUserPrivileges (userId : int) : Privilege list AsyncHttpResult =
        let cmd = Commands.getUserPrivileges (Some userId, None)
        queryMany<byte>(cmd, "Privilege")
        |> thenMap (List.map Mapping.mapPrivilegeId)

    interface IUserRepository with
        member x.getUser userId =
            let cmd = Commands.getUser (Some userId, None)
            querySingle<UserSqlModel>(cmd, "User")
            |> thenBindAsync (fun userSqlModel -> 
                getUserPrivileges userId
                |> thenMap (Mapping.mapUserResponse userSqlModel)        
            )
    
        member x.getUserByName name =
            let cmd = Commands.getUser (None, Some name)
            querySingle<UserSqlModel>(cmd, "User")
            |> thenBindAsync (fun userSqlModel -> 
                getUserPrivileges userSqlModel.userId
                |> thenMap (Mapping.mapUserResponse userSqlModel)        
            )

        member x.createUser request =
            let cmd = Commands2.createUser request
            querySingle<int>(cmd, "User")
            |> thenBindAsync (x :> IUserRepository).getUser

        member x.deleteUser userId =
            let cmd = Commands.deleteUser userId
            queryUnit(cmd, "User")

        member x.updateFailedLoginAttempts request =
            let cmd = Commands2.updateFailedLoginAttempts request
            queryUnit(cmd, "User")