namespace Djambi.Api.IntegrationTests.Logic.GameManager

open System
open System.Data.Entity.Core
open System.Threading.Tasks
open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Enums
open Djambi.Api.IntegrationTests
open Djambi.Api.Logic.Interfaces
open Djambi.Api.Model

type RemovePlayerTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Remove player should work if removing self``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (_, _, game) = createuserSessionAndGame(false)

            let! user = createUser()
            let session = getSessionForUser user
            let request = CreatePlayerRequest.user user

            let! response = host.Get<IPlayerManager>().addPlayer game.id request session
            let player = response.game.players |> List.except game.players |> List.head

            //Act
            let! resp = host.Get<IPlayerManager>().removePlayer (game.id, player.id) session

            //Assert
            resp.game.players |> shouldNotExist (fun p -> p.id = player.id)
        }

    [<Fact>]
    let ``Remove player should fail if invalid gameId``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false)

            let! user = createUser()
            let request = CreatePlayerRequest.user user
            let session = session |> TestUtilities.setSessionPrivileges [Privilege.EditPendingGames]

            let! response = host.Get<IPlayerManager>().addPlayer game.id request session
            let player = response.game.players |> List.except game.players |> List.head

            //Act/Assert

            let! ex = Assert.ThrowsAsync<ObjectNotFoundException>(fun () -> 
                task {
                    return! host.Get<IPlayerManager>().removePlayer (Int32.MinValue, player.id) session
                } :> Task
            )

            ex.Message |> shouldBe "Game not found."

            let! game = host.Get<IGameManager>().getGame game.id session
            game.players |> shouldExist (fun p -> p.id = player.id)
        }