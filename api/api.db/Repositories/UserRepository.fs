namespace Djambi.Api.Db.Repositories

open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db
open Djambi.Api.Db.Interfaces
open Djambi.Api.Model

type UserRepository(ctxProvider : CommandContextProvider) =

    let getUserPrivileges (userId : int) : Privilege list AsyncHttpResult =
        let cmd = Commands.getUserPrivileges (Some userId, None)
        cmd.execute ctxProvider
        |> thenMap (List.map Mapping.mapPrivilegeId)

    interface IUserRepository with
        member x.getUser userId =
            let cmd = Commands.getUser (Some userId, None)
            (cmd.execute ctxProvider)
            |> thenBindAsync (fun userSqlModel -> 
                getUserPrivileges userId
                |> thenMap (Mapping.mapUserResponse userSqlModel)        
            )
    
        member x.getUserByName name =
            let cmd = Commands.getUser (None, Some name)
            (cmd.execute ctxProvider)
            |> thenBindAsync (fun userSqlModel -> 
                getUserPrivileges userSqlModel.userId
                |> thenMap (Mapping.mapUserResponse userSqlModel)        
            )

        member x.createUser request =
            let cmd = Commands2.createUser request
            cmd.execute ctxProvider
            |> thenBindAsync (x :> IUserRepository).getUser

        member x.deleteUser userId =
            let cmd = Commands.deleteUser userId
            cmd.execute ctxProvider

        member x.updateFailedLoginAttempts request =
            let cmd = Commands2.updateFailedLoginAttempts request
            cmd.execute ctxProvider