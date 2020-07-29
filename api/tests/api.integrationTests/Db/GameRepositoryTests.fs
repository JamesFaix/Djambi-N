namespace Apex.Api.IntegrationTests.Db

open FSharp.Control.Tasks
open Xunit
open Apex.Api.Db.Interfaces
open Apex.Api.Enums
open Apex.Api.IntegrationTests
open Apex.Api.Model

type GameRepositoryTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Create game should work``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! user = createUser()
            let request = getCreateGameRequest(user.id)

            //Act
            let! gameId = host.Get<IGameRepository>().createGame request

            //Assert
            Assert.NotEqual(0, gameId)
        }

    [<Fact>]
    let ``Get game should work`` () =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! user = createUser()
            let request = getCreateGameRequest(user.id)
            let! gameId = host.Get<IGameRepository>().createGame request

            //Act
            let! game = host.Get<IGameRepository>().getGame gameId

            //Assert
            Assert.Equal(gameId, game.id)
            Assert.Equal(request.parameters, game.parameters)
        }
        
    [<Fact>]
    let ``Add user player should work``() =
        let host = HostFactory.createHost()
        //Arrange
        task {            
            let! user = createUser()
            let gameRequest = getCreateGameRequest(user.id)
            let userRequest = getCreateUserRequest()
            let! gameId = host.Get<IGameRepository>().createGame gameRequest
            let player = CreatePlayerRequest.user user |> CreatePlayerRequest.toPlayer (Some user.name)

            //Act
            let! _ = host.Get<IPlayerRepository>().addPlayer (gameId, player)

            //Assert
            let! game = host.Get<IGameRepository>().getGame gameId
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
        task {
            let! user = createUser()
            let gameRequest = getCreateGameRequest(user.id)
            let! gameId = host.Get<IGameRepository>().createGame gameRequest
            let player = CreatePlayerRequest.neutral "test" |> CreatePlayerRequest.toPlayer None

            //Act
            let! _ = host.Get<IPlayerRepository>().addPlayer (gameId, player)

            //Assert
            let! game = host.Get<IGameRepository>().getGame gameId
            let exists = game.players |> List.exists (fun p ->
                p.userId = None
                && p.name = player.name
                && p.kind = PlayerKind.Neutral)
            Assert.True(exists)
        }

    [<Fact>]
    let ``Add guest player should work``() =
        let host = HostFactory.createHost()
        //Arrange
        task {
            let! user = createUser()
            let gameRequest = getCreateGameRequest(user.id)
            let userRequest = getCreateUserRequest()
            let! gameId = host.Get<IGameRepository>().createGame gameRequest
            let! user = host.Get<IUserRepository>().createUser userRequest
            let player = CreatePlayerRequest.guest (user.id, "test") |> CreatePlayerRequest.toPlayer None

            //Act
            let! _ = host.Get<IPlayerRepository>().addPlayer (gameId, player)

            //Assert
            let! game = host.Get<IGameRepository>().getGame gameId
            let exists = game.players |> List.exists (fun p ->
                p.userId = Some user.id
                && p.name = player.name
                && p.kind = PlayerKind.Guest)
            Assert.True(exists)
        }

    [<Fact>]
    let ``Remove player should work``() =
        let host = HostFactory.createHost()
        //Arrange
        task {
            let! user = createUser()
            let gameRequest = getCreateGameRequest(user.id)
            let userRequest = getCreateUserRequest()
            let! gameId = host.Get<IGameRepository>().createGame gameRequest
            let! user = host.Get<IUserRepository>().createUser userRequest
            let player = CreatePlayerRequest.user (user |> UserDetails.hideDetails) |> CreatePlayerRequest.toPlayer (Some user.name)
            let! player = host.Get<IPlayerRepository>().addPlayer (gameId, player)

            //Act
            let! _ = host.Get<IPlayerRepository>().removePlayer (gameId, player.id)

            //Assert
            let! game = host.Get<IGameRepository>().getGame gameId
            let exists = game.players |> List.exists (fun p -> p.id = player.id)
            Assert.False(exists)
        }