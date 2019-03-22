namespace Djambi.Api.Db.Repositories

open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db
open Djambi.Api.Db.Interfaces
open Djambi.Api.Db.Model
open Djambi.Api.Model

type UserRepository(u : SqlUtility) =
    let getUserPrivileges (userId : int) : Privilege list AsyncHttpResult =
        let cmd = Commands.getUserPrivileges (Some userId, None)
        u.queryMany<byte>(cmd)
        |> thenMap (List.map Mapping.mapPrivilegeId)

    interface IUserRepository with
        member x.getUser userId =
            let cmd = Commands.getUser (Some userId, None)
            u.querySingle<UserSqlModel>(cmd)
            |> thenBindAsync (fun userSqlModel -> 
                getUserPrivileges userId
                |> thenMap (Mapping.mapUserResponse userSqlModel)        
            )
    
        member x.getUserByName name =
            let cmd = Commands.getUser (None, Some name)
            u.querySingle<UserSqlModel>(cmd)
            |> thenBindAsync (fun userSqlModel -> 
                getUserPrivileges userSqlModel.userId
                |> thenMap (Mapping.mapUserResponse userSqlModel)        
            )

        member x.createUser request =
            let cmd = Commands2.createUser request
            u.querySingle<int>(cmd)
            |> thenBindAsync (x :> IUserRepository).getUser

        member x.deleteUser userId =
            let cmd = Commands.deleteUser userId
            u.queryUnit(cmd)

        member x.updateFailedLoginAttempts request =
            let cmd = Commands2.updateFailedLoginAttempts request
            u.queryUnit(cmd)