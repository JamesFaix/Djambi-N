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
        let gameRequest = getCreateGameRequest()
        task {
            let! game = GameRepository.createGame (gameRequest, userId) |> thenValue
            let! players = GameRepository.getPlayersForGames [game.id] |> thenValue

            //Act
            let! updatedGame = PlayerService.fillEmptyPlayerSlots game |> thenValue

            //Assert
            let! doubleCheck = GameRepository.getGame game.id |> thenValue

            updatedGame.players.Length |> shouldBe gameRequest.regionCount
            doubleCheck |> shouldBe updatedGame

            //All players after creator are neutral
            updatedGame.players
            |> List.filter (fun p -> p.kind = PlayerKind.Neutral)
            |> List.length
            |> shouldBe (updatedGame.players.Length - 1)
        }