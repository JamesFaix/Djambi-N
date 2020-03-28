namespace Apex.Api.IntegrationTests.Logic.userServ

open System
open FSharp.Control.Tasks
open Xunit
open Apex.Api.Common.Control
open Apex.Api.IntegrationTests
open Apex.Api.Model
open Apex.Api.Logic
open Apex.Api.Enums

type GetUserTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Get user should work if EditUsers`` () =
        task {
            //Arrange
            let request = getCreateUserRequest()
            let! user = userServ.createUser request None
                        |> AsyncHttpResult.thenValue

            let session = getSessionForUser 1 |> TestUtilities.setSessionPrivileges [Privilege.EditUsers]

            //Act
            let! userResponse = userServ.getUser user.id session
                                |> AsyncHttpResult.thenValue

            //Assert
            userResponse.id |> shouldBe user.id
            userResponse.name |> shouldBe user.name
        }

    [<Fact>]
    let ``Get user should fail if not EditUsers`` () =
        task {
            //Arrange
            let request = getCreateUserRequest()
            let! user = userServ.createUser request None
                        |> AsyncHttpResult.thenValue

            let session = getSessionForUser 1 |> TestUtilities.setSessionPrivileges []

            //Act
            let! error = userServ.getUser user.id session

            //Assert
            error |> shouldBeError 403 Security.noPrivilegeOrSelfErrorMessage
        }

    [<Fact>]
    let ``Get user should fail is user doesn't exist`` () =
        task {
            //Arrange
            let request = getCreateUserRequest()
            let session = getSessionForUser 1 |> TestUtilities.setSessionPrivileges [Privilege.EditUsers]

            //Act
            let! error = userServ.getUser Int32.MinValue session

            //Assert
            error |> shouldBeError 404 "User not found."
        }