namespace Djambi.Api.IntegrationTests.Logic.UserService

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common
open Djambi.Api.IntegrationTests
open Djambi.Api.Logic.Services

type CreateUserTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Create user should work``() =
        task {
            //Arrange
            let request = getCreateUserRequest()
            
            //Act
            let! user = UserService.createUser request
                        |> AsyncHttpResult.thenValue

            //Assert
            user.id |> shouldNotBe 0
            user.name |> shouldBe request.name
        }

    [<Fact>]
    let ``Create user should fail if namespace conflict``() =
        task {
            //Arrange
            let request = getCreateUserRequest()
            let! _ = UserService.createUser request

            //Act
            let! error = UserService.createUser request

            //Assert
            error |> shouldBeError 409 "Conflict when attempting to write User."
        }

    //TODO: Create user should fail if signed in and not admin
   
    //TODO: Create user should work if signed in and admin