namespace Djambi.Api.IntegrationTests.Db

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.IntegrationTests
open Djambi.Api.Model

type LobbyRepositoryTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Create lobby should work``() =
        //Arrange
        let userId = 1
        let request = getCreateLobbyRequest()
        task {
            //Act
            let! lobby = LobbyRepository.createLobby (request, userId) |> thenValue

            //Assert
            Assert.NotEqual(0, lobby.id)
            Assert.Equal(request.regionCount, lobby.regionCount)
            Assert.Equal(request.description, lobby.description)
          //  Assert.Equal(GameStatus.Open, game.status)
        }

    [<Fact>]
    let ``Get lobby should work`` () =
        //Arrange
        let userId = 1
        let request = getCreateLobbyRequest()
        task {
            let! createdLobby = LobbyRepository.createLobby (request, userId) |> thenValue

            //Act
            let! lobby = LobbyRepository.getLobby createdLobby.id |> thenValue

            //Assert
            Assert.Equal(createdLobby.id, lobby.id)
            Assert.Equal(createdLobby.description, lobby.description)
            Assert.Equal(createdLobby.regionCount, lobby.regionCount)
            //Assert.Equal(createdLobby.status, lobby.status)
        }

    [<Fact>]
    let ``Delete lobby should work``() =
        //Arrange
        let userId = 1
        let request = getCreateLobbyRequest()
        task {
            let! lobby = LobbyRepository.createLobby (request, userId) |> thenValue

            //Act
            let! _ = LobbyRepository.deleteLobby lobby.id |> thenValue

            //Assert
            let! getResult = LobbyRepository.getLobby lobby.id
            let error = getResult |> Result.error
            Assert.Equal(404, error.statusCode)
        }

    [<Fact>]
    let ``Get lobbies should work``() =
        //Arrange
        let userId = 1
        let request = getCreateLobbyRequest()
        task {
            let! createdLobby = LobbyRepository.createLobby (request, userId) |> thenValue
            let query = LobbiesQuery.empty

            //Act
            let! lobbies = LobbyRepository.getLobbies query |> thenValue

            //Assert
            let exists = lobbies |> List.exists (fun l -> l.id = createdLobby.id)
            Assert.True(exists)
          //  Assert.All(games, fun g -> Assert.Equal(GameStatus.Open, g.status))
        }