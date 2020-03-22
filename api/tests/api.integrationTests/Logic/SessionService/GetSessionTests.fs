namespace Apex.Api.IntegrationTests.Logic.sessionServ

open FSharp.Control.Tasks
open Xunit
open Apex.Api.Common.Control
open Apex.Api.IntegrationTests

type GetSessionTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Get session should work``() =
        task {
            //Arrange
            let userRequest = getCreateUserRequest()
            let! _ = userServ.createUser userRequest None
                     |> AsyncHttpResult.thenValue

            let request = getLoginRequest userRequest

            let! session = sessionServ.openSession request
                           |> AsyncHttpResult.thenValue
            //Act
            let! sessionResponse = sessionServ.getSession session.token
                                  |> AsyncHttpResult.thenValue

            //Assert
            sessionResponse |> shouldBe session
        }

    [<Fact>]
    let ``Get session should fail if token not found``() =
        task {
            //Arrange

            //Act
            let! result = sessionServ.getSession "does not exist"

            //Assert
            result |> shouldBeError 404 "Session not found."
        }

    [<Fact>]
    let ``Get session should fail if session closed``() =
        task {
            //Arrange
            let userRequest = getCreateUserRequest()
            let! _ = userServ.createUser userRequest None
                     |> AsyncHttpResult.thenValue

            let loginRequest = getLoginRequest userRequest
            let! session = sessionServ.openSession loginRequest
                           |> AsyncHttpResult.thenValue

            let! _ = sessionServ.closeSession session

            //Act
            let! result = sessionServ.getSession session.token

            //Assert
            result |> shouldBeError 404 "Session not found."
        }