namespace Apex.Api.IntegrationTests.Logic.GameManager

open FSharp.Control.Tasks
open Xunit
open Apex.Api.Common.Control
open Apex.Api.IntegrationTests
open Apex.Api.Model
open Apex.Api.Logic.Interfaces
open Apex.Api.Enums

type CreateGameTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Create game should work``() =
        task {
            //Arrange
            let parameters = getGameParameters()
            let session = getSessionForUser 1

            //Act
            let! game = (gameMan :> IGameManager).createGame parameters session
                        |> AsyncHttpResult.thenValue

            //Assert
            game.id |> shouldNotBe 0
            game.parameters.allowGuests |> shouldBe parameters.allowGuests
            game.parameters.description |> shouldBe parameters.description
            game.parameters.isPublic |> shouldBe parameters.isPublic
            game.parameters.regionCount |> shouldBe parameters.regionCount
            game.createdBy.userId |> shouldBe session.user.id
        }

    [<Fact>]
    let ``Create game should add self as player``() =
        task {
            //Arrange
            let parameters = getGameParameters()
            let session = getSessionForUser 1

            //Act
            let! game = (gameMan :> IGameManager).createGame parameters session
                        |> AsyncHttpResult.thenValue

            //Assert
            let players = game.players
            players.Length |> shouldBe 1
            players |> shouldExist (fun p -> p.userId = Some session.user.id
                                          && p.kind = PlayerKind.User)
        }

