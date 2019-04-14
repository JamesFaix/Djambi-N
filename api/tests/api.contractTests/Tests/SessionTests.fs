module Djambi.Api.ContractTests.SessionTests

open System.Net
open System.Threading.Tasks
open FSharp.Control.Tasks
open NUnit.Framework
open Djambi.Api.WebClient

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