namespace Djambi.Api.IntegrationTests.Logic.LobbyService

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common
open Djambi.Api.IntegrationTests
open Djambi.Api.Logic.Services
open Djambi.Api.Model

type CreateGameTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Create game should work``() =
        task {
            //Arrange
            let request = getCreateGameRequest()
            let session = getSessionForUser 1

            //Act
            let! game = LobbyService.createGame request session
                          |> AsyncHttpResult.thenValue

            //Assert
            game.id |> shouldNotBe 0
            game.parameters.allowGuests |> shouldBe request.allowGuests
            game.parameters.description |> shouldBe request.description
            game.parameters.isPublic |> shouldBe request.isPublic
            game.parameters.regionCount |> shouldBe request.regionCount
            game.createdByUserId |> shouldBe session.userId
        }

    [<Fact>]
    let ``Create game should add self as player``() =
        task {
            //Arrange
            let request = getCreateGameRequest()
            let session = getSessionForUser 1

            //Act
            let! game = LobbyService.createGame request session
                         |> AsyncHttpResult.thenValue

            //Assert
            let! players = PlayerService.getGamePlayers game.id session
                           |> AsyncHttpResult.thenValue

            players.Length |> shouldBe 1
            players |> shouldExist (fun p -> p.userId = Some session.userId
                                          && p.kind = PlayerKind.User)
        }

