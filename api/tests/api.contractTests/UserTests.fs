module Djambi.Api.ContractTests.UserTests

open System
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
let ``Create user should fail if name conflict`` () =
    task {
        //Arrange
        let request = RequestFactory.createUserRequest()
        let! _ = UserClient.createUser request
    
        //Act
        let! response = UserClient.createUser request
        
        //Assert
        response |> shouldBeError HttpStatusCode.Conflict "Conflict when attempting to write User."
    } :> Task

[<Test>]
let ``Create user should fail if already signed in`` () =
    task {
        //Arrange
        let! (_, token) = SetupUtility.createUserAndSignIn()
        let request = RequestFactory.createUserRequest()

        //Act
        let! response = UserClient.tryCreateUserWithToken(request, token)

        //Assert
        response |> shouldBeError HttpStatusCode.Unauthorized "Operation not allowed if already signed in."
    } :> Task

[<Test>]
let ``Get user should work`` () =
    task {
        //Arrange
        let request = RequestFactory.createUserRequest()
        let! user = UserClient.createUser request
                    |> AsyncResponse.bodyValue
    
        //Act
        let! response = UserClient.getUser user.id
        
        //Assert
        response |> shouldHaveStatus HttpStatusCode.OK
                
        let responseUser = response.bodyValue
        responseUser.name |> shouldBe request.name
        responseUser.isAdmin |> shouldBe false
    } :> Task

[<Test>]
let ``Get user should fail if user doesnt exist`` () =
    task {
        //Arrange
        let userId = Int32.MinValue
    
        //Act
        let! response = UserClient.getUser userId
        
        //Assert
        response |> shouldBeError HttpStatusCode.NotFound "User not found."
    } :> Task

[<Test>]
let ``Delete user should work`` () =
    task {
        //Arrange
        let request = RequestFactory.createUserRequest()    
        let! user = UserClient.createUser request
                    |> AsyncResponse.bodyValue

        //Act
        let! deleteResponse = UserClient.deleteUser user.id
        let! getResponse = UserClient.getUser user.id
        
        //Assert
        deleteResponse |> shouldHaveStatus HttpStatusCode.OK
        getResponse |> shouldBeError HttpStatusCode.NotFound "User not found."
    } :> Task

[<Test>]
let ``Delete user should fail if already deleted`` () =
    task {
        //Arrange
        let request = RequestFactory.createUserRequest()    
        let! user = UserClient.createUser request
                    |> AsyncResponse.bodyValue

        let! _ = UserClient.deleteUser user.id

        //Act
        let! response = UserClient.deleteUser user.id
        
        //Assert
        response |> shouldBeError HttpStatusCode.NotFound "User not found."
    } :> Task

[<Test>]
let ``Get users should return multiple users`` () =
    task {
        //Arrange
        let request1 = RequestFactory.createUserRequest()
        let request2 = RequestFactory.createUserRequest()

        let! user1 = UserClient.createUser request1
                     |> AsyncResponse.bodyValue
                     
        let! user2 = UserClient.createUser request2
                     |> AsyncResponse.bodyValue
    
        //Act
        let! response = UserClient.getUsers ()
        
        //Assert
        response |> shouldHaveStatus HttpStatusCode.OK
                
        let responseUsers = response.bodyValue
        responseUsers.Length |> shouldBeAtLeast 2
        responseUsers |> shouldExist (fun u -> u.id = user1.id)
        responseUsers |> shouldExist (fun u -> u.id = user2.id)
    } :> Task