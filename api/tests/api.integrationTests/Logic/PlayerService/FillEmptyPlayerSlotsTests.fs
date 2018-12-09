namespace Djambi.Api.IntegrationTests.Logic.PlayerService

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.IntegrationTests
open Djambi.Api.Logic.Services
open Djambi.Api.Model

type FillEmptyPlayerSlotsTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Fill empty player slots should work``() =
        //Arrange
        let userId = 1
        let lobbyRequest = getCreateLobbyRequest()
        task {
            let! lobby = LobbyRepository.createLobby (lobbyRequest, userId) |> thenValue
            let! players = PlayerRepository.getPlayersForLobby lobby.id |> thenValue

            //Act
            let! updatedPlayers = PlayerService.fillEmptyPlayerSlots lobby players |> thenValue

            //Assert
            let! doubleCheck = PlayerRepository.getPlayersForLobby lobby.id |> thenValue

            updatedPlayers.Length |> shouldBe lobbyRequest.regionCount
            doubleCheck |> shouldBe updatedPlayers

            //All players after creator are neutral
            updatedPlayers
            |> List.filter (fun p -> p.kind = PlayerKind.Neutral)
            |> List.length
            |> shouldBe (updatedPlayers.Length - 1)
        }