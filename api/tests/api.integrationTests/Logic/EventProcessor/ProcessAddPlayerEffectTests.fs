namespace Djambi.Api.IntegrationTests.Logic.EventProcessor

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.IntegrationTests
open Djambi.Api.Model
open Djambi.Api.Logic.Services

type ProcessAddPlayerEffectTests() =
    inherit TestsBase()
    
    [<Fact>]
    let ``Should work``() =
        //Arrange
        task {
            let! (user, _, game) = TestUtilities.createuserSessionAndGame(true) |> thenValue

            game.players.Length |> shouldBe 1

            let player2request = CreatePlayerRequest.guest(user.id, "p2")
            let effect = Effect.playerAdded(player2request)
            let event = TestUtilities.createEvent([effect])

            //Act
            let! resp = EventProcessor.processEvent game event |> thenValue

            //Assert
            let updatedGame = resp.game
            updatedGame.players.Length |> shouldBe 2

            let newPlayer = updatedGame.players |> List.find (fun p -> p.name = player2request.name.Value)
            newPlayer.kind |> shouldBe player2request.kind
            newPlayer.userId |> shouldBe player2request.userId

            let! persistedGame = GameRepository.getGame game.id |> thenValue
            persistedGame |> shouldBe updatedGame
        }