namespace Djambi.Api.IntegrationTests.Logic.services.sessions

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.Control
open Djambi.Api.IntegrationTests

type RenewSessionTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Renew session should work`` () =
        task {
            //Arrange
            let userRequest = getCreateUserRequest()
            let! _ = services.users.createUser userRequest None
                     |> AsyncHttpResult.thenValue

            let request = getLoginRequest userRequest

            let! session = services.sessions.openSession request
                           |> AsyncHttpResult.thenValue
            //Act
            let! sessionResponse = services.sessions.renewSession session.token
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
            let! result = services.sessions.renewSession "does not exist"

            //Assert
            result |> shouldBeError 404 "Session not found."
        }

    [<Fact>]
    let ``Renew session should fail if session closed``() =
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
            let! result = services.sessions.renewSession session.token

            //Assert
            result |> shouldBeError 404 "Session not found."
        }