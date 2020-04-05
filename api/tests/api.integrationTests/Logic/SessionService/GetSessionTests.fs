namespace Apex.Api.IntegrationTests.Logic.sessionServ

open FSharp.Control.Tasks
open Xunit
open Apex.Api.Common.Control
open Apex.Api.IntegrationTests
open Apex.Api.Logic.Services
open Apex.Api.Logic.Interfaces

type GetSessionTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Get session should work``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let userRequest = getCreateUserRequest()
            let! _ = host.Get<IUserManager>().createUser userRequest None

            let request = getLoginRequest userRequest

            let! session = host.Get<SessionService>().openSession request
                           |> AsyncHttpResult.thenValue
            //Act
            let! sessionResponse = host.Get<SessionService>().getSession session.token
                                  |> AsyncHttpResult.thenValue

            //Assert
            sessionResponse |> shouldBe session
        }

    [<Fact>]
    let ``Get session should fail if token not found``() =
        let host = HostFactory.createHost()
        task {
            //Arrange

            //Act
            let! result = host.Get<SessionService>().getSession "does not exist"

            //Assert
            result |> shouldBeError 404 "Session not found."
        }

    [<Fact>]
    let ``Get session should fail if session closed``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let userRequest = getCreateUserRequest()
            let! _ = host.Get<IUserManager>().createUser userRequest None

            let loginRequest = getLoginRequest userRequest
            let! session = host.Get<SessionService>().openSession loginRequest
                           |> AsyncHttpResult.thenValue

            let! _ = host.Get<SessionService>().closeSession session

            //Act
            let! result = host.Get<SessionService>().getSession session.token

            //Assert
            result |> shouldBeError 404 "Session not found."
        }