namespace Djambi.Api.IntegrationTests.Logic.PlayerService

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.IntegrationTests
open Djambi.Api.Model
open Djambi.Api.Logic.Managers
open Djambi.Api.Logic.Services

//TODO: Audit test class
type FillEmptyPlayerSlotsTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Fill empty player slots should work``() =
        //Arrange
        let session = getSessionForUser 1
        let gameRequest = getGameParameters()
        task {
            let! game = GameManager.createGame gameRequest session |> thenValue

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

    [<Fact>]
    let ``Fill empty player slots should return effect for each neutral player``() =
        //Arrange
        let session = getSessionForUser 1
        let gameRequest = getGameParameters()
        task {
            let! game = GameManager.createGame gameRequest session |> thenValue

            //Act
            let! effects = PlayerService.fillEmptyPlayerSlots game |> thenValue

            //Assert
            effects.Length |> shouldBe 2

            match (effects.[0], effects.[1]) with
            | (Effect.PlayerAdded p2, Effect.PlayerAdded p3) ->
                p2.kind |> shouldBe PlayerKind.Neutral
                p2.userId |> shouldBe None
                p3.kind |> shouldBe PlayerKind.Neutral
                p3.userId |> shouldBe None                
            | _ -> failwith "Invalid effects."
        }