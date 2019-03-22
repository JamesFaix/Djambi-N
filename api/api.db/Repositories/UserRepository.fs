namespace Djambi.Api.Db.Repositories

open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db
open Djambi.Api.Db.Interfaces
open Djambi.Api.Model

type UserRepository(ctxProvider : CommandContextProvider) =

    let getUserPrivileges (userId : int) : Privilege list AsyncHttpResult =
        Commands.getUserPrivileges (Some userId, None)
        |> Command.execute ctxProvider
        |> thenMap (List.map Mapping.mapPrivilegeId)

    interface IUserRepository with
        member x.getUser userId =
            Commands.getUser (Some userId, None)
            |> Command.execute ctxProvider
            |> thenBindAsync (fun userSqlModel -> 
                getUserPrivileges userId
                |> thenMap (Mapping.mapUserResponse userSqlModel)        
            )
    
        member x.getUserByName name =
            Commands.getUser (None, Some name)
            |> Command.execute ctxProvider
            |> thenBindAsync (fun userSqlModel -> 
                getUserPrivileges userSqlModel.userId
                |> thenMap (Mapping.mapUserResponse userSqlModel)        
            )

        member x.createUser request =
            Commands2.createUser request
            |> Command.execute ctxProvider
            |> thenBindAsync (x :> IUserRepository).getUser

        member x.deleteUser userId =
            Commands.deleteUser userId
            |> Command.execute ctxProvider

        member x.updateFailedLoginAttempts request =
            Commands2.updateFailedLoginAttempts request
            |> Command.execute ctxProvider