[<AutoOpen>]
module Djambi.Api.Model.SessionModel

open System
    
type Session =
    {
        id : int
        userId : int
        token : string
        createdOn : DateTime
        expiresOn : DateTime
        isAdmin : bool
    }

[<CLIMutable>]
type LoginRequest =
    {
        username : string
        password : string
    }