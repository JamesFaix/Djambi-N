module Apex.Api.UnitTests.Services.EncryptionServiceTests

open Xunit
open Apex.Api.Logic.Interfaces
open Apex.Api.Logic.Services
open System

let service = EncryptionService() :> IEncryptionService

[<Fact>]
let ``hash does not return input``() =
    let password = "abc"
    let hash = service.hash password
    Assert.NotEqual<string>(password, hash)

[<Fact>]
let ``check returns not verified if password and hash do not match``() =
    let password = "abc"
    let wrongPassword = "123"
    let hash = service.hash wrongPassword
    let result = service.check(hash, password)
    Assert.False result.verified

[<Fact>]
let ``check return verified if password matches hash``() = 
    let password = "abc"
    let hash = service.hash password
    let result = service.check(hash, password)
    Assert.True result.verified
