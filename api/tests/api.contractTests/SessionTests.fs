module Djambi.Api.ContractTests.SessionTests

open System
open System.Net
open System.Net.Http
open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Web.Model.LobbyWebModel

[<Fact>]
let ``POST sessions should work``() =
    task {
        //Arrange
        let createUserRequest = 
            {
                name = Guid.NewGuid().ToString()
                password = "test"
                role = "Normal"
            }
    
        let! (_, _ : unit) = WebUtility.sendRequest("/users", HttpMethod.Post, createUserRequest)

        //Act
        let request = 
            {
                userName = createUserRequest.name
                password = createUserRequest.password
            }
    
        let! (statusCode, _ : unit) = WebUtility.sendRequest("/sessions", HttpMethod.Post, request)
        
        Assert.Equal(HttpStatusCode.OK, statusCode)
    }

[<Fact>]
let ``POST user should work`` () =
    task {
        let request = 
            {
                name = Guid.NewGuid().ToString()
                password = "test"
                role = "Normal"
            }
    
        let! (statusCode, _ : unit) = WebUtility.sendRequest("/users", HttpMethod.Post, request)
        
        Assert.Equal(HttpStatusCode.OK, statusCode)
    }