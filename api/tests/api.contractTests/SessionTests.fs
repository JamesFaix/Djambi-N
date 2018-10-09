module Djambi.Api.ContractTests.SessionTests

open System
open System.Net
open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Web.Model.LobbyWebModel
open Djambi.Api.WebClient

[<Fact>]
let ``Create session should work``() =
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

        let session = response.result.Value()
        Assert.NotEqual(0, session.id)
        Assert.Equal(1, session.userIds.Length)
    }

[<Fact>]
let ``Create session should fail if user has another session``() =
    task {
        //Arrange
        let createUserRequest = 
            {
                name = Guid.NewGuid().ToString()
                password = "test"
                role = "Normal"
            }
    
        let! _ = UserRepository.createUser createUserRequest

        let request = 
            {
                userName = createUserRequest.name
                password = createUserRequest.password
            }
    
        let! response1 = SessionRepository.createSession request
        Assert.Equal(HttpStatusCode.OK, response1.statusCode)

        //Act
        let! response2 = SessionRepository.createSession request
        
        //Assert
        Assert.Equal(HttpStatusCode.Conflict, response2.statusCode)

        Assert.True(response2.getToken().IsNone)
        let message = response2.result.ErrStr()
        Assert.Equal("User already logged in.", message)
    }