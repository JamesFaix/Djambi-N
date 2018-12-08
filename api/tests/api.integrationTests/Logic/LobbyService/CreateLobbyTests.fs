namespace Djambi.Api.IntegrationTests.Logic.LobbyService

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common
open Djambi.Api.IntegrationTests
open Djambi.Api.Logic.Services
open Djambi.Api.Model

type CreateLobbyTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Create lobby should work``() =
        task {
            //Arrange
            let request = getCreateLobbyRequest()
            let session = getSessionForUser 1

            //Act
            let! lobby = LobbyService.createLobby request session
                          |> AsyncHttpResult.thenValue

            //Assert
            lobby.id |> shouldNotBe 0
            lobby.allowGuests |> shouldBe request.allowGuests
            lobby.description |> shouldBe request.description
            lobby.isPublic |> shouldBe request.isPublic
            lobby.regionCount |> shouldBe request.regionCount
            lobby.createdByUserId |> shouldBe session.userId
        }

    [<Fact>]
    let ``Create lobby should add self as player``() =
        task {
            //Arrange
            let request = getCreateLobbyRequest()
            let session = getSessionForUser 1

            //Act
            let! lobby = LobbyService.createLobby request session
                         |> AsyncHttpResult.thenValue

            //Assert
            let! players = PlayerService.getLobbyPlayers lobby.id session
                           |> AsyncHttpResult.thenValue

            players.Length |> shouldBe 1
            players |> shouldExist (fun p -> p.userId = Some session.userId
                                          && p.playerType = PlayerType.User)
        }

