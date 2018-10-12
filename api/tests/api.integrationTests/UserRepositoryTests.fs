namespace Djambi.Api.IntegrationTests

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common
open Djambi.Api.Db
open Djambi.Api.Db.Repositories
open Djambi.Tests.TestUtilities

type UserRepositoryTests() =
    do 
        SqlUtility.connectionString <- connectionString

    [<Fact>]
    let ``Create user should work``() =
        let request = getCreateUserRequest()
        task {
            let! user = UserRepository.createUser(request) |> Task.map Result.value
            Assert.NotEqual(0, user.id)
            Assert.Equal(request.name, user.name)
        }

    [<Fact>]
    let ``Get user should work``() =
        let request = getCreateUserRequest()
        task {
            let! createdUser = UserRepository.createUser(request) |> Task.map Result.value
            let! user = UserRepository.getUser(createdUser.id) |> Task.map Result.value  
            Assert.Equal(createdUser.id, user.id)
            Assert.Equal(createdUser.name, user.name)
        }

    [<Fact>]
    let ``Delete user should work``() =
        let request = getCreateUserRequest()
        task {
            let! user = UserRepository.createUser(request) |> Task.map Result.value
            let! _ = UserRepository.deleteUser(user.id) |> Task.map Result.value
            let! result = UserRepository.getUser(user.id)
            Assert.True(result |> Result.isError)
            Assert.Equal(404, Result.error(result).statusCode)
        }