namespace Djambi.Api.IntegrationTests.Logic.SessionService

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.Control
open Djambi.Api.IntegrationTests
open Djambi.Api.Logic.Services

type CloseSessionTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Close session should work if session exists``() =
        task {
            //Arrange
            let userRequest = getCreateUserRequest()
            let! _ = UserService.createUser userRequest None
                     |> AsyncHttpResult.thenValue

            let loginRequest = getLoginRequest userRequest
            let! session = SessionService.openSession loginRequest
                           |> AsyncHttpResult.thenValue

            //Act
            let! result = SessionService.closeSession session

            //Assert
            result |> Result.isOk |> shouldBeTrue
        }

    [<Fact>]
    let ``Close session should fail if session does not exist``() =
        task {
            //Arrange
            let session = getSessionForUser 1

            //Act
            let! result = SessionService.closeSession session

            //Assert
            result |> shouldBeError 404 "Session not found."
        }