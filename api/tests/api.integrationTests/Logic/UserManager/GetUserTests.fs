namespace Apex.Api.IntegrationTests.Logic.userServ

open System
open FSharp.Control.Tasks
open Xunit
open Apex.Api.Common.Control
open Apex.Api.IntegrationTests
open Apex.Api.Model
open Apex.Api.Logic
open Apex.Api.Enums
open Apex.Api.Logic.Interfaces
open System.Threading.Tasks

type GetUserTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Get user should work if EditUsers`` () =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let request = getCreateUserRequest()
            let! user = host.Get<IUserManager>().createUser request None
            let! currentUser = createUser()

            let session = getSessionForUser currentUser |> TestUtilities.setSessionPrivileges [Privilege.EditUsers]

            //Act
            let! userResponse = host.Get<IUserManager>().getUser user.id session

            //Assert
            userResponse.id |> shouldBe user.id
            userResponse.name |> shouldBe user.name
        }

    [<Fact>]
    let ``Get user should fail if not EditUsers`` () =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let request = getCreateUserRequest()
            let! user = host.Get<IUserManager>().createUser request None
            let! currentUser = createUser()

            let session = getSessionForUser currentUser |> TestUtilities.setSessionPrivileges []

            //Act/Assert
            let! ex = Assert.ThrowsAsync<UnauthorizedAccessException>(fun () -> 
                task {
                    let! _ = host.Get<IUserManager>().getUser user.id session
                    return ()
                } :> Task
            )

            ex.Message |> shouldBe Security.noPrivilegeOrSelfErrorMessage
        }

    [<Fact>]
    let ``Get user should fail is user doesn't exist`` () =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! user = createUser()
            let session = getSessionForUser user |> TestUtilities.setSessionPrivileges [Privilege.EditUsers]

            //Act/Assert
            let! ex = Assert.ThrowsAsync<NotFoundException>(fun () -> 
                task {
                    let! _ = host.Get<IUserManager>().getUser -1 session
                    return ()
                } :> Task
            )

            ex.Message |> shouldBe "User not found."
        }