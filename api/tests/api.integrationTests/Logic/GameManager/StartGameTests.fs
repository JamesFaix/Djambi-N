namespace Djambi.Api.IntegrationTests.Logic.GameManager

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common
open Djambi.Api.IntegrationTests
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Logic.Managers
open Djambi.Api.Db.Repositories
open Djambi.Api.Model

type StartGameTests() =
    inherit TestsBase()
   
    [<Fact>]
    let ``Start game should work``() =
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true) |> thenValue

            let playerRequest = CreatePlayerRequest.guest (user.id, "test")

            let! _ = GameManager.addPlayer game.id playerRequest session |> thenValue

            //Act
            let! resp = GameManager.startGame game.id session 
                        |> thenValue

            //Assert
            let updatedGame = resp.game
            updatedGame.status |> shouldBe GameStatus.Started
            updatedGame.players.Length |> shouldBe game.parameters.regionCount
            updatedGame.pieces.Length |> shouldBe (9 * game.parameters.regionCount)
        }

    [<Fact>]
    let ``Start game should fail if only one non-neutral player``() =
        task {
             //Arrange
            let! (user, session, game) = createuserSessionAndGame(true) |> thenValue

            //Act
            let! result = GameManager.startGame game.id session

            //Assert
            result |> shouldBeError 400 "Cannot start game with only one player."

            let! lobbyResult = GameRepository.getGame game.id
            lobbyResult |> Result.isOk |> shouldBeTrue
        }

    //TODO: Test other error cases