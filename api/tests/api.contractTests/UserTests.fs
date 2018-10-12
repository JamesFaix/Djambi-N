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
        Assert.Equal(HttpStatusCode.OK, response.statusCode)
                
        let user = response.result |> Result.value
        Assert.Equal(request.name, user.name)
        Assert.Equal(request.role, user.role)
    }