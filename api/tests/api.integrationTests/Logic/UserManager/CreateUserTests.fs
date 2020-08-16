namespace Djambi.Api.IntegrationTests.Logic.UserManager

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.IntegrationTests
open Djambi.Api.Model
open Djambi.Api.Enums
open Djambi.Api.Logic.Interfaces
open System.Threading.Tasks
open System.Data
open System

type CreateUserTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Create user should work if not signed in``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let request = getCreateUserRequest()

            //Act
            let! user = host.Get<IUserManager>().createUser request None

            //Assert
            user.id |> shouldNotBe 0
            user.name |> shouldBe request.name
        }

    [<Fact>]
    let ``Create user should fail if name conflict``() =
        let host = HostFactory.createHost()
        let request = getCreateUserRequest()
        
        task {
            //Arrange
            let! _ = host.Get<IUserManager>().createUser request None

            //Act/Assert
            let! ex = Assert.ThrowsAsync<DuplicateNameException>(fun () -> 
                task {
                    let! _ = host.Get<IUserManager>().createUser request None            
                    return ()
                } :> Task
            )
            
            ex.Message |> shouldBe "User name taken."
        }

    [<Fact>]
    let ``Create user should fail if signed in and not EditUsers``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! currentUser = createUser()
            let request = getCreateUserRequest()
            let session = getSessionForUser currentUser |> TestUtilities.setSessionPrivileges []

            //Act/Assert

            let! ex = Assert.ThrowsAsync<UnauthorizedAccessException>(fun () -> 
                task {
                    let! _ = host.Get<IUserManager>().createUser request (Some session)
                    return ()
                } :> Task
            )

            ex.Message |> shouldBe "Cannot create user if logged in."
        }

    [<Fact>]
    let ``Create user should work if signed in and EditUsers``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! currentUser = createUser()
            let request = getCreateUserRequest()
            let session = getSessionForUser currentUser |> TestUtilities.setSessionPrivileges [Privilege.EditUsers]

            //Act
            let! user = host.Get<IUserManager>().createUser request (Some session)

            //Assert
            user.id |> shouldNotBe 0
            user.name |> shouldBe request.name
        }