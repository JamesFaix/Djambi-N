[<AutoOpen>]
module Djambi.Api.ContractTests.RequestFactory

open System
open Djambi.Api.Web.Model.LobbyWebModel

let createUserRequest() : CreateUserJsonModel = 
    {
        name = Guid.NewGuid().ToString()
        password = "test"
        role = "Normal"
    }

let loginRequest (createUserRequest : CreateUserJsonModel) : LoginRequestJsonModel =
    {
        userName = createUserRequest.name
        password = createUserRequest.password
    }