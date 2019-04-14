namespace Djambi.Api.IntegrationTests.Logic.Services.Sessions

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.IntegrationTests
open Djambi.Api.Common.Control.AsyncHttpResult;

type CreateSessionTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Create session should work``() =
        task {
            //Arrange
            let userRequest = getCreateUserRequest()
            let! user = services.users.createUser userRequest None
                        |> thenValue

            let request = getLoginRequest userRequest

            //Act
            let! session = services.sessions.openSession request
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
            let! user = services.users.createUser userRequest None
                        |> thenValue

            let request = getLoginRequest userRequest

            let! oldSession = services.sessions.openSession request
                              |> thenValue

            //Act
            let! newSession = services.sessions.openSession request |> thenValue

            //Assert
            newSession.token |> shouldNotBe oldSession.token
        }

    [<Fact>]
    let ``Create session should fail if incorrect password``() =
        task {
            //Arrange
            let userRequest = getCreateUserRequest()
            let! _ = services.users.createUser userRequest None
                        |> thenValue

            let request = { getLoginRequest userRequest with password = "wrong" }

            //Act
            let! error = services.sessions.openSession request

            //Assert
            error |> shouldBeError 401 "Incorrect password."
        }

    [<Fact>]
    let ``Create session should fail with locked message on 6th incorrect password attempt``() =
        task {
             //Arrange
            let userRequest = getCreateUserRequest()
            let! _ = services.users.createUser userRequest None
                        |> thenValue

            let request = { getLoginRequest userRequest with password = "wrong" }

            let! _ = services.sessions.openSession request
            let! _ = services.sessions.openSession request
            let! _ = services.sessions.openSession request
            let! _ = services.sessions.openSession request
            let! _ = services.sessions.openSession request

            //Act
            let! error = services.sessions.openSession request

            //Assert
            error |> shouldBeError 401 "Account locked."
        }