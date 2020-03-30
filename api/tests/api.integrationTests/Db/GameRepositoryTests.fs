namespace Apex.Api.IntegrationTests.Db

open FSharp.Control.Tasks
open Xunit
open Apex.Api.Common.Control.AsyncHttpResult
open Apex.Api.IntegrationTests
open Apex.Api.Model
open Apex.Api.Db.Interfaces
open Apex.Api.Enums

type GameRepositoryTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Create game should work``() =
        let host = HostFactory.createHost()
        //Arrange
        let userId = 1
        let request = getCreateGameRequest(userId)
        task {
            //Act
            let! gameId = host.Get<IGameRepository>().createGame request |> thenValue

            //Assert
            Assert.NotEqual(0, gameId)
        }

    [<Fact>]
    let ``Get game should work`` () =
        let host = HostFactory.createHost()
        //Arrange
        let userId = 1
        let request = getCreateGameRequest(userId)
        task {
            let! gameId = host.Get<IGameRepository>().createGame request |> thenValue

            //Act
            let! game = host.Get<IGameRepository>().getGame gameId |> thenValue

            //Assert
            Assert.Equal(gameId, game.id)
            Assert.Equal(request.parameters, game.parameters)
        }
        
    [<Fact>]
    let ``Add user player should work``() =
        let host = HostFactory.createHost()
        let gameRepo = host.Get<IGameRepository>();
        //Arrange
        let userId = 1
        let gameRequest = getCreateGameRequest(userId)
        let userRequest = getCreateUserRequest()
        task {
            let! gameId = host.Get<IGameRepository>().createGame gameRequest |> thenValue
            let! user = host.Get<IUserRepository>().createUser userRequest |> thenValue
            let request = CreatePlayerRequest.user user.id

            //Act
            let! _ = host.Get<IGameRepository>().addPlayer (gameId, request) |> thenValue

            //Assert
            let! game = host.Get<IGameRepository>().getGame gameId |> thenValue
            let exists = game.players
                         |> List.exists (fun p -> p.userId = Some user.id
                                                  && p.name = user.name
                                                  && p.kind = PlayerKind.User)
            Assert.True(exists)
        }

    [<Fact>]
    let ``Add neutral player should work``() =
        let host = HostFactory.createHost()
        //Arrange
        let userId = 1
        let gameRequest = getCreateGameRequest(userId)
        task {
            let! gameId = host.Get<IGameRepository>().createGame gameRequest |> thenValue
            let request = CreatePlayerRequest.neutral "test"

            //Act
            let! _ = host.Get<IGameRepository>().addPlayer (gameId, request) |> thenValue

            //Assert
            let! game = host.Get<IGameRepository>().getGame gameId |> thenValue
            let exists = game.players |> List.exists (fun p ->
                p.userId = None
                && p.name = request.name.Value
                && p.kind = PlayerKind.Neutral)
            Assert.True(exists)
        }

    [<Fact>]
    let ``Add guest player should work``() =
        let host = HostFactory.createHost()
        //Arrange
        let userId = 1
        let gameRequest = getCreateGameRequest(userId)
        let userRequest = getCreateUserRequest()
        task {
            let! gameId = host.Get<IGameRepository>().createGame gameRequest |> thenValue
            let! user = host.Get<IUserRepository>().createUser userRequest |> thenValue
            let request = CreatePlayerRequest.guest (user.id, "test")

            //Act
            let! _ = host.Get<IGameRepository>().addPlayer (gameId, request) |> thenValue

            //Assert
            let! game = host.Get<IGameRepository>().getGame gameId |> thenValue
            let exists = game.players |> List.exists (fun p ->
                p.userId = Some user.id
                && p.name = request.name.Value
                && p.kind = PlayerKind.Guest)
            Assert.True(exists)
        }

    [<Fact>]
    let ``Remove player should work``() =
        let host = HostFactory.createHost()
        //Arrange
        let userId = 1
        let gameRequest = getCreateGameRequest(userId)
        let userRequest = getCreateUserRequest()
        task {
            let! gameId = host.Get<IGameRepository>().createGame gameRequest |> thenValue
            let! user = host.Get<IUserRepository>().createUser userRequest |> thenValue
            let playerRequest = CreatePlayerRequest.user user.id
            let! player = host.Get<IGameRepository>().addPlayer (gameId, playerRequest) |> thenValue

            //Act
            let! _ = host.Get<IGameRepository>().removePlayer (gameId, player.id) |> thenValue

            //Assert
            let! game = host.Get<IGameRepository>().getGame gameId |> thenValue
            let exists = game.players |> List.exists (fun p -> p.id = player.id)
            Assert.False(exists)
        }