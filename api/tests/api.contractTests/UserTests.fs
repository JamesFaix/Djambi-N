module Djambi.Api.ContractTests.UserTests

open System
open System.Net
open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common
open Djambi.Api.Web.Model.LobbyWebModel
open Djambi.Api.WebClient

[<Fact>]
let ``POST user should work`` () =
    task {
        //Arrange
        let request = 
            {
                name = Guid.NewGuid().ToString()
                password = "test"
                role = "Normal"
            }
    
        //Act
        let! response = UserRepository.createUser request
        
        //Assert
        response |> shouldHaveStatus HttpStatusCode.OK
                
        let user = response.result |> Result.value
        user.name |> shouldBe request.name
        user.role |> shouldBe request.role
    }

[<Fact>]
let ``POST user should fail if name conflict`` () =
    task {
        //Arrange
        let request = 
            {
                name = Guid.NewGuid().ToString()
                password = "test"
                role = "Normal"
            }

        let! _ = UserRepository.createUser request
    
        //Act
        let! response = UserRepository.createUser request
        
        //Assert
        response |> shouldBeError HttpStatusCode.Conflict "Conflict when attempting to write User."
    }

[<Fact>]
let ``GET user should work`` () =
    task {
        //Arrange
        let request = 
            {
                name = Guid.NewGuid().ToString()
                password = "test"
                role = "Normal"
            }

        let! user = UserRepository.createUser request
                    |> Task.map (fun resp -> resp.result |> Result.value)
    
        //Act
        let! response = UserRepository.getUser user.id
        
        //Assert
        response |> shouldHaveStatus HttpStatusCode.OK
                
        let responseUser = response.result |> Result.value
        responseUser.name |> shouldBe request.name
        responseUser.role |> shouldBe request.role
    }

[<Fact>]
let ``GET user should fail if user doesnt exist`` () =
    task {
        //Arrange
        let userId = Int32.MinValue
    
        //Act
        let! response = UserRepository.getUser userId
        
        //Assert
        response |> shouldBeError HttpStatusCode.NotFound "User not found."
    }

[<Fact>]
let ``DELETE user should work`` () =
    task {
        //Arrange
        let request = 
            {
                name = Guid.NewGuid().ToString()
                password = "test"
                role = "Normal"
            }
    
        let! user = UserRepository.createUser request
                    |> Task.map (fun resp -> resp.result |> Result.value)

        //Act
        let! deleteResponse = UserRepository.deleteUser user.id
        let! getResponse = UserRepository.getUser user.id
        
        //Assert
        deleteResponse |> shouldHaveStatus HttpStatusCode.OK
        getResponse |> shouldBeError HttpStatusCode.NotFound "User not found."
    }

[<Fact>]
let ``DELETE user should fail if already deleted`` () =
    task {
        //Arrange
        let request = 
            {
                name = Guid.NewGuid().ToString()
                password = "test"
                role = "Normal"
            }
    
        let! user = UserRepository.createUser request
                    |> Task.map (fun resp -> resp.result |> Result.value)

        let! _ = UserRepository.deleteUser user.id

        //Act
        let! response = UserRepository.deleteUser user.id
        
        //Assert
        response |> shouldBeError HttpStatusCode.NotFound "User not found."
    }