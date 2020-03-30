namespace Apex.Api.IntegrationTests.Logic.sessionServ

open FSharp.Control.Tasks
open Xunit
open Apex.Api.Common.Control
open Apex.Api.IntegrationTests
open Apex.Api.Logic.Services

type CloseSessionTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Close session should work if session exists``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let userRequest = getCreateUserRequest()
            let! _ = host.Get<UserService>().createUser userRequest None
                     |> AsyncHttpResult.thenValue

            let loginRequest = getLoginRequest userRequest
            let! session = host.Get<SessionService>().openSession loginRequest
                           |> AsyncHttpResult.thenValue

            //Act
            let! result = host.Get<SessionService>().closeSession session

            //Assert
            result |> Result.isOk |> shouldBeTrue
        }

    [<Fact>]
    let ``Close session should fail if session does not exist``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let session = getSessionForUser 1

            //Act
            let! result = host.Get<SessionService>().closeSession session

            //Assert
            result |> shouldBeError 404 "Session not found."
        }