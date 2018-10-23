namespace Djambi.Api.IntegrationTests.Logic

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db
open Djambi.Api.Db.Repositories
open Djambi.Api.Logic.Services
open Djambi.Api.Model.PlayerModel
open Djambi.Tests.TestUtilities

type PlayerServiceTests() =
    do 
        SqlUtility.connectionString <- connectionString

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