namespace Djambi.Api.IntegrationTests.Db

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.IntegrationTests
open Djambi.Api.Db.Interfaces

type UserRepositoryTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Create user should work``() =
        let host = HostFactory.createHost()
        let request = getCreateUserRequest()
        task {
            let! user = host.Get<IUserRepository>().createUser(request)
            Assert.NotEqual(0, user.id)
            Assert.Equal(request.name, user.name)
        }

    [<Fact>]
    let ``Get user should work``() =
        let host = HostFactory.createHost()
        let request = getCreateUserRequest()
        task {
            let! createdUser = host.Get<IUserRepository>().createUser(request)
            let! userOption = host.Get<IUserRepository>().getUser(createdUser.id)

            userOption.IsSome |> shouldBe true
            let user = userOption.Value

            Assert.Equal(createdUser.id, user.id)
            Assert.Equal(createdUser.name, user.name)
        }

    [<Fact>]
    let ``Delete user should work``() =
        let host = HostFactory.createHost()
        let request = getCreateUserRequest()
        task {
            let! user = host.Get<IUserRepository>().createUser(request)
            let! _ = host.Get<IUserRepository>().deleteUser(user.id)
            
            let! userOption = host.Get<IUserRepository>().getUser(user.id)

            userOption.IsNone |> shouldBe true
        }