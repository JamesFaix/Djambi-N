module Djambi.Api.ContractTests.SetupUtility

open System.Threading.Tasks
open FSharp.Control.Tasks
open Djambi.Api.Web.Model.UserWebModel
open Djambi.Api.WebClient

let createUserAndSignIn () : (UserResponseJsonModel * string) Task =
    task {
        let createUserRequest = RequestFactory.createUserRequest()
        let! user = UserClient.createUser createUserRequest |> AsyncResponse.bodyValue
        let request = RequestFactory.loginRequest createUserRequest
        let! sessionResponse = SessionClient.createSession request
        let token = sessionResponse.getToken().Value
        return (user, token)
    }