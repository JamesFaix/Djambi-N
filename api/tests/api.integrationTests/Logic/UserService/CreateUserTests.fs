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
        let host = HostFactory.createHost()
        task {
            //Arrange
            let request = getCreateUserRequest()

            //Act
            let! user = host.Get<UserService>().createUser request None
                        |> AsyncHttpResult.thenValue

            //Assert
            user.id |> shouldNotBe 0
            user.name |> shouldBe request.name
        }

    [<Fact>]
    let ``Create user should fail if name conflict``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let request = getCreateUserRequest()
            let! _ = host.Get<UserService>().createUser request None

            //Act
            let! error = host.Get<UserService>().createUser request None

            //Assert
            error |> shouldBeError 409 "Conflict when attempting to write User."
        }

    [<Fact>]
    let ``Create user should fail if signed in and not EditUsers``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! currentUser = createUser() |> AsyncHttpResult.thenValue
            let request = getCreateUserRequest()
            let session = getSessionForUser (currentUser |> UserDetails.hideDetails) |> TestUtilities.setSessionPrivileges []

            //Act
            let! error = host.Get<UserService>().createUser request (Some session)

            //Assert
            error |> shouldBeError 403 "Cannot create user if logged in."
        }

    [<Fact>]
    let ``Create user should work if signed in and EditUsers``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! currentUser = createUser() |> AsyncHttpResult.thenValue
            let request = getCreateUserRequest()
            let session = getSessionForUser (currentUser |> UserDetails.hideDetails) |> TestUtilities.setSessionPrivileges [Privilege.EditUsers]

            //Act
            let! user = host.Get<UserService>().createUser request (Some session)
                        |> AsyncHttpResult.thenValue

            //Assert
            user.id |> shouldNotBe 0
            user.name |> shouldBe request.name
        }