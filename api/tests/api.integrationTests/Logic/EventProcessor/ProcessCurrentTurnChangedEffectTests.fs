namespace Djambi.Api.IntegrationTests.Logic.EventProcessor

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.IntegrationTests
open Djambi.Api.Model
open Djambi.Api.Logic.Services

type ProcessCurrentTurnChangedEffectTests() =
    inherit TestsBase()
    
    [<Fact>]
    let ``Should work``() =
        //Arrange
        task {
            let! (user, _, game) = TestUtilities.createuserSessionAndGame(true) |> thenValue
            
            game.currentTurn |> shouldBeNone
            
            let updatedTurn : Turn = 
                { 
                    status = TurnStatus.AwaitingSelection
                    selections = [
                        Selection.subject(1, 2)
                    ]
                    selectionOptions = [3;4;5]
                    requiredSelectionKind = Some SelectionKind.Move
                }

            let effect = Effect.currentTurnChanged(game.currentTurn, Some updatedTurn)
            let event = TestUtilities.createEvent([effect])

            //Act
            let! resp = EventProcessor.processEvent game event |> thenValue

            //Assert
            let updatedGame = resp.game
            updatedGame.currentTurn |> shouldBe (Some updatedTurn)

            let! persistedGame = GameRepository.getGame game.id |> thenValue
            persistedGame |> shouldBe updatedGame
        }