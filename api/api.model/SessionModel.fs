[<AutoOpen>]
module Djambi.Api.Model.SessionModel

open System
open Djambi.Api.Model
open Djambi.ClientGenerator.Annotations

[<ClientType>]
type Session =
    {
        id : int
        user : User
        token : string
        createdOn : DateTime
        expiresOn : DateTime
    }

[<CLIMutable>]
[<ClientType>]
type LoginRequest =
    {
        username : string
        password : string
    }