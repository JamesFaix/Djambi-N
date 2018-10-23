namespace Djambi.Api.IntegrationTests.Logic

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.IntegrationTests
open Djambi.Api.Logic.Services
open Djambi.Api.Model.PlayerModel

type PlayerServiceTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Fill empty player slots should work``() =
        //Arrange
        let lobbyRequest = getCreateLobbyRequest()
        task {
            let! lobby = LobbyRepository.createLobby lobbyRequest |> thenValue
            let! players = PlayerRepository.getPlayers lobby.id |> thenValue

            //Act
            let! updatedPlayers = PlayerService.fillEmptyPlayerSlots lobby players |> thenValue

            //Assert
            let! doubleCheck = PlayerRepository.getPlayers lobby.id |> thenValue
            Assert.Equal(lobbyRequest.regionCount, updatedPlayers.Length)
            Assert.Equal<Player list>(updatedPlayers, doubleCheck)
        }        