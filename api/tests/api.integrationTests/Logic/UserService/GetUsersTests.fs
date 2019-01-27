namespace Djambi.Api.IntegrationTests.Logic.UserService

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.Control
open Djambi.Api.IntegrationTests
open Djambi.Api.Logic.Services

type GetUsersTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Get users should work if admin`` () =
        task {
            //Arrange
            let! user1 = UserService.createUser (getCreateUserRequest()) None
                         |> AsyncHttpResult.thenValue
            let! user2 = UserService.createUser (getCreateUserRequest()) None
                         |> AsyncHttpResult.thenValue

            let session = getSessionForUser 1 |> TestUtilities.setSessionIsAdmin true

            //Act
            let! usersResponse = UserService.getUsers session
                                 |> AsyncHttpResult.thenValue

            //Assert
            usersResponse |> shouldExist (fun u -> u.id = user1.id)
            usersResponse |> shouldExist (fun u -> u.id = user2.id)
        }

    [<Fact>]
    let ``Get users should fail if not admin`` () =
        task {
            //Arrange
            let session = getSessionForUser 1 |> TestUtilities.setSessionIsAdmin false

            //Act
            let! error = UserService.getUsers session

            //Assert
            error |> shouldBeError 403 SecurityService.notAdminErrorMessage
        }
