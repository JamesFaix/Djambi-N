namespace Djambi.Api.IntegrationTests.Logic.SessionService

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.Control
open Djambi.Api.IntegrationTests
open Djambi.Api.Logic.Services

type GetSessionTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Get session should work``() =
        task {
            //Arrange
            let userRequest = getCreateUserRequest()
            let! _ = UserService.createUser userRequest None
                     |> AsyncHttpResult.thenValue

            let request = getLoginRequest userRequest

            let! session = SessionService.openSession request
                           |> AsyncHttpResult.thenValue
            //Act
            let! sessionResponse = SessionService.getSession session.token
                                  |> AsyncHttpResult.thenValue

            //Assert
            sessionResponse |> shouldBe session
        }

    [<Fact>]
    let ``Get session should fail if token not found``() =
        task {
            //Arrange

            //Act
            let! result = SessionService.getSession "does not exist"

            //Assert
            result |> shouldBeError 404 "Session not found."
        }

    [<Fact>]
    let ``Get session should fail if session closed``() =
        task {
            //Arrange
            let userRequest = getCreateUserRequest()
            let! _ = UserService.createUser userRequest None
                     |> AsyncHttpResult.thenValue

            let loginRequest = getLoginRequest userRequest
            let! session = SessionService.openSession loginRequest
                           |> AsyncHttpResult.thenValue

            let! _ = SessionService.closeSession session

            //Act
            let! result = SessionService.getSession session.token

            //Assert
            result |> shouldBeError 404 "Session not found."
        }