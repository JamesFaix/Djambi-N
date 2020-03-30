namespace Apex.Api.IntegrationTests.Logic.userServ

open FSharp.Control.Tasks
open Xunit
open Apex.Api.Common.Control
open Apex.Api.IntegrationTests
open Apex.Api.Model
open Apex.Api.Enums
open Apex.Api.Logic.Services

type CreateUserTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Create user should work if not signed in``() =
        task {
            //Arrange
            let request = getCreateUserRequest()

            //Act
            let! user = Host.get<UserService>().createUser request None
                        |> AsyncHttpResult.thenValue

            //Assert
            user.id |> shouldNotBe 0
            user.name |> shouldBe request.name
        }

    [<Fact>]
    let ``Create user should fail if name conflict``() =
        task {
            //Arrange
            let request = getCreateUserRequest()
            let! _ = Host.get<UserService>().createUser request None

            //Act
            let! error = Host.get<UserService>().createUser request None

            //Assert
            error |> shouldBeError 409 "Conflict when attempting to write User."
        }

    [<Fact>]
    let ``Create user should fail if signed in and not EditUsers``() =
        task {
            //Arrange
            let request = getCreateUserRequest()
            let session = getSessionForUser 1 |> TestUtilities.setSessionPrivileges []

            //Act
            let! error = Host.get<UserService>().createUser request (Some session)

            //Assert
            error |> shouldBeError 403 "Cannot create user if logged in."
        }

    [<Fact>]
    let ``Create user should work if signed in and EditUsers``() =
        task {
            //Arrange
            let request = getCreateUserRequest()
            let session = getSessionForUser 1 |> TestUtilities.setSessionPrivileges [Privilege.EditUsers]

            //Act
            let! user = Host.get<UserService>().createUser request (Some session)
                        |> AsyncHttpResult.thenValue

            //Assert
            user.id |> shouldNotBe 0
            user.name |> shouldBe request.name
        }