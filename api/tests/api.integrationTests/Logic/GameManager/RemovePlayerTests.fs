namespace Apex.Api.IntegrationTests.Logic.GameManager

open System
open FSharp.Control.Tasks
open Xunit
open Apex.Api.Common.Control.AsyncHttpResult
open Apex.Api.IntegrationTests
open Apex.Api.Model
open Apex.Api.Logic.Interfaces
open Apex.Api.Enums

type RemovePlayerTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Remove player should work if removing self``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (_, _, game) = createuserSessionAndGame(false) |> thenValue

            let! user = createUser() |> thenValue
            let session = getSessionForUser user.id
            let request = CreatePlayerRequest.user user.id

            let! player = host.Get<IPlayerManager>().addPlayer game.id request session
                          |> thenMap (fun resp -> resp.game.players |> List.except game.players |> List.head)
                          |> thenValue

            //Act
            let! resp = host.Get<IPlayerManager>().removePlayer (game.id, player.id) session
                        |> thenValue

            //Assert
            resp.game.players |> shouldNotExist (fun p -> p.id = player.id)
        }

    [<Fact>]
    let ``Remove player should fail if invalid gameId``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false) |> thenValue

            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.user user.id
            let session = session |> TestUtilities.setSessionPrivileges [Privilege.EditPendingGames]

            let! player = host.Get<IPlayerManager>().addPlayer game.id request session
                          |> thenMap (fun resp -> resp.game.players |> List.except game.players |> List.head)
                          |> thenValue

            //Act
            let! error = host.Get<IPlayerManager>().removePlayer (Int32.MinValue, player.id) session

            //Assert
            error |> shouldBeError 404 "Game not found."

            let! game = host.Get<IGameManager>().getGame game.id session |> thenValue
            game.players |> shouldExist (fun p -> p.id = player.id)
        }