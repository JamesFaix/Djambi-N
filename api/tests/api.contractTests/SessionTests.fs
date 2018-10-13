module Djambi.Api.ContractTests.SessionTests

open System.Net
open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common
open Djambi.Api.WebClient

[<Fact>]
let ``Create session should work``() =
    task {
        //Arrange
        let createUserRequest = RequestFactory.createUserRequest()
        let! _ = UserRepository.createUser createUserRequest
        let request = RequestFactory.loginRequest createUserRequest
    
        //Act
        let! response = SessionRepository.createSession request
        
        //Assert
        response |> shouldHaveStatus HttpStatusCode.OK

        response.getToken() |> shouldBeSome

        let session = response.result |> Result.value
        session.id |> shouldNotBe 0
        session.userIds.Length |> shouldBe 1
    }

[<Fact>]
let ``Create session should fail if user has another session``() =
    task {
        //Arrange
        let createUserRequest = RequestFactory.createUserRequest()    
        let! _ = UserRepository.createUser createUserRequest
        let request = RequestFactory.loginRequest createUserRequest
    
        let! response1 = SessionRepository.createSession request
        response1 |> shouldHaveStatus HttpStatusCode.OK

        //Act
        let! response2 = SessionRepository.createSession request
        
        //Assert
        response2 |> shouldBeError HttpStatusCode.Conflict "Already signed in."
        response2.getToken() |> shouldBeNone
    }