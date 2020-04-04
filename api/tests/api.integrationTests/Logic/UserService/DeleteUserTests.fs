namespace Apex.Api.IntegrationTests.Logic.userServ

open FSharp.Control.Tasks
open Xunit
open Apex.Api.Common.Control
open Apex.Api.IntegrationTests
open Apex.Api.Model
open Apex.Api.Logic
open Apex.Api.Enums
open Apex.Api.Logic.Services

type DeleteUserTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Delete user should work if deleting self`` =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let request = getCreateUserRequest()
            let! user = host.Get<UserService>().createUser request None
                        |> AsyncHttpResult.thenValue

            let session = getSessionForUser (user |> UserDetails.hideDetails) |> TestUtilities.setSessionPrivileges []

            //Act
            let! response = host.Get<UserService>().deleteUser user.id session

            //Assert
            response |> Result.isOk |> shouldBeTrue
        }

    [<Fact>]
    let ``Delete user should work if EditUsers and deleting other user`` () =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! currentUser = createUser() |> AsyncHttpResult.thenValue
            let request = getCreateUserRequest()
            let! user = host.Get<UserService>().createUser request None
                        |> AsyncHttpResult.thenValue

            let session = getSessionForUser { (currentUser |> UserDetails.hideDetails) with id = currentUser.id + 1} |> TestUtilities.setSessionPrivileges [Privilege.EditUsers]

            //Act
            let! response = host.Get<UserService>().deleteUser user.id session

            //Assert
            response |> Result.isOk |> shouldBeTrue
        }

    [<Fact>]
    let ``Delete user should fail if not EditUsers and deleting other user`` () =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! currentUser = createUser() |> AsyncHttpResult.thenValue
            let request = getCreateUserRequest()
            let! user = host.Get<UserService>().createUser request None
                        |> AsyncHttpResult.thenValue

            let session = getSessionForUser { (currentUser |> UserDetails.hideDetails) with id = currentUser.id + 1} |> TestUtilities.setSessionPrivileges []

            //Act
            let! response = host.Get<UserService>().deleteUser user.id session

            //Assert
            response |> shouldBeError 403 Security.noPrivilegeOrSelfErrorMessage
        }

    [<Fact>]
    let ``Delete user should fail if already deleted`` () =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! currentUser = createUser() |> AsyncHttpResult.thenValue
            let request = getCreateUserRequest()
            let! user = host.Get<UserService>().createUser request None
                        |> AsyncHttpResult.thenValue

            let session = getSessionForUser (currentUser |> UserDetails.hideDetails) |> TestUtilities.setSessionPrivileges [Privilege.EditUsers]

            let! _ = host.Get<UserService>().deleteUser user.id session

            //Act
            let! response = host.Get<UserService>().deleteUser user.id session

            //Assert
            response |> shouldBeError 404 "User not found."
        }