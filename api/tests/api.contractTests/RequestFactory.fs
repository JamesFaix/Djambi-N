[<AutoOpen>]
module Djambi.Api.ContractTests.RequestFactory

open System
open Djambi.Api.Web.Model.LobbyWebModel
open Djambi.Api.Web.Model.SessionWebModel
open Djambi.Api.Web.Model.UserWebModel

let createUserRequest() : CreateUserJsonModel = 
    {
        name = Guid.NewGuid().ToString()
        password = "test"
    }

let loginRequest (createUserRequest : CreateUserJsonModel) : LoginRequestJsonModel =
    {
        userName = createUserRequest.name
        password = createUserRequest.password
    }

let createLobbyRequest () : CreateLobbyJsonModel =
    {
        regionCount = 3
        description = Some "test"
        isPublic = false
        allowGuests = false
    }