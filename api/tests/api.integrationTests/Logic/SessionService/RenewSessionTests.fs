namespace Apex.Api.IntegrationTests.Logic.sessionServ

open FSharp.Control.Tasks
open Xunit
open Apex.Api.Common.Control
open Apex.Api.IntegrationTests
open Apex.Api.Logic.Services

type RenewSessionTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Renew session should work`` () =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let userRequest = getCreateUserRequest()
            let! _ = host.Get<UserService>().createUser userRequest None
                     |> AsyncHttpResult.thenValue

            let request = getLoginRequest userRequest

            let! session = host.Get<SessionService>().openSession request
                           |> AsyncHttpResult.thenValue
            //Act
            let! sessionResponse = host.Get<SessionService>().renewSession session.token
                                  |> AsyncHttpResult.thenValue

            //Assert
            sessionResponse.expiresOn |> shouldBeGreaterThan session.expiresOn
            { sessionResponse with expiresOn = session.expiresOn } |> shouldBe session
        }

    [<Fact>]
    let ``Renew session should fail if session does not exist``() =
        let host = HostFactory.createHost()
        task {
            //Arrange

            //Act
            let! result = host.Get<SessionService>().renewSession "does not exist"

            //Assert
            result |> shouldBeError 404 "Session not found."
        }

    [<Fact>]
    let ``Renew session should fail if session closed``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let userRequest = getCreateUserRequest()
            let! _ = host.Get<UserService>().createUser userRequest None
                     |> AsyncHttpResult.thenValue

            let loginRequest = getLoginRequest userRequest
            let! session = host.Get<SessionService>().openSession loginRequest
                           |> AsyncHttpResult.thenValue

            let! _ = host.Get<SessionService>().closeSession session

            //Act
            let! result = host.Get<SessionService>().renewSession session.token

            //Assert
            result |> shouldBeError 404 "Session not found."
        }