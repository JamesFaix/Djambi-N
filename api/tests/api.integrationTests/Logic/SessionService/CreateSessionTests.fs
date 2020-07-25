namespace Apex.Api.IntegrationTests.Logic.sessionServ

open FSharp.Control.Tasks
open Xunit
open Apex.Api.IntegrationTests
open Apex.Api.Logic.Interfaces
open System.Threading.Tasks
open Apex.Api.Model
open System.Security.Authentication

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
            let! session = host.Get<ISessionService>().openSession request

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

            let! oldSession = host.Get<ISessionService>().openSession request

            //Act
            let! newSession = host.Get<ISessionService>().openSession request

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

            //Act/Assert
            let! ex = Assert.ThrowsAsync<AuthenticationException>(fun () ->
                task {
                    return! host.Get<ISessionService>().openSession request
                } :> Task
            )

            ex.Message |> shouldBe "Incorrect password."
        }

    [<Fact>]
    let ``Create session should fail with locked message on 6th incorrect password attempt``() =
        let host = HostFactory.createHost()

        let attemptLoginAndIgnoreInvalidPasswordError (request : LoginRequest) =
            task {
                try 
                    let! _ = host.Get<ISessionService>().openSession request
                    return ()
                with 
                | :? AuthenticationException as ex when ex.Message = "Incorrect password." ->
                    return ()
            }

        task {
             //Arrange
            let userRequest = getCreateUserRequest()
            let! _ = host.Get<IUserManager>().createUser userRequest None

            let request = { getLoginRequest userRequest with password = "wrong" }

            for _ in 1..5 do
                let! _ = attemptLoginAndIgnoreInvalidPasswordError request
                ()

            //Act/Assert
            let! ex = Assert.ThrowsAsync<AuthenticationException>(fun () ->
                task {
                    return! host.Get<ISessionService>().openSession request
                } :> Task
            )

            ex.Message |> shouldBe "Account locked."
        }