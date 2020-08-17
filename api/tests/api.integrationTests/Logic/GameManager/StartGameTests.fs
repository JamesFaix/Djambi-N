namespace Djambi.Api.IntegrationTests.Logic.GameManager

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.Control
open Djambi.Api.IntegrationTests
open Djambi.Api.Model
open Djambi.Api.Logic.Interfaces
open Djambi.Api.Db.Interfaces
open Djambi.Api.Enums
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
            let! ex = Assert.ThrowsAsync<GameConfigurationException>(fun () -> 
                task {
                    return! host.Get<IGameManager>().startGame game.id session
                } :> Task
            )

            ex.Message |> shouldBe "Cannot start game with only one player."

            let! _ = host.Get<IGameRepository>().getGame game.id
            // Didn't throw
            return ()
        }

    //TODO: Test other error cases