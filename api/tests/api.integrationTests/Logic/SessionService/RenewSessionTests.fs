namespace Djambi.Api.IntegrationTests.Logic.SessionService

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common
open Djambi.Api.IntegrationTests
open Djambi.Api.Logic.Services

type RenewSessionTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Renew session should work`` () =
        task {
            //Arrange
            let userRequest = getCreateUserRequest()
            let! _ = UserService.createUser userRequest None
                     |> AsyncHttpResult.thenValue

            let request = getLoginRequest userRequest

            let! session = SessionService.openSession request
                           |> AsyncHttpResult.thenValue
            //Act
            let! sessionResponse = SessionService.renewSession session.token
                                  |> AsyncHttpResult.thenValue

            //Assert
            sessionResponse.expiresOn |> shouldBeGreaterThan session.expiresOn
            { sessionResponse with expiresOn = session.expiresOn } |> shouldBe session
        }

    [<Fact>]
    let ``Renew session should fail if session does not exist``() =
        task {
            //Arrange

            //Act
            let! result = SessionService.renewSession "does not exist"

            //Assert
            result |> shouldBeError 404 "Session not found."
        }

    [<Fact>]
    let ``Renew session should fail if session closed``() =
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
            let! result = SessionService.renewSession session.token

            //Assert
            result |> shouldBeError 404 "Session not found."
        }