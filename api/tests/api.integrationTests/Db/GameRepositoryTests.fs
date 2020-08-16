namespace Djambi.Api.IntegrationTests.Db

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Db.Interfaces
open Djambi.Api.IntegrationTests
open Djambi.Api.Model

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
            let! gameId = host.Get<IGameRepository>().createGame(request, true)

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
            let! gameId = host.Get<IGameRepository>().createGame(request, true)

            //Act
            let! game = host.Get<IGameRepository>().getGame gameId

            //Assert
            Assert.Equal(gameId, game.id)
            Assert.Equal(request.parameters, game.parameters)
        }
