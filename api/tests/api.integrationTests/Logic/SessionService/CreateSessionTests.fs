namespace Djambi.Api.IntegrationTests.Logic.SessionService

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common
open Djambi.Api.IntegrationTests
open Djambi.Api.Logic.Services
open Djambi.Api.Common.AsyncHttpResult;

type CreateSessionTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Create session should work``() =
        task {
            //Arrange
            let userRequest = getCreateUserRequest()
            let! user = UserService.createUser userRequest None
                        |> thenValue

            let request = getLoginRequest userRequest

            //Act
            let! session = SessionService.openSession request
                           |> thenValue

            //Assert
            session.userId |> shouldBe user.id
            session.id |> shouldNotBe 0
            session.token.Length |> shouldBeAtLeast 1
        }

    [<Fact>]
    let ``Create session should replace existing session if user already has session``() =
        task {
            //Arrange
            let userRequest = getCreateUserRequest()
            let! user = UserService.createUser userRequest None
                        |> thenValue

            let request = getLoginRequest userRequest

            let! oldSession = SessionService.openSession request
                              |> thenValue

            //Act
            let! newSession = SessionService.openSession request |> thenValue

            //Assert
            newSession.token |> shouldNotBe oldSession.token
        }

    [<Fact>]
    let ``Create session should fail if incorrect password``() =
        task {
            //Arrange
            let userRequest = getCreateUserRequest()
            let! _ = UserService.createUser userRequest None
                        |> thenValue

            let request = { getLoginRequest userRequest with password = "wrong" }

            //Act
            let! error = SessionService.openSession request

            //Assert
            error |> shouldBeError 401 "Incorrect password."
        }

    [<Fact>]
    let ``Create session should fail with locked message on 6th incorrect password attempt``() =
        task {
             //Arrange
            let userRequest = getCreateUserRequest()
            let! _ = UserService.createUser userRequest None
                        |> thenValue

            let request = { getLoginRequest userRequest with password = "wrong" }

            let! _ = SessionService.openSession request
            let! _ = SessionService.openSession request
            let! _ = SessionService.openSession request
            let! _ = SessionService.openSession request
            let! _ = SessionService.openSession request

            //Act
            let! error = SessionService.openSession request

            //Assert
            error |> shouldBeError 401 "Account locked."
        }