module Djambi.Api.UnitTets.Logic.Managers.UserManagerTests.CreateUserTests

open System
open System.Threading.Tasks
open Djambi.Api.Logic.Interfaces
open Djambi.Api.Db.Interfaces
open Djambi.Api.Logic.Managers
open Djambi.Api.Model
open FakeItEasy
open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Enums

[<Fact>]
let ``throws if logged in and no EditUsers privilege``() =
    let encryption = A.Fake<IEncryptionService>()
    let userRepo = A.Fake<IUserRepository>()
    let manager = UserManager(encryption, userRepo) :> IUserManager

    let request : CreateUserRequest = {
        name = ""
        password = ""
    }

    let session : Session = {
        id = 1
        user = {
            id = 1
            name = ""
            privileges = [ ]
        }
        token = ""
        createdOn = DateTime.UtcNow
        expiresOn = DateTime.UtcNow.AddDays(1.0)
    }

    let createUser() = 
        task {
            return! manager.createUser request (Some session)    
        } :> Task

    task {
        return! Assert.ThrowsAsync<UnauthorizedAccessException>(fun () -> createUser())
    }

[<Fact>]
let ``works if logged in and EditUsers privilege``() =
    let encryption = A.Fake<IEncryptionService>()
    let userRepo = A.Fake<IUserRepository>()
    let manager = UserManager(encryption, userRepo) :> IUserManager

    let request : CreateUserRequest = {
        name = "someName"
        password = "somePassword"
    }

    let userDetails = {
        id = 1
        name = request.name
        privileges = []
        password = request.password
        failedLoginAttempts = 0
        lastFailedLoginAttemptOn = None
     }

    let session : Session = {
        id = 1
        user = {
            id = 1
            name = ""
            privileges = [ Privilege.EditUsers ]
        }
        token = ""
        createdOn = DateTime.UtcNow
        expiresOn = DateTime.UtcNow.AddDays(1.0)
    }

    A.CallTo(fun () -> userRepo.createUser A<CreateUserRequest>.``_``)
     .Returns(userDetails) |> ignore

    task {
        let! actual = manager.createUser request (Some session)
        let expected = userDetails |> UserDetails.hideDetails
        Assert.Equal(expected, actual)
    }

[<Fact>]
let ``works if not logged in``() =
    let encryption = A.Fake<IEncryptionService>()
    let userRepo = A.Fake<IUserRepository>()
    let manager = UserManager(encryption, userRepo) :> IUserManager

    let request : CreateUserRequest = {
        name = "someName"
        password = "somePassword"
    }

    let userDetails = {
        id = 1
        name = request.name
        privileges = []
        password = request.password
        failedLoginAttempts = 0
        lastFailedLoginAttemptOn = None
     }

    A.CallTo(fun () -> userRepo.createUser A<CreateUserRequest>.``_``)
     .Returns(userDetails) |> ignore

    task {
        let! actual = manager.createUser request None
        let expected = userDetails |> UserDetails.hideDetails
        Assert.Equal(expected, actual)
    }
