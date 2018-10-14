module Djambi.Api.ContractTests.SetupUtility

open System.Threading.Tasks
open FSharp.Control.Tasks
open Djambi.Api.Web.Model.LobbyWebModel
open Djambi.Api.WebClient

let createUserAndSignIn () : (UserJsonModel * string) Task =
    task {
        let createUserRequest = RequestFactory.createUserRequest()
        let! user = UserRepository.createUser createUserRequest |> AsyncResponse.bodyValue
        let request = RequestFactory.loginRequest createUserRequest
        let! sessionResponse = SessionRepository.createSession request
        let token = sessionResponse.getToken().Value
        return (user, token)
    }

let createUserAndAddToSession (token : string) : UserJsonModel Task =
    task {
        let createUserRequest = RequestFactory.createUserRequest()
        let! user = UserRepository.createUser createUserRequest |> AsyncResponse.bodyValue
        let request = RequestFactory.loginRequest createUserRequest
        let! _ = SessionRepository.addUserToSession(request, token)
        return user
    }