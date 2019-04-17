namespace Djambi.Api.IntegrationTests.Logic.Services.Users

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.Control
open Djambi.Api.IntegrationTests
open Djambi.Api.Model
open Djambi.Api.Logic

type DeleteUserTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Delete user should work if deleting self`` =
        task {
            //Arrange
            let request = getCreateUserRequest()
            let! user = services.users.createUser request None
                        |> AsyncHttpResult.thenValue

            let session = getSessionForUser user.id |> TestUtilities.setSessionPrivileges []

            //Act
            let! response = services.users.deleteUser user.id session

            //Assert
            response |> Result.isOk |> shouldBeTrue
        }

    [<Fact>]
    let ``Delete user should work if EditUsers and deleting other user`` () =
        task {
            //Arrange
            let request = getCreateUserRequest()
            let! user = services.users.createUser request None
                        |> AsyncHttpResult.thenValue

            let session = getSessionForUser (user.id + 1) |> TestUtilities.setSessionPrivileges [EditUsers]

            //Act
            let! response = services.users.deleteUser user.id session

            //Assert
            response |> Result.isOk |> shouldBeTrue
        }

    [<Fact>]
    let ``Delete user should fail if not EditUsers and deleting other user`` () =
        task {
            //Arrange
            let request = getCreateUserRequest()
            let! user = services.users.createUser request None
                        |> AsyncHttpResult.thenValue

            let session = getSessionForUser (user.id + 1) |> TestUtilities.setSessionPrivileges []

            //Act
            let! response = services.users.deleteUser user.id session

            //Assert
            response |> shouldBeError 403 Security.noPrivilegeOrSelfErrorMessage
        }

    [<Fact>]
    let ``Delete user should fail if already deleted`` () =
        task {
            //Arrange
            let request = getCreateUserRequest()
            let! user = services.users.createUser request None
                        |> AsyncHttpResult.thenValue

            let session = getSessionForUser 1 |> TestUtilities.setSessionPrivileges [EditUsers]

            let! _ = services.users.deleteUser user.id session

            //Act
            let! response = services.users.deleteUser user.id session

            //Assert
            response |> shouldBeError 404 "User not found."
        }