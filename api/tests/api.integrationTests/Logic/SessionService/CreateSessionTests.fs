namespace Apex.Api.IntegrationTests.Logic.sessionServ

open FSharp.Control.Tasks
open Xunit
open Apex.Api.IntegrationTests
open Apex.Api.Common.Control.AsyncHttpResult;
open Apex.Api.Logic.Services
open Apex.Api.Logic.Interfaces

type CreateSessionTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Create session should work``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let userRequest = getCreateUserRequest()
            let! user = host.Get<IUserManager>().createUser userRequest None

            let request = getLoginRequest userRequest

            //Act
            let! session = host.Get<SessionService>().openSession request
                           |> thenValue

            //Assert
            session.user.id |> shouldBe user.id
            session.id |> shouldNotBe 0
            session.token.Length |> shouldBeAtLeast 1
        }

    [<Fact>]
    let ``Create session should replace existing session if user already has session``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let userRequest = getCreateUserRequest()
            let! user = host.Get<IUserManager>().createUser userRequest None

            let request = getLoginRequest userRequest

            let! oldSession = host.Get<SessionService>().openSession request
                              |> thenValue

            //Act
            let! newSession = host.Get<SessionService>().openSession request |> thenValue

            //Assert
            newSession.token |> shouldNotBe oldSession.token
        }

    [<Fact>]
    let ``Create session should fail if incorrect password``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let userRequest = getCreateUserRequest()
            let! _ = host.Get<IUserManager>().createUser userRequest None

            let request = { getLoginRequest userRequest with password = "wrong" }

            //Act
            let! error = host.Get<SessionService>().openSession request

            //Assert
            error |> shouldBeError 401 "Incorrect password."
        }

    [<Fact>]
    let ``Create session should fail with locked message on 6th incorrect password attempt``() =
        let host = HostFactory.createHost()
        task {
             //Arrange
            let userRequest = getCreateUserRequest()
            let! _ = host.Get<IUserManager>().createUser userRequest None

            let request = { getLoginRequest userRequest with password = "wrong" }

            let! _ = host.Get<SessionService>().openSession request
            let! _ = host.Get<SessionService>().openSession request
            let! _ = host.Get<SessionService>().openSession request
            let! _ = host.Get<SessionService>().openSession request
            let! _ = host.Get<SessionService>().openSession request

            //Act
            let! error = host.Get<SessionService>().openSession request

            //Assert
            error |> shouldBeError 401 "Account locked."
        }