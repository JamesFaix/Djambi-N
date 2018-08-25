namespace Djambi.Tests

open System
open Giraffe
open Xunit

open Djambi.Api.Persistence
open Djambi.Api.Domain.LobbyModels

type UserRepositoryTests() =
    do 
        SqlUtility.connectionString <- TestUtilities.connectionString

    let getCreateUserRequest() : CreateUserRequest = 
        {
            name = "Test_" + Guid.NewGuid().ToString()
            isGuest = false
        }

    [<Fact>]
    let ``Create user should work``() =
        let request = getCreateUserRequest()
        task {
            let! user = UserRepository.createUser(request)
            Assert.NotEqual(0, user.id)
            Assert.Equal(request.name, user.name)
        }

    [<Fact>]
    let ``Get user should work``() =
        let request = getCreateUserRequest()
        task {
            let! createdUser = UserRepository.createUser(request)
            let! user = UserRepository.getUser(createdUser.id)        
            Assert.Equal(createdUser.id, user.id)
            Assert.Equal(createdUser.name, user.name)
        }

    [<Fact>]
    let ``Delete user should work``() =
        let request = getCreateUserRequest()
        task {
            let! user = UserRepository.createUser(request)
            let! _ = UserRepository.deleteUser(user.id) 
            let getUser = fun () -> UserRepository.getUser(user.id).Result |> ignore
            Assert.Throws<AggregateException>(getUser) |> ignore
        }