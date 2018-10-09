module Djambi.Api.ContractTests.SessionTests

open System
open System.Net
open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Web.Model.LobbyWebModel
open Djambi.Api.WebClient

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
    
        let! _ = UserRepository.createUser createUserRequest

        //Act
        let request = 
            {
                userName = createUserRequest.name
                password = createUserRequest.password
            }
    
        let! response = SessionRepository.createSession request
        
        //Assert
        Assert.Equal(HttpStatusCode.OK, response.statusCode)

        Assert.True(response.getToken().IsSome)

        Assert.NotEqual(0, response.value.id)
        Assert.Equal(1, response.value.userIds.Length)
    }