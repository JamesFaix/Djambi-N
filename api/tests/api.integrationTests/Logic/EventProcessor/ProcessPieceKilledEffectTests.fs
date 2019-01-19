namespace Djambi.Api.IntegrationTests.Logic.EventProcessor

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.IntegrationTests
open Djambi.Api.Model
open Djambi.Api.Logic.Services

type ProcessPieceKilledEffectTests() =
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

            let effect = Effect.pieceKilled(piece.id)
            let event = TestUtilities.createEvent([effect])

            //Act
            let! resp = EventProcessor.processEvent game event |> thenValue

            //Assert
            let updatedGame = resp.game
            let updatedPiece = updatedGame.pieces |> List.find (fun p -> p.id = piece.id)
            updatedPiece.kind |> shouldBe PieceKind.Corpse
            updatedPiece.playerId |> shouldBe None
            updatedPiece.originalPlayerId |> shouldBe piece.originalPlayerId

            let! persistedGame = GameRepository.getGame game.id |> thenValue
            persistedGame |> shouldBe updatedGame
        }