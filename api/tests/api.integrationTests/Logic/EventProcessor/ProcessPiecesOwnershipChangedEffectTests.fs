namespace Djambi.Api.IntegrationTests.Logic.EventProcessor

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.IntegrationTests
open Djambi.Api.Model
open Djambi.Api.Logic.Services

type ProcessPiecesOwnershipChangedEffectTests() =
    inherit TestsBase()
    
    [<Fact>]
    let ``Should work``() =
        //Arrange
        task {
            let! (_, _, game) = TestUtilities.createuserSessionAndGame(true) |> thenValue
            let! game = GameStartService.startGame game |> thenValue

            let player1Id = game.players.Head.id

            let piece = game.pieces.Head
            piece.playerId |> shouldBe (Some player1Id)

            let effect = Effect.piecesOwnershipChanged([piece.id], Some player1Id, None)

            match effect with
            | Effect.PiecesOwnershipChanged e ->

                //Act
                let! updatedGame = EventProcessor.processPiecesOwnershipChangedEffect e game |> thenValue

                //Assert
                let updatedPiece = updatedGame.pieces |> List.find (fun p -> p.id = piece.id)
                updatedPiece |> shouldBe { piece with playerId = None }

                let! persistedGame = GameRepository.getGame game.id |> thenValue
                persistedGame |> shouldBe updatedGame

            | _ -> failwith "Incorrect effect type"
        }