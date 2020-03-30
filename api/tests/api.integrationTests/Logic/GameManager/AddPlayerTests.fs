namespace Apex.Api.IntegrationTests.Logic.GameManager

open System
open FSharp.Control.Tasks
open Xunit
open Apex.Api.Common.Control.AsyncHttpResult
open Apex.Api.IntegrationTests
open Apex.Api.Model
open Apex.Api.Logic.Interfaces
open Apex.Api.Enums

type AddPlayerTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Should work``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (_, _, game) = createuserSessionAndGame(false) |> thenValue

            let! user = createUser() |> thenValue
            let session = getSessionForUser user.id
            let request = CreatePlayerRequest.user user.id

            //Act
            let! resp = host.Get<IPlayerManager>().addPlayer game.id request session |> thenValue

            //Assert
            let player = resp.game.players |> List.except game.players |> List.head
            player.id |> shouldNotBe 0
            player.gameId |> shouldBe game.id
            player.name |> shouldBe user.name
            player.userId |> shouldBe (Some user.id)
            player.kind |> shouldBe PlayerKind.User

            resp.event.effects.Length |> shouldBe 1
            resp.event.effects.[0] |> shouldBe (PlayerAddedEffect.fromRequest request)
        }

    [<Fact>]
    let ``Should fail if invalid lobby id``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (user, session, _) = createuserSessionAndGame(true) |> thenValue
            let request = CreatePlayerRequest.guest (user.id, "test")
            let session = session |> TestUtilities.setSessionPrivileges [Privilege.EditPendingGames]
            //Act
            let! error = host.Get<IPlayerManager>().addPlayer Int32.MinValue request session

            //Assert
            error |> shouldBeError 404 "Game not found."
        }