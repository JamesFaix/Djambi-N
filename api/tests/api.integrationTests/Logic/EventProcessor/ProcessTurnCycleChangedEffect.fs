namespace Djambi.Api.IntegrationTests.Logic.EventProcessor

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.IntegrationTests
open Djambi.Api.Model
open Djambi.Api.Logic.Services

type ProcessTurnCycleChangedEffect() =
    inherit TestsBase()
    
    [<Fact>]
    let ``Should work``() =
        //Arrange
        task {
            let! (user, _, game) = TestUtilities.createuserSessionAndGame(true) |> thenValue

            let player2request = CreatePlayerRequest.guest(user.id, "p2")
            let! _ = GameRepository.addPlayer(game.id, player2request) |> thenValue

            let player3request = CreatePlayerRequest.guest(user.id, "p3")
            let! _ = GameRepository.addPlayer(game.id, player3request) |> thenValue

            let! game = GameRepository.getGame game.id |> thenValue
            let! game = GameStartService.startGame game |> thenValue

            let newCycle = game.turnCycle |> List.rev
            let effect = Effect.turnCycleChanged(game.turnCycle, newCycle)

            match effect with
            | Effect.TurnCycleChanged e ->

                //Act
                let! updatedGame = EventProcessor.processTurnCycleChangedEffect e game |> thenValue

                //Assert

                updatedGame |> shouldBe { game with turnCycle = newCycle }

                let! persistedGame = GameRepository.getGame game.id |> thenValue
                persistedGame |> shouldBe updatedGame

            | _ -> failwith "Incorrect effect type"
        }