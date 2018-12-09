[<AutoOpen>]
module Djambi.Api.ContractTests.RequestFactory

open System
open Djambi.Api.Web.Model
open Djambi.Api.Model.UserModel
open Djambi.Api.Model.SessionModel

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

let createLobbyRequest () : CreateLobbyJsonModel =
    {
        regionCount = 3
        description = Some "test"
        isPublic = false
        allowGuests = false
    }