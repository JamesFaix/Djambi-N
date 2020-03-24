[<AutoOpen>]
module Apex.Api.Model.SessionModel

open System
open Apex.Api.Model

type Session =
    {
        id : int
        user : User
        token : string
        createdOn : DateTime
        expiresOn : DateTime
    }

type LoginRequest =
    {
        username : string
        password : string
    }