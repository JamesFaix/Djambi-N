namespace Apex.Api.IntegrationTests.Logic.Services.Users

open System
open FSharp.Control.Tasks
open Xunit
open Apex.Api.Common.Control
open Apex.Api.IntegrationTests
open Apex.Api.Model
open Apex.Api.Logic

type GetUserTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Get user should work if EditUsers`` () =
        task {
            //Arrange
            let request = getCreateUserRequest()
            let! user = services.users.createUser request None
                        |> AsyncHttpResult.thenValue

            let session = getSessionForUser 1 |> TestUtilities.setSessionPrivileges [EditUsers]

            //Act
            let! userResponse = services.users.getUser user.id session
                                |> AsyncHttpResult.thenValue

            //Assert
            userResponse.id |> shouldBe user.id
            userResponse.name |> shouldBe user.name
        }

    [<Fact>]
    let ``Get user should fail if not EditUsers`` () =
        task {
            //Arrange
            let request = getCreateUserRequest()
            let! user = services.users.createUser request None
                        |> AsyncHttpResult.thenValue

            let session = getSessionForUser 1 |> TestUtilities.setSessionPrivileges []

            //Act
            let! error = services.users.getUser user.id session

            //Assert
            error |> shouldBeError 403 Security.noPrivilegeOrSelfErrorMessage
        }

    [<Fact>]
    let ``Get user should fail is user doesn't exist`` () =
        task {
            //Arrange
            let request = getCreateUserRequest()
            let session = getSessionForUser 1 |> TestUtilities.setSessionPrivileges [EditUsers]

            //Act
            let! error = services.users.getUser Int32.MinValue session

            //Assert
            error |> shouldBeError 404 "User not found."
        }