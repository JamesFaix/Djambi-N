namespace Apex.Api.IntegrationTests.Logic.sessionServ

open FSharp.Control.Tasks
open Xunit
open Apex.Api.Common.Control
open Apex.Api.IntegrationTests
open Apex.Api.Logic.Services

type GetSessionTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Get session should work``() =
        task {
            //Arrange
            let userRequest = getCreateUserRequest()
            let! _ = Host.get<UserService>().createUser userRequest None
                     |> AsyncHttpResult.thenValue

            let request = getLoginRequest userRequest

            let! session = Host.get<SessionService>().openSession request
                           |> AsyncHttpResult.thenValue
            //Act
            let! sessionResponse = Host.get<SessionService>().getSession session.token
                                  |> AsyncHttpResult.thenValue

            //Assert
            sessionResponse |> shouldBe session
        }

    [<Fact>]
    let ``Get session should fail if token not found``() =
        task {
            //Arrange

            //Act
            let! result = Host.get<SessionService>().getSession "does not exist"

            //Assert
            result |> shouldBeError 404 "Session not found."
        }

    [<Fact>]
    let ``Get session should fail if session closed``() =
        task {
            //Arrange
            let userRequest = getCreateUserRequest()
            let! _ = Host.get<UserService>().createUser userRequest None
                     |> AsyncHttpResult.thenValue

            let loginRequest = getLoginRequest userRequest
            let! session = Host.get<SessionService>().openSession loginRequest
                           |> AsyncHttpResult.thenValue

            let! _ = Host.get<SessionService>().closeSession session

            //Act
            let! result = Host.get<SessionService>().getSession session.token

            //Assert
            result |> shouldBeError 404 "Session not found."
        }