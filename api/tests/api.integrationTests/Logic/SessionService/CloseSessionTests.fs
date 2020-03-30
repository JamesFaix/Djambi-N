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
        task {
            //Arrange
            let userRequest = getCreateUserRequest()
            let! _ = Host.get<UserService>().createUser userRequest None
                     |> AsyncHttpResult.thenValue

            let loginRequest = getLoginRequest userRequest
            let! session = Host.get<SessionService>().openSession loginRequest
                           |> AsyncHttpResult.thenValue

            //Act
            let! result = Host.get<SessionService>().closeSession session

            //Assert
            result |> Result.isOk |> shouldBeTrue
        }

    [<Fact>]
    let ``Close session should fail if session does not exist``() =
        task {
            //Arrange
            let session = getSessionForUser 1

            //Act
            let! result = Host.get<SessionService>().closeSession session

            //Assert
            result |> shouldBeError 404 "Session not found."
        }