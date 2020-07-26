module Apex.Api.UnitTests.Logic.Services.SessionServiceTests.OpenSessionTests

open FSharp.Control.Tasks
open FakeItEasy
open Xunit
open Apex.Api.Logic.Interfaces
open Apex.Api.Db.Interfaces
open Apex.Api.Logic.Services
open Apex.Api.Model
open System.Security.Authentication
open System.Threading.Tasks
open System

[<Fact>]
let ``throws if user does not exist``() =
    let encryption = A.Fake<IEncryptionService>()
    let sessionRepo = A.Fake<ISessionRepository>()
    let userRepo = A.Fake<IUserRepository>()
    let service = SessionService(encryption, sessionRepo, userRepo) :> ISessionService

    let request : LoginRequest = {
        username = "aUsername"
        password = "aPassword"
    }

    A.CallTo(fun () -> userRepo.getUserByName request.username)
     .Returns(None) |> ignore

    let openSession() =
        task {
            return! service.openSession request
        } :> Task

    task {
        return! Assert.ThrowsAsync<AuthenticationException>(fun () -> openSession())            
    }
    
[<Fact>]
let ``throws if account locked``() =
    let encryption = A.Fake<IEncryptionService>()
    let sessionRepo = A.Fake<ISessionRepository>()
    let userRepo = A.Fake<IUserRepository>()
    let service = SessionService(encryption, sessionRepo, userRepo) :> ISessionService
    
    let request : LoginRequest = {
        username = "aUsername"
        password = "aPassword"
    }
    
    let user : UserDetails = {
        id = 1
        name = "aUsername"
        privileges = []
        password = "aPassword"
        failedLoginAttempts = 5
        lastFailedLoginAttemptOn = Some(DateTime.UtcNow)
    }
    
    A.CallTo(fun () -> userRepo.getUserByName request.username)
        .Returns(Some user) |> ignore
    
    let openSession() =
        task {
            return! service.openSession request
        } :> Task
    
    task {
        return! Assert.ThrowsAsync<AuthenticationException>(fun () -> openSession())            
    }

[<Fact>]
let ``throws if invalid password``() =
    let encryption = A.Fake<IEncryptionService>()
    let sessionRepo = A.Fake<ISessionRepository>()
    let userRepo = A.Fake<IUserRepository>()
    let service = SessionService(encryption, sessionRepo, userRepo) :> ISessionService

    let request : LoginRequest = {
        username = "aUsername"
        password = "aPassword"
    }

    let user : UserDetails = {
        id = 1
        name = "aUsername"
        privileges = []
        password = "aPassword"
        failedLoginAttempts = 0
        lastFailedLoginAttemptOn = None
    }

    A.CallTo(fun () -> userRepo.getUserByName request.username)
     .Returns(Some user) |> ignore

    A.CallTo(fun () -> encryption.check(A<string>.``_``, A<string>.``_``))
     .Returns({ verified = false; needsUpgrade = false }) |> ignore

    let openSession() =
        task {
            return! service.openSession request
        } :> Task

    task {
        return! Assert.ThrowsAsync<AuthenticationException>(fun () -> openSession())            
    }

[<Fact>]
let ``creates session``() =
    let encryption = A.Fake<IEncryptionService>()
    let sessionRepo = A.Fake<ISessionRepository>()
    let userRepo = A.Fake<IUserRepository>()
    let service = SessionService(encryption, sessionRepo, userRepo) :> ISessionService

    let request : LoginRequest = {
        username = "aUsername"
        password = "aPassword"
    }

    let user : UserDetails = {
        id = 1
        name = "aUsername"
        privileges = []
        password = "aPassword"
        failedLoginAttempts = 0
        lastFailedLoginAttemptOn = None
    }

    let session : Session = {
        id = 1
        user = user |> UserDetails.hideDetails
        token = "aToken"
        createdOn = DateTime.UtcNow
        expiresOn = DateTime.UtcNow.AddDays(1.0)
    }

    A.CallTo(fun () -> userRepo.getUserByName request.username)
     .Returns(Some user) |> ignore

    A.CallTo(fun () -> encryption.check(A<string>.``_``, A<string>.``_``))
     .Returns({ verified = true; needsUpgrade = false }) |> ignore

    A.CallTo(fun () -> sessionRepo.createSession(A<CreateSessionRequest>.``_``))
     .Returns(session) |> ignore

    task {
        let! actual = service.openSession request
        Assert.Equal(session, actual)
    }