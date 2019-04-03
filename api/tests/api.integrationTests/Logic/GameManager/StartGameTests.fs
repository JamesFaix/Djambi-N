namespace Djambi.Api.IntegrationTests.Logic.GameManager

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.IntegrationTests
open Djambi.Api.Model

type StartGameTests() =
    inherit TestsBase()
   
    [<Fact>]
    let ``Start game should work``() =
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true) |> thenValue

            let playerRequest = CreatePlayerRequest.guest (user.id, "test")

            let! _ = managers.players.addPlayer game.id playerRequest session |> thenValue

            //Act
            let! resp = managers.games.startGame game.id session 
                        |> thenValue

            //Assert
            let updatedGame = resp.game
            updatedGame.status |> shouldBe GameStatus.InProgress
            updatedGame.players.Length |> shouldBe game.parameters.regionCount
            updatedGame.pieces.Length |> shouldBe (9 * game.parameters.regionCount)

            for p in updatedGame.players do
                p.colorId |> shouldNotBe None
                p.startingRegion |> shouldNotBe None
                p.status |> shouldBe (if p.kind = Neutral then AcceptsDraw else Alive)
        }

    [<Fact>]
    let ``Start game should fail if only one non-neutral player``() =
        task {
             //Arrange
            let! (user, session, game) = createuserSessionAndGame(true) |> thenValue

            //Act
            let! result = managers.games.startGame game.id session

            //Assert
            result |> shouldBeError 400 "Cannot start game with only one player."

            let! lobbyResult = db.games.getGame game.id
            lobbyResult |> Result.isOk |> shouldBeTrue
        }

    //TODO: Test other error cases