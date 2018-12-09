﻿[<AutoOpen>]
module Djambi.Api.Model.UserModel

open System

type User =
    {
        id : int
        name : string
        isAdmin : bool
    }

type UserDetails = 
    {
        id : int
        name : string
        isAdmin : bool
        password : string
        failedLoginAttempts : int
        lastFailedLoginAttemptOn : DateTime option
    }

module UserDetails =
    let hideDetails (user : UserDetails) : User =
        {
            id = user.id
            name = user.name
            isAdmin = user.isAdmin
        }

[<CLIMutable>]        
type CreateUserRequest =
    {
        name : string
        password : string
    }