[<AutoOpen>]
module Apex.Api.ContractTests.RequestFactory

open System
open Apex.Api.Model

let createUserRequest() : CreateUserRequest =
    {
        name = Guid.NewGuid().ToString()
        password = "test"
    }

let loginRequest (createUserRequest : CreateUserRequest) : LoginRequest =
    {
        username = createUserRequest.name
        password = createUserRequest.password
    }

let gameParameters () : GameParameters =
    {
        regionCount = 3
        description = Some "test"
        isPublic = false
        allowGuests = false
    }