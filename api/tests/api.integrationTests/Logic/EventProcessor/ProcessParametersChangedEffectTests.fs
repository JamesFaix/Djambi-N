namespace Djambi.Api.IntegrationTests.Logic.EventProcessor

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.IntegrationTests
open Djambi.Api.Model
open Djambi.Api.Logic.Services

type ProcessParametersChangedEffectTests() =
    inherit TestsBase()
    
    [<Fact>]
    let ``Should work``() =
        //Arrange
        task {
            let! (_, _, game) = TestUtilities.createuserSessionAndGame(true) |> thenValue

            game.parameters.allowGuests |> shouldBe true
            game.parameters.description |> shouldBe (Some "Test")
            game.parameters.isPublic |> shouldBe false
            game.parameters.regionCount |> shouldBe 3
            
            let newParameters = 
                {
                    allowGuests = false
                    description = None
                    isPublic = true
                    regionCount = 4
                }

            let effect = Effect.parametersChanged(game.parameters, newParameters)

            match effect with
            | Effect.ParametersChanged e ->

                //Act
                let! updatedGame = EventProcessor.processParameterChangedEffect e game |> thenValue

                //Assert

                updatedGame |> shouldBe { game with parameters = newParameters }

                let! persistedGame = GameRepository.getGame game.id |> thenValue
                persistedGame |> shouldBe updatedGame

            | _ -> failwith "Incorrect effect type"
        }