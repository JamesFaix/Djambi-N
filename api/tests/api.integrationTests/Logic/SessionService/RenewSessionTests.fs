namespace Apex.Api.IntegrationTests.Logic.sessionServ

open FSharp.Control.Tasks
open Xunit
open Apex.Api.Common.Control
open Apex.Api.IntegrationTests
open Apex.Api.Logic.Services
open Apex.Api.Logic.Interfaces
open System.Threading.Tasks

type RenewSessionTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Renew session should work`` () =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let userRequest = getCreateUserRequest()
            let! _ = host.Get<IUserManager>().createUser userRequest None

            let request = getLoginRequest userRequest

            let! session = host.Get<SessionService>().openSession request

            //Act
            let! sessionResponse = host.Get<SessionService>().renewSession session.token

            //Assert
            sessionResponse.expiresOn |> shouldBeGreaterThan session.expiresOn
            { sessionResponse with expiresOn = session.expiresOn } |> shouldBe session
        }

    [<Fact>]
    let ``Renew session should fail if session does not exist``() =
        let host = HostFactory.createHost()
        task {
            let! ex = Assert.ThrowsAsync<HttpException>(fun () -> 
                task {
                    return! host.Get<SessionService>().renewSession "invalid token string"
                } :> Task
            )

            ex.statusCode |> shouldBe 404
            ex.Message |> shouldBe "Session not found."
        }

    [<Fact>]
    let ``Renew session should fail if session closed``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let userRequest = getCreateUserRequest()
            let! _ = host.Get<IUserManager>().createUser userRequest None

            let loginRequest = getLoginRequest userRequest
            let! session = host.Get<SessionService>().openSession loginRequest

            let! _ = host.Get<SessionService>().closeSession session

            let! ex = Assert.ThrowsAsync<HttpException>(fun () -> 
                task {
                    return! host.Get<SessionService>().renewSession session.token
                } :> Task
            )

            ex.statusCode |> shouldBe 404
            ex.Message |> shouldBe "Session not found."
        }