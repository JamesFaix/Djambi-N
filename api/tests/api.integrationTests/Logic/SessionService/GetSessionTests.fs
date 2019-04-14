namespace Djambi.Api.IntegrationTests.Logic.Services.Sessions

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.Control
open Djambi.Api.IntegrationTests

type GetSessionTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Get session should work``() =
        task {
            //Arrange
            let userRequest = getCreateUserRequest()
            let! _ = services.users.createUser userRequest None
                     |> AsyncHttpResult.thenValue

            let request = getLoginRequest userRequest

            let! session = services.sessions.openSession request
                           |> AsyncHttpResult.thenValue
            //Act
            let! sessionResponse = services.sessions.getSession session.token
                                  |> AsyncHttpResult.thenValue

            //Assert
            sessionResponse |> shouldBe session
        }

    [<Fact>]
    let ``Get session should fail if token not found``() =
        task {
            //Arrange

            //Act
            let! result = services.sessions.getSession "does not exist"

            //Assert
            result |> shouldBeError 404 "Session not found."
        }

    [<Fact>]
    let ``Get session should fail if session closed``() =
        task {
            //Arrange
            let userRequest = getCreateUserRequest()
            let! _ = services.users.createUser userRequest None
                     |> AsyncHttpResult.thenValue

            let loginRequest = getLoginRequest userRequest
            let! session = services.sessions.openSession loginRequest
                           |> AsyncHttpResult.thenValue

            let! _ = services.sessions.closeSession session

            //Act
            let! result = services.sessions.getSession session.token

            //Assert
            result |> shouldBeError 404 "Session not found."
        }