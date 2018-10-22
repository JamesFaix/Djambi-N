module Djambi.Api.ContractTests.SessionTests

open System.Net
open System.Threading.Tasks
open FSharp.Control.Tasks
open NUnit.Framework
open Djambi.Api.WebClient
open Djambi.Api.Web.Model.SessionWebModel

[<Test>]
let ``Create session should work``() =
    task {
        //Arrange
        let createUserRequest = RequestFactory.createUserRequest()
        let! user = UserClient.createUser createUserRequest |> AsyncResponse.bodyValue
        let request = RequestFactory.loginRequest createUserRequest
    
        //Act
        let! response = SessionClient.createSession request
        
        //Assert
        response |> shouldHaveStatus HttpStatusCode.OK
        response.getToken() |> shouldBeSome
    } :> Task

[<Test>]
let ``Create session should fail if user has another session``() =
    task {
        //Arrange
        let createUserRequest = RequestFactory.createUserRequest()    
        let! _ = UserClient.createUser createUserRequest
        let request = RequestFactory.loginRequest createUserRequest
    
        let! response1 = SessionClient.createSession request
        response1 |> shouldHaveStatus HttpStatusCode.OK

        //Act
        let! response2 = SessionClient.createSession request
        
        //Assert
        response2 |> shouldBeError HttpStatusCode.Conflict "Already signed in."
        response2.getToken() |> shouldBeNone
    } :> Task

[<Test>]
let ``Create session should fail if request has a token``() =
    task {
        //Arrange
        let createUserRequest = RequestFactory.createUserRequest()    
        let! _ = UserClient.createUser createUserRequest
        let request = RequestFactory.loginRequest createUserRequest
    
        //Act
        let! response = SessionClient.tryToCreateSessionWithToken(request, "someToken")

        //Assert
        response |> shouldBeError HttpStatusCode.Unauthorized "Operation not allowed if already signed in."
        response.getToken() |> shouldBeNone
    } :> Task

[<Test>]
let ``Create session should fail if incorrect password``() =
    task {
        //Arrange
        let createUserRequest = RequestFactory.createUserRequest()    
        let! _ = UserClient.createUser createUserRequest
        let request : LoginRequestJsonModel = 
            {
                userName = createUserRequest.name
                password = "wrong"
            }            
    
        //Act
        let! response = SessionClient.createSession request

        //Assert
        response |> shouldBeError HttpStatusCode.Unauthorized "Incorrect password."
        response.getToken() |> shouldBeNone
    } :> Task

[<Test>]
let ``Create session should fail with locked message on 6th incorrect password attempt``() =
    task {
        //Arrange
        let createUserRequest = RequestFactory.createUserRequest()    
        let! _ = UserClient.createUser createUserRequest
        let request = 
            {
                userName = createUserRequest.name
                password = "wrong"
            }            
        
        for _ in [1..5] do
            let! passwordResponse = SessionClient.createSession request
            passwordResponse |> shouldBeError HttpStatusCode.Unauthorized "Incorrect password."

        //Act
        let! lockedResponse = SessionClient.createSession request

        //Assert
        lockedResponse |> shouldBeError HttpStatusCode.Unauthorized "Account locked."
    } :> Task
    
[<Test>]
let ``Close session should work``() =
    task {
        //Arrange
        let! (_, token) = SetupUtility.createUserAndSignIn()
    
        //Act
        let! response = SessionClient.closeSession token

        //Assert
        response |> shouldHaveStatus HttpStatusCode.OK
        response.getToken().Value |> shouldBe ""
    } :> Task

[<Test>]
let ``Close session should clear cookie if no session on backend``() =
    task {
        //Arrange

        //Act
        let! response = SessionClient.closeSession "someToken"

        //Assert
        response |> shouldBeError HttpStatusCode.Unauthorized "Not signed in."
        response.getToken().Value |> shouldBe ""
    } :> Task