[<AutoOpen>]
module Djambi.Api.Model.SessionModel

open System
open Djambi.Api.Model

type Session =
    {
        id : int
        user : User
        token : string
        createdOn : DateTime
        expiresOn : DateTime
    }

[<CLIMutable>]
type LoginRequest =
    {
        username : string
        password : string
    }