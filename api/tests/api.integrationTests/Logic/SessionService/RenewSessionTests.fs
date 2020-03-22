namespace Apex.Api.IntegrationTests.Logic.sessionServ

open FSharp.Control.Tasks
open Xunit
open Apex.Api.Common.Control
open Apex.Api.IntegrationTests

type RenewSessionTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Renew session should work`` () =
        task {
            //Arrange
            let userRequest = getCreateUserRequest()
            let! _ = userServ.createUser userRequest None
                     |> AsyncHttpResult.thenValue

            let request = getLoginRequest userRequest

            let! session = sessionServ.openSession request
                           |> AsyncHttpResult.thenValue
            //Act
            let! sessionResponse = sessionServ.renewSession session.token
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
            let! result = sessionServ.renewSession "does not exist"

            //Assert
            result |> shouldBeError 404 "Session not found."
        }

    [<Fact>]
    let ``Renew session should fail if session closed``() =
        task {
            //Arrange
            let userRequest = getCreateUserRequest()
            let! _ = userServ.createUser userRequest None
                     |> AsyncHttpResult.thenValue

            let loginRequest = getLoginRequest userRequest
            let! session = sessionServ.openSession loginRequest
                           |> AsyncHttpResult.thenValue

            let! _ = sessionServ.closeSession session

            //Act
            let! result = sessionServ.renewSession session.token

            //Assert
            result |> shouldBeError 404 "Session not found."
        }