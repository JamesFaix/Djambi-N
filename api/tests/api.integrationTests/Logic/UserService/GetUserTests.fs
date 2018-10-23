namespace Djambi.Api.IntegrationTests.Logic.UserService

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common
open Djambi.Api.IntegrationTests
open Djambi.Api.Logic.Services

type GetUserTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Get user should work if admin`` () =
        task {
            //Arrange
            let request = getCreateUserRequest()
            let! user = UserService.createUser request
                        |> AsyncHttpResult.thenValue
                        
            let session = { getSessionForUser (user.id + 1) with isAdmin = true }

            //Act
            let! userResponse = UserService.getUser user.id session
                                |> AsyncHttpResult.thenValue

            //Assert
            userResponse.id |> shouldBe user.id
            userResponse.name |> shouldBe user.name
        }        

    //Get user should work if admin

    //Get user should fail if not admin

    //Get user should fail if user doesn't exist