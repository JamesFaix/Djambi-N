module Djambi.Api.Db.Model.UserDbModel

open System
open Djambi.Api.Common.Utilities
open Djambi.Api.Model.UserModel

[<CLIMutable>]
type UserSqlModel = 
    {
        userId : int
        name : string
        isAdmin : bool
        password : string
        failedLoginAttempts : byte
        lastFailedLoginAttemptOn : DateTime Nullable
    }

module UserSqlModel =

    let toModel (sqlModel : UserSqlModel) : User =
        {
            id = sqlModel.userId
            name = sqlModel.name
            isAdmin = sqlModel.isAdmin
            password = sqlModel.password
            failedLoginAttempts = int sqlModel.failedLoginAttempts
            lastFailedLoginAttemptOn = sqlModel.lastFailedLoginAttemptOn |> nullableToOption
        }