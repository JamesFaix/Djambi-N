namespace Djambi.Api.IntegrationTests.Logic.EventProcessor

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.IntegrationTests
open Djambi.Api.Model
open Djambi.Api.Logic.Services

type ProcessPlayersRemovedEffectTests() =
    inherit TestsBase()
    
    [<Fact>]
    let ``Should work``() =
        //Arrange
        task {
            let! (user, _, game) = TestUtilities.createuserSessionAndGame(true) |> thenValue

            let player2request = CreatePlayerRequest.guest(user.id, "p2")
            let! player2 = GameRepository.addPlayer(game.id, player2request) |> thenValue

            let! game = GameRepository.getGame game.id |> thenValue           
            game.players |> shouldExist (fun p -> p.id = player2.id)
 
            let effect = Effect.playersRemoved([player2.id])
            let event = TestUtilities.createEvent([effect])

            //Act
            let! resp = EventProcessor.processEvent game event |> thenValue

            //Assert
            let updatedGame = resp.game
            updatedGame.players |> shouldNotExist (fun p -> p.id = player2.id)

            let! persistedGame = GameRepository.getGame game.id |> thenValue
            persistedGame |> shouldBe updatedGame
        }