namespace Djambi.Api.IntegrationTests.Logic.UserService

open System
open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.Control
open Djambi.Api.IntegrationTests
open Djambi.Api.Logic.Services

type GetUserTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Get user should work if admin`` () =
        task {
            //Arrange
            let request = getCreateUserRequest()
            let! user = UserService.createUser request None
                        |> AsyncHttpResult.thenValue

            let session = { getSessionForUser 1 with isAdmin = true }

            //Act
            let! userResponse = UserService.getUser user.id session
                                |> AsyncHttpResult.thenValue

            //Assert
            userResponse.id |> shouldBe user.id
            userResponse.name |> shouldBe user.name
        }

    [<Fact>]
    let ``Get user should fail if not admin`` () =
        task {
            //Arrange
            let request = getCreateUserRequest()
            let! user = UserService.createUser request None
                        |> AsyncHttpResult.thenValue

            let session = { getSessionForUser 1 with isAdmin = false }

            //Act
            let! error = UserService.getUser user.id session

            //Assert
            error |> shouldBeError 403 SecurityService.notAdminOrSelfErrorMessage
        }

    [<Fact>]
    let ``Get user should fail is user doesn't exist`` () =
        task {
            //Arrange
            let request = getCreateUserRequest()
            let session = { getSessionForUser 1 with isAdmin = true }

            //Act
            let! error = UserService.getUser Int32.MinValue session

            //Assert
            error |> shouldBeError 404 "User not found."
        }