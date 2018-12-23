namespace Djambi.Api.IntegrationTests.Logic.PlayerService

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.IntegrationTests
open Djambi.Api.Model
open Djambi.Api.Logic.Managers

type FillEmptyPlayerSlotsTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Fill empty player slots should work``() =
        //Arrange
        let session = getSessionForUser 1
        let gameRequest = getGameParameters()
        task {
            let! resp = GameManager.createGame gameRequest session |> thenValue
            let game = resp.game

            //Act
            let! updatedGame = TestUtilities.fillEmptyPlayerSlots game |> thenValue

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