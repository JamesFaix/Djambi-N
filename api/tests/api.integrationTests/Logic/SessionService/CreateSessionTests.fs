namespace Apex.Api.IntegrationTests.Logic.sessionServ

open FSharp.Control.Tasks
open Xunit
open Apex.Api.IntegrationTests
open Apex.Api.Common.Control.AsyncHttpResult;

type CreateSessionTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Create session should work``() =
        task {
            //Arrange
            let userRequest = getCreateUserRequest()
            let! user = userServ.createUser userRequest None
                        |> thenValue

            let request = getLoginRequest userRequest

            //Act
            let! session = sessionServ.openSession request
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
            let! user = userServ.createUser userRequest None
                        |> thenValue

            let request = getLoginRequest userRequest

            let! oldSession = sessionServ.openSession request
                              |> thenValue

            //Act
            let! newSession = sessionServ.openSession request |> thenValue

            //Assert
            newSession.token |> shouldNotBe oldSession.token
        }

    [<Fact>]
    let ``Create session should fail if incorrect password``() =
        task {
            //Arrange
            let userRequest = getCreateUserRequest()
            let! _ = userServ.createUser userRequest None
                        |> thenValue

            let request = { getLoginRequest userRequest with password = "wrong" }

            //Act
            let! error = sessionServ.openSession request

            //Assert
            error |> shouldBeError 401 "Incorrect password."
        }

    [<Fact>]
    let ``Create session should fail with locked message on 6th incorrect password attempt``() =
        task {
             //Arrange
            let userRequest = getCreateUserRequest()
            let! _ = userServ.createUser userRequest None
                        |> thenValue

            let request = { getLoginRequest userRequest with password = "wrong" }

            let! _ = sessionServ.openSession request
            let! _ = sessionServ.openSession request
            let! _ = sessionServ.openSession request
            let! _ = sessionServ.openSession request
            let! _ = sessionServ.openSession request

            //Act
            let! error = sessionServ.openSession request

            //Assert
            error |> shouldBeError 401 "Account locked."
        }