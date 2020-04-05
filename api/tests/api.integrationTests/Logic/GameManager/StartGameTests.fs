namespace Apex.Api.IntegrationTests.Logic.GameManager

open FSharp.Control.Tasks
open Xunit
open Apex.Api.Common.Control
open Apex.Api.IntegrationTests
open Apex.Api.Model
open Apex.Api.Logic.Interfaces
open Apex.Api.Db.Interfaces
open Apex.Api.Enums
open System.Threading.Tasks

type StartGameTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Start game should work``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true)

            let playerRequest = CreatePlayerRequest.guest (user.id, "test")

            let! _ = host.Get<IPlayerManager>().addPlayer game.id playerRequest session

            //Act
            let! resp = host.Get<IGameManager>().startGame game.id session

            //Assert
            let updatedGame = resp.game
            updatedGame.status |> shouldBe GameStatus.InProgress
            updatedGame.players.Length |> shouldBe game.parameters.regionCount
            updatedGame.pieces.Length |> shouldBe (9 * game.parameters.regionCount)

            for p in updatedGame.players do
                p.colorId |> shouldNotBe None
                p.startingRegion |> shouldNotBe None
                p.status |> shouldBe (if p.kind = PlayerKind.Neutral then PlayerStatus.AcceptsDraw else PlayerStatus.Alive)
        }

    [<Fact>]
    let ``Start game should fail if only one non-neutral player``() =
        let host = HostFactory.createHost()
        task {
             //Arrange
            let! (user, session, game) = createuserSessionAndGame(true)

            //Act/Assert
            let! ex = Assert.ThrowsAsync<HttpException>(fun () -> 
                task {
                    return! host.Get<IGameManager>().startGame game.id session
                } :> Task
            )

            ex.statusCode |> shouldBe 400
            ex.Message |> shouldBe "Cannot start game with only one player."

            let! lobbyResult = host.Get<IGameRepository>().getGame game.id
            lobbyResult |> Result.isOk |> shouldBeTrue
        }

    //TODO: Test other error cases