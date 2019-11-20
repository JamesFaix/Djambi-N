[<AutoOpen>]
module Apex.Api.Model.SessionModel

open System
open Apex.Api.Model
open Apex.ClientGenerator.Annotations

[<ClientType(ClientSection.Session)>]
type Session =
    {
        id : int
        user : User
        token : string
        createdOn : DateTime
        expiresOn : DateTime
    }

[<CLIMutable>]
[<ClientType(ClientSection.Session)>]
type LoginRequest =
    {
        username : string
        password : string
    }