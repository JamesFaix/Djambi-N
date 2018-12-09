module Djambi.Api.ContractTests.SetupUtility

open System.Threading.Tasks
open FSharp.Control.Tasks
open Microsoft.Extensions.Configuration
open Djambi.Api.Web.Model.SessionWebModel
open Djambi.Api.WebClient
open Djambi.Utilities
open Djambi.Api.Model

let private config = 
    ConfigurationBuilder()
        .AddJsonFile(Environment.environmentConfigPath(6), false)
        .Build()

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
        let request : LoginRequestJsonModel = 
            {
                userName = config.["adminUsername"]
                password = config.["adminPassword"]
            }
        let! sessionResponse = SessionClient.createSession request
        return sessionResponse.getToken().Value        
    }