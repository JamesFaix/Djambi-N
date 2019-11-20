namespace Apex.Api.IntegrationTests.Db

open FSharp.Control.Tasks
open Xunit
open Apex.Api.Common.Control
open Apex.Api.Common.Control.AsyncHttpResult
open Apex.Api.IntegrationTests

type UserRepositoryTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Create user should work``() =
        let request = getCreateUserRequest()
        task {
            let! user = db.users.createUser(request) |> thenValue
            Assert.NotEqual(0, user.id)
            Assert.Equal(request.name, user.name)
        }

    [<Fact>]
    let ``Get user should work``() =
        let request = getCreateUserRequest()
        task {
            let! createdUser = db.users.createUser(request) |> thenValue
            let! user = db.users.getUser(createdUser.id) |> thenValue
            Assert.Equal(createdUser.id, user.id)
            Assert.Equal(createdUser.name, user.name)
        }

    [<Fact>]
    let ``Delete user should work``() =
        let request = getCreateUserRequest()
        task {
            let! user = db.users.createUser(request) |> thenValue
            let! _ = db.users.deleteUser(user.id) |> thenValue
            let! result = db.users.getUser(user.id)
            Assert.True(result |> Result.isError)
            Assert.Equal(404, Result.error(result).statusCode)
        }