namespace Djambi.Api.IntegrationTests.Db

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.IntegrationTests

type UserRepositoryTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Create user should work``() =
        let request = getCreateUserRequest()
        task {
            let! user = UserRepository.createUser(request) |> thenValue
            Assert.NotEqual(0, user.id)
            Assert.Equal(request.name, user.name)
        }

    [<Fact>]
    let ``Get user should work``() =
        let request = getCreateUserRequest()
        task {
            let! createdUser = UserRepository.createUser(request) |> thenValue
            let! user = UserRepository.getUser(createdUser.id) |> thenValue
            Assert.Equal(createdUser.id, user.id)
            Assert.Equal(createdUser.name, user.name)
        }

    [<Fact>]
    let ``Delete user should work``() =
        let request = getCreateUserRequest()
        task {
            let! user = UserRepository.createUser(request) |> thenValue
            let! _ = UserRepository.deleteUser(user.id) |> thenValue
            let! result = UserRepository.getUser(user.id)
            Assert.True(result |> Result.isError)
            Assert.Equal(404, Result.error(result).statusCode)
        }