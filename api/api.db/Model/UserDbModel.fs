[<AutoOpen>]
module Djambi.Api.Db.Model.UserDbModel

open System
open Djambi.Api.Common.Utilities
open Djambi.Api.Model

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

let mapUserResponse (sqlModel : UserSqlModel) : UserDetails =
    {
        id = sqlModel.userId
        name = sqlModel.name
        isAdmin = sqlModel.isAdmin
        password = sqlModel.password
        failedLoginAttempts = int sqlModel.failedLoginAttempts
        lastFailedLoginAttemptOn = sqlModel.lastFailedLoginAttemptOn |> nullableToOption
    }