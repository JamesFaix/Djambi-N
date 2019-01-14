namespace Djambi.Api.IntegrationTests.Logic.EventProcessor

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.IntegrationTests
open Djambi.Api.Model
open Djambi.Api.Logic.Services

type ProcessPieceMovedEffectTests() =
    inherit TestsBase()
    
    [<Fact>]
    let ``Should work``() =
        //Arrange
        task {
            let! (_, _, game) = TestUtilities.createuserSessionAndGame(true) |> thenValue
            let! game = GameStartService.startGame game |> thenValue

            let piece = game.pieces |> List.head

            piece.kind |> shouldNotBe PieceKind.Corpse
            piece.playerId |> shouldBe (Some piece.originalPlayerId)

            let otherPiece = game.pieces.[game.pieces.Length-1] 
            let newCellId = otherPiece.cellId

            let effect = Effect.pieceMoved(piece.id, piece.cellId, newCellId)

            match effect with
            | Effect.PieceMoved e ->

                //Act
                let! updatedGame = EventProcessor.processPieceMovedEffect e game |> thenValue

                //Assert
                let updatedPiece = updatedGame.pieces |> List.find (fun p -> p.id = piece.id)
                updatedPiece.cellId |> shouldBe newCellId

                //Doesn't validate that multiple pieces aren't in the same cell
                let updatedOtherPiece = updatedGame.pieces |> List.find (fun p -> p.id = otherPiece.id)
                updatedOtherPiece.cellId |> shouldBe newCellId

                let! persistedGame = GameRepository.getGame game.id |> thenValue
                persistedGame |> shouldBe updatedGame

            | _ -> failwith "Incorrect effect type"
        }