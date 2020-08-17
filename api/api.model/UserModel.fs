[<AutoOpen>]
module Djambi.Api.Model.UserModel

open System
open Djambi.Api.Enums

type User =
    {
        id : int
        name : string
        privileges : Privilege list
    }

type User with
    member x.has (p : Privilege) =
        x.privileges |> List.contains p

type UserDetails =
    {
        id : int
        name : string
        privileges : Privilege list
        password : string
        failedLoginAttempts : int
        lastFailedLoginAttemptOn : DateTime option
    }

module UserDetails =
    let hideDetails (user : UserDetails) : User =
        {
            id = user.id
            name = user.name
            privileges = user.privileges
        }

type CreateUserRequest =
    {
        name : string
        password : string
    }

type CreationSource =
    {
        userId : int
        userName : string
        time : DateTime
    }

type PasswordCheckResult = {
    verified: bool
    needsUpgrade: bool
}
