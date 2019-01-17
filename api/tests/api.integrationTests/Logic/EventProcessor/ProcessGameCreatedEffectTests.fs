namespace Djambi.Api.IntegrationTests.Logic.EventProcessor

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.IntegrationTests
open Djambi.Api.Model
open Djambi.Api.Logic.Services

type ProcessGameCreatedEffectTests() =
    inherit TestsBase()
    
    [<Fact>]
    let ``Should work``() =
        //Arrange
        task {
            let! user = TestUtilities.createUser() |> thenValue
            let gameRequest = TestUtilities.getCreateGameRequest(user.id)
            
            let effect = Effect.gameCreated(gameRequest)

            match effect with
            | Effect.GameCreated e ->

                //Act
                let! game = EventProcessor.processGameCreatedEffect e |> thenValue

                //Assert
                game.players.Length |> shouldBe 0 //Doesn't add 1st player
                game.parameters |> shouldBe gameRequest.parameters
                game.createdByUserId |> shouldBe user.id
                game.status |> shouldBe GameStatus.Pending

                let! persistedGame = GameRepository.getGame game.id |> thenValue
                persistedGame |> shouldBe game

            | _ -> failwith "Incorrect effect type"
        }