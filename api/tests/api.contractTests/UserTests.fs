module Djambi.Api.ContractTests.UserTests

open System
open System.Net
open FSharp.Control.Tasks
open Xunit
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
                
        Assert.NotEqual(0, response.value.id)
        Assert.Equal(request.name, response.value.name)
        Assert.Equal(request.role, response.value.role)
    }