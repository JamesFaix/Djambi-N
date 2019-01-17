namespace Djambi.Api.IntegrationTests.Logic.EventProcessor

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.IntegrationTests
open Djambi.Api.Model
open Djambi.Api.Logic.Services

type ProcessPlayerEliminatedEffectTests() =
    inherit TestsBase()
    
    [<Fact>]
    let ``Should work``() =
        //Arrange
        task {
            let! (user, _, game) = TestUtilities.createuserSessionAndGame(true) |> thenValue

            let player2request = CreatePlayerRequest.guest(user.id, "p2")
            let! player2 = GameRepository.addPlayer(game.id, player2request) |> thenValue

            let! game = GameRepository.getGame game.id |> thenValue
            let! game = GameStartService.startGame game |> thenValue

            (game.players |> List.find(fun p -> p.id = player2.id)).isAlive |> shouldBe (Some true)
            game.pieces |> List.filter (fun p -> p.playerId = Some player2.id) |> List.length |> shouldBe 9
            game.turnCycle |> shouldExist (fun pId -> pId = player2.id)

            let effect = Effect.playerEliminated(player2.id)

            match effect with
            | Effect.PlayerEliminated e ->

                //Act
                let! updatedGame = EventProcessor.processPlayerEliminatedEffect e game |> thenValue

                //Assert
                (updatedGame.players |> List.find(fun p -> p.id = player2.id)).isAlive |> shouldBe (Some false)

                //This event does not update pieces or the turn cycle
                updatedGame.pieces |> List.filter (fun p -> p.playerId = Some player2.id) |> List.length |> shouldBe 9
                updatedGame.turnCycle |> shouldExist (fun pId -> pId = player2.id)

                let! persistedGame = GameRepository.getGame game.id |> thenValue
                persistedGame |> shouldBe updatedGame

            | _ -> failwith "Incorrect effect type"
        }