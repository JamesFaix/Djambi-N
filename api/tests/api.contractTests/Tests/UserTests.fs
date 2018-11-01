module Djambi.Api.ContractTests.UserTests

open System.Net
open System.Threading.Tasks
open FSharp.Control.Tasks
open NUnit.Framework
open Djambi.Api.WebClient

[<Test>]
let ``Create user should work`` () =
    task {
        //Arrange
        let request = RequestFactory.createUserRequest()

        //Act
        let! response = UserClient.createUser request

        //Assert
        response |> shouldHaveStatus HttpStatusCode.OK

        let user = response.bodyValue
        user.name |> shouldBe request.name
        user.isAdmin |> shouldBe false
    } :> Task

[<Test>]
let ``Delete user should work if admin and deleting other user`` () =
    task {
        //Arrange
        let! token = SetupUtility.loginAsAdmin()
        let request = RequestFactory.createUserRequest()
        let! user = UserClient.createUser request
                    |> AsyncResponse.bodyValue

        //Act
        let! deleteResponse = UserClient.deleteUser(user.id, token)
        let! getResponse = UserClient.getUser(user.id, token)

        //Assert
        deleteResponse |> shouldHaveStatus HttpStatusCode.OK
        getResponse |> shouldBeError HttpStatusCode.NotFound "User not found."
    } :> Task

//TODO: Delete user should log out if deleting self

[<Test>]
let ``Get users should return multiple users`` () =
    task {
        //Arrange
        let! token = SetupUtility.loginAsAdmin()
        let request1 = RequestFactory.createUserRequest()
        let request2 = RequestFactory.createUserRequest()

        let! user1 = UserClient.createUser request1
                     |> AsyncResponse.bodyValue

        let! user2 = UserClient.createUser request2
                     |> AsyncResponse.bodyValue

        //Act
        let! response = UserClient.getUsers token

        //Assert
        response |> shouldHaveStatus HttpStatusCode.OK

        let responseUsers = response.bodyValue
        responseUsers.Length |> shouldBeAtLeast 2
        responseUsers |> shouldExist (fun u -> u.id = user1.id)
        responseUsers |> shouldExist (fun u -> u.id = user2.id)
    } :> Task

[<Test>]
let ``Get user should work`` () =
    task {
        //Arrange
        let! token = SetupUtility.loginAsAdmin()
        let request = RequestFactory.createUserRequest()
        let! user = UserClient.createUser request
                    |> AsyncResponse.bodyValue

        //Act
        let! response = UserClient.getUser(user.id, token)

        //Assert
        response |> shouldHaveStatus HttpStatusCode.OK

        let responseUser = response.bodyValue
        responseUser.name |> shouldBe request.name
        responseUser.isAdmin |> shouldBe false
    } :> Task