namespace Apex.Api.IntegrationTests.Logic.userServ

open FSharp.Control.Tasks
open Xunit
open Apex.Api.IntegrationTests
open Apex.Api.Model
open Apex.Api.Logic
open Apex.Api.Enums
open Apex.Api.Logic.Interfaces
open System.Threading.Tasks
open System

type DeleteUserTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Delete user should work if deleting self`` =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let request = getCreateUserRequest()
            let! user = host.Get<IUserManager>().createUser request None

            let session = getSessionForUser user |> TestUtilities.setSessionPrivileges []

            //Act/Assert
            let! _ = host.Get<IUserManager>().deleteUser user.id session
            // Did no throw

            return ()
        }

    [<Fact>]
    let ``Delete user should work if EditUsers and deleting other user`` () =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! currentUser = createUser()
            let request = getCreateUserRequest()
            let! user = host.Get<IUserManager>().createUser request None

            let session = getSessionForUser { currentUser with id = currentUser.id + 1} 
                          |> TestUtilities.setSessionPrivileges [Privilege.EditUsers]

            //Act/Assert
            let! _ = host.Get<IUserManager>().deleteUser user.id session
            // Did not throw
            return ()
        }

    [<Fact>]
    let ``Delete user should fail if not EditUsers and deleting other user`` () =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! currentUser = createUser()
            let request = getCreateUserRequest()
            let! user = host.Get<IUserManager>().createUser request None

            let session = getSessionForUser currentUser |> TestUtilities.setSessionPrivileges []

            //Act/Assert
            let! ex = Assert.ThrowsAsync<UnauthorizedAccessException>(fun () ->
                host.Get<IUserManager>().deleteUser user.id session :> Task
            )

            ex.Message |> shouldBe Security.noPrivilegeOrSelfErrorMessage
        }

    [<Fact>]
    let ``Delete user should not fail if already deleted`` () =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! currentUser = createUser()
            let request = getCreateUserRequest()
            let! user = host.Get<IUserManager>().createUser request None

            let session = getSessionForUser currentUser |> TestUtilities.setSessionPrivileges [Privilege.EditUsers]

            let! _ = host.Get<IUserManager>().deleteUser user.id session

            //Act/Assert
            return! host.Get<IUserManager>().deleteUser user.id session
            // Did not throw
        }