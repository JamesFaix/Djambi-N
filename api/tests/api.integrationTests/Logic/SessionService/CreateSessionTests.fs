namespace Apex.Api.IntegrationTests.Logic.sessionServ

open FSharp.Control.Tasks
open Xunit
open Apex.Api.IntegrationTests
open Apex.Api.Logic.Services
open Apex.Api.Logic.Interfaces
open Apex.Api.Common.Control
open System.Threading.Tasks
open Apex.Api.Model

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

            //Act
            let! newSession = host.Get<SessionService>().openSession request

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
            let! ex = Assert.ThrowsAsync<HttpException>(fun () ->
                task {
                    return! host.Get<SessionService>().openSession request
                } :> Task
            )

            ex.statusCode |> shouldBe 401
            ex.Message |> shouldBe "Incorrect password."
        }

    [<Fact>]
    let ``Create session should fail with locked message on 6th incorrect password attempt``() =
        let host = HostFactory.createHost()

        let attemptLoginAndIgnoreInvalidPasswordError (request : LoginRequest) =
            task {
                try 
                    let! _ = host.Get<SessionService>().openSession request
                    return ()
                with 
                | :? HttpException as ex when ex.Message = "Incorrect password." ->
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
            let! ex = Assert.ThrowsAsync<HttpException>(fun () ->
                task {
                    return! host.Get<SessionService>().openSession request
                } :> Task
            )

            ex.statusCode |> shouldBe 401
            ex.Message |> shouldBe "Account locked."
        }