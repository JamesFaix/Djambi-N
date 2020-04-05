namespace Apex.Api.IntegrationTests.Logic.sessionServ

open FSharp.Control.Tasks
open Xunit
open Apex.Api.Common.Control
open Apex.Api.IntegrationTests
open Apex.Api.Logic.Services
open Apex.Api.Logic.Interfaces
open System.Threading.Tasks

type CloseSessionTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Close session should work if session exists``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let userRequest = getCreateUserRequest()
            let! _ = host.Get<IUserManager>().createUser userRequest None

            let loginRequest = getLoginRequest userRequest
            let! session = host.Get<SessionService>().openSession loginRequest

            //Act/Assert
            let! _ = host.Get<SessionService>().closeSession session
            // Did not throw

            return ()
        }

    [<Fact>]
    let ``Close session should fail if session does not exist``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! user = createUser()
            let session = getSessionForUser user

            //Act/Assert
            let! ex = Assert.ThrowsAsync<HttpException>(fun () -> 
                task {
                    let! _ = host.Get<SessionService>().closeSession session
                    return ()
                } :> Task
            )

            ex.statusCode |> shouldBe 404
            ex.Message |> shouldBe "Session not found."
        }