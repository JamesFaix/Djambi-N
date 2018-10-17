module Djambi.Api.Model.UserModel

open System

type User = 
    {
        id : int
        name : string
        isAdmin : bool
        password : string
        failedLoginAttempts : int
        lastFailedLoginAttemptOn : DateTime option
    }
        
type CreateUserRequest =
    {
        name : string
        password : string
    }