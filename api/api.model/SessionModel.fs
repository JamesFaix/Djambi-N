[<AutoOpen>]
module Djambi.Api.Model.SessionModel

open System
open Djambi.ClientGenerator.Annotations

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
[<ClientType>]
type LoginRequest =
    {
        username : string
        password : string
    }