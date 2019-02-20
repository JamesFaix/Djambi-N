module Djambi.Api.ContractTests.SetupUtility

open System.Threading.Tasks
open FSharp.Control.Tasks
open Djambi.Api.WebClient
open Djambi.Utilities
open Djambi.Api.Model

let env = Environment.load(6)

let createUserAndSignIn () : (User * string) Task =
    task {
        let createUserRequest = RequestFactory.createUserRequest()
        let! user = UserClient.createUser createUserRequest |> AsyncResponse.bodyValue
        let request = RequestFactory.loginRequest createUserRequest
        let! sessionResponse = SessionClient.createSession request
        let token = sessionResponse.getToken().Value
        return (user, token)
    }

let loginAsAdmin () : string Task =
    task {
        let request : LoginRequest = 
            {
                username = env.adminUsername
                password = env.adminPassword
            }
        let! sessionResponse = SessionClient.createSession request
        return sessionResponse.getToken().Value        
    }