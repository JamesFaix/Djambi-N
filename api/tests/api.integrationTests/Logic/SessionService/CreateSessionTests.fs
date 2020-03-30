namespace Apex.Api.IntegrationTests.Logic.sessionServ

open FSharp.Control.Tasks
open Xunit
open Apex.Api.IntegrationTests
open Apex.Api.Common.Control.AsyncHttpResult;
open Apex.Api.Logic.Services

type CreateSessionTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Create session should work``() =
        task {
            //Arrange
            let userRequest = getCreateUserRequest()
            let! user = Host.get<UserService>().createUser userRequest None
                        |> thenValue

            let request = getLoginRequest userRequest

            //Act
            let! session = Host.get<SessionService>().openSession request
                           |> thenValue

            //Assert
            session.user.id |> shouldBe user.id
            session.id |> shouldNotBe 0
            session.token.Length |> shouldBeAtLeast 1
        }

    [<Fact>]
    let ``Create session should replace existing session if user already has session``() =
        task {
            //Arrange
            let userRequest = getCreateUserRequest()
            let! user = Host.get<UserService>().createUser userRequest None
                        |> thenValue

            let request = getLoginRequest userRequest

            let! oldSession = Host.get<SessionService>().openSession request
                              |> thenValue

            //Act
            let! newSession = Host.get<SessionService>().openSession request |> thenValue

            //Assert
            newSession.token |> shouldNotBe oldSession.token
        }

    [<Fact>]
    let ``Create session should fail if incorrect password``() =
        task {
            //Arrange
            let userRequest = getCreateUserRequest()
            let! _ = Host.get<UserService>().createUser userRequest None
                        |> thenValue

            let request = { getLoginRequest userRequest with password = "wrong" }

            //Act
            let! error = Host.get<SessionService>().openSession request

            //Assert
            error |> shouldBeError 401 "Incorrect password."
        }

    [<Fact>]
    let ``Create session should fail with locked message on 6th incorrect password attempt``() =
        task {
             //Arrange
            let userRequest = getCreateUserRequest()
            let! _ = Host.get<UserService>().createUser userRequest None
                        |> thenValue

            let request = { getLoginRequest userRequest with password = "wrong" }

            let! _ = Host.get<SessionService>().openSession request
            let! _ = Host.get<SessionService>().openSession request
            let! _ = Host.get<SessionService>().openSession request
            let! _ = Host.get<SessionService>().openSession request
            let! _ = Host.get<SessionService>().openSession request

            //Act
            let! error = Host.get<SessionService>().openSession request

            //Assert
            error |> shouldBeError 401 "Account locked."
        }