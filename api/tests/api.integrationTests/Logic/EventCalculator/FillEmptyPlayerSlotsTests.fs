namespace Djambi.Api.IntegrationTests.Logic.EventCalculator

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.IntegrationTests
open Djambi.Api.Logic.Managers
open Djambi.Api.Logic.Services
open Djambi.Api.Model

type FillEmptyPlayerSlotsTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Fill empty player slots should return effect for each neutral player``() =
        //Arrange
        let session = getSessionForUser 1
        let gameRequest = getGameParameters()
        task {
            let! resp = GameManager.createGame gameRequest session |> thenValue
            let game = resp.game

            //Act
            let! effects = PlayerService.fillEmptyPlayerSlots game |> thenValue

            //Assert
            effects.Length |> shouldBe 2

            match (effects.[0], effects.[1]) with
            | (EventEffect.PlayerAdded p2, EventEffect.PlayerAdded p3) ->
                p2.value.kind |> shouldBe PlayerKind.Neutral
                p2.value.userId |> shouldBe None
                p3.value.kind |> shouldBe PlayerKind.Neutral
                p3.value.userId |> shouldBe None                
            | _ -> failwith "Invalid effects."
        }