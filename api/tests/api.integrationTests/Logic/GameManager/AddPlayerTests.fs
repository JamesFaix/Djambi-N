namespace Djambi.Api.IntegrationTests.Logic.GameManager

open System
open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.IntegrationTests
open Djambi.Api.Logic.Services
open Djambi.Api.Model
open Djambi.Api.Logic.Managers

type AddPlayerTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Should work``() =
        task {
            //Arrange
            let! (_, _, game) = createuserSessionAndGame(false) |> thenValue

            let! user = createUser() |> thenValue
            let session = getSessionForUser user.id
            let request = CreatePlayerRequest.user user.id

            //Act
            let! resp = GameManager.addPlayer game.id request session |> thenValue

            //Assert
            let player = resp.game.players |> List.except game.players |> List.head
            player.id |> shouldNotBe 0
            player.gameId |> shouldBe game.id
            player.name |> shouldBe user.name
            player.userId |> shouldBe (Some user.id)
            player.kind |> shouldBe PlayerKind.User

            resp.event.effects.Length |> shouldBe 1
            resp.event.effects.[0] |> shouldBe (Effect.playerAdded request)
        }

    [<Fact>]
    let ``Should fail if invalid lobby id``() =
        task {
            //Arrange
            let! (user, session, _) = createuserSessionAndGame(true) |> thenValue
            let request = CreatePlayerRequest.guest (user.id, "test")

            //Act
            let! error = GameManager.addPlayer Int32.MinValue request { session with isAdmin = true }

            //Assert
            error |> shouldBeError 404 "Game not found."
        }