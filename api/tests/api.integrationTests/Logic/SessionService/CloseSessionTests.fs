namespace Djambi.Api.IntegrationTests.Logic.sessionServ

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.IntegrationTests
open Djambi.Api.Logic.Interfaces

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
            let! session = host.Get<ISessionService>().openSession loginRequest

            //Act/Assert
            let! _ = host.Get<ISessionService>().closeSession session
            // Did not throw

            return ()
        }

    [<Fact>]
    let ``Close session should not fail if session does not exist``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! user = createUser()
            let session = getSessionForUser user

            //Act/Assert
            return! host.Get<ISessionService>().closeSession session
            // Did not throw
        }