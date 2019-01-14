namespace Djambi.Api.IntegrationTests.Logic.EventProcessor

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.IntegrationTests
open Djambi.Api.Model
open Djambi.Api.Logic.Services

type ProcessGameStatusChangedEffectTests() =
    inherit TestsBase()
    
    [<Fact>]
    let ``Should work from Pending to Started``() =
        //Arrange
        task {
            let! (_, _, game) = TestUtilities.createuserSessionAndGame(false) |> thenValue
            game.status |> shouldBe GameStatus.Pending
            let newStatus = GameStatus.Started
            let effect = Effect.gameStatusChanged(game.status, newStatus)

            match effect with
            | Effect.GameStatusChanged e ->

                //Act
                let! game = EventProcessor.processGameStatusChangedEffect e game |> thenValue

                //Assert

                game.id |> shouldBe game.id
                game.status |> shouldBe newStatus
                game.parameters |> shouldBe game.parameters
                game.players.Length |> shouldBe 1 //this effect will not add neutral players
                game.pieces.Length |> shouldBe (game.players.Length * 9)
                game.turnCycle.Length |> shouldBe game.players.Length
                game.currentTurn.IsSome |> shouldBe true
                game.currentTurn.Value.selections.Length |> shouldBe 0
                game.currentTurn.Value.status |> shouldBe TurnStatus.AwaitingSelection
                game.currentTurn.Value.requiredSelectionKind |> shouldBe (Some SelectionKind.Subject)

                let! persistedGame = GameRepository.getGame game.id |> thenValue
                persistedGame |> shouldBe game

            | _ -> failwith "Incorrect effect type"
        }
        
    [<Fact>]
    let ``Should work from Pending to AbortedWhilePending``() =
        //Arrange
        task {
            let! (_, _, game) = TestUtilities.createuserSessionAndGame(false) |> thenValue
            game.status |> shouldBe GameStatus.Pending
            let newStatus = GameStatus.AbortedWhilePending
            let effect = Effect.gameStatusChanged(game.status, newStatus)
            
            match effect with
            | Effect.GameStatusChanged e ->

                //Act
                let! game = EventProcessor.processGameStatusChangedEffect e game |> thenValue

                //Assert
                game |> shouldBe { game with status = newStatus }

                let! persistedGame = GameRepository.getGame game.id |> thenValue
                persistedGame |> shouldBe game
                
            | _ -> failwith "Incorrect effect type"
        }
    