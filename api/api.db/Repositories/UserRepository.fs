namespace Apex.Api.Db.Repositories

open Apex.Api.Common.Control
open Apex.Api.Common.Control.AsyncHttpResult
open Apex.Api.Db
open Apex.Api.Db.Interfaces
open Apex.Api.Enums

type UserRepository(ctxProvider : CommandContextProvider) =

    let getUserPrivileges (userId : int) : Privilege list AsyncHttpResult =
        Commands.getUserPrivileges (Some userId, None)
        |> Command.execute ctxProvider

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