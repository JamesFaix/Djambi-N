[<AutoOpen>]
module Djambi.Api.ContractTests.RequestFactory

open System
open Djambi.Api.Model

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