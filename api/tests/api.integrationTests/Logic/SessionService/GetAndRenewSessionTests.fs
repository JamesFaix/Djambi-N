namespace Djambi.Api.IntegrationTests.Logic.sessionServ

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.IntegrationTests
open Djambi.Api.Logic.Interfaces

type GetAndRenewSessionTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Get and renew session should work``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let userRequest = getCreateUserRequest()
            let! _ = host.Get<IUserManager>().createUser userRequest None

            let request = getLoginRequest userRequest

            let! session = host.Get<ISessionService>().openSession request

            //Act
            let! sessionResponse = host.Get<ISessionService>().getAndRenewSession session.token

            //Assert
            sessionResponse.IsSome |> shouldBe true
            let s = sessionResponse.Value

            s.id |> shouldBe session.id
            s.token |> shouldBe session.token
            s.createdOn |> shouldBe session.createdOn
            s.user |> shouldBe session.user
            s.expiresOn |> shouldBeGreaterThan session.expiresOn
        }

    [<Fact>]
    let ``Get and renew session should return None if token not found``() =
        let host = HostFactory.createHost()
        task {
            let! sessionResponse = host.Get<ISessionService>().getAndRenewSession "invalid token string"

            sessionResponse.IsNone |> shouldBe true
        }

    [<Fact>]
    let ``Get and renew session should return None if session closed``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let userRequest = getCreateUserRequest()
            let! _ = host.Get<IUserManager>().createUser userRequest None

            let loginRequest = getLoginRequest userRequest
            let! session = host.Get<ISessionService>().openSession loginRequest

            let! _ = host.Get<ISessionService>().closeSession session

            //Act
            let! sessionResponse = host.Get<ISessionService>().getAndRenewSession session.token
            
            //Assert
            sessionResponse.IsNone |> shouldBe true
        }