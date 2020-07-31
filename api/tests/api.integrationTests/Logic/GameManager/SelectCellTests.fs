namespace Apex.Api.IntegrationTests.Logic.GameManager

open System
open System.Data.Entity.Core
open System.Threading.Tasks
open FSharp.Control.Tasks
open Xunit
open Apex.Api.Common.Control
open Apex.Api.Enums
open Apex.Api.IntegrationTests
open Apex.Api.Logic
open Apex.Api.Logic.Interfaces
open Apex.Api.Model

type SelectCellTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Select cell should work if current player is active user or guest of active user``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true)

            let guestRequest = CreatePlayerRequest.guest (user.id, "test")
            let! _ = host.Get<IPlayerManager>().addPlayer game.id guestRequest session

            let! resp = host.Get<IGameManager>().startGame game.id session
            let updatedGame = resp.game

            let cellId = updatedGame.currentTurn.Value.selectionOptions.Head

            //Act
            let! response = host.Get<ITurnManager>().selectCell (updatedGame.id, cellId) session

            //Assert
            let turn = response.game.currentTurn.Value
            turn.selections.Length |> shouldBe 1
            turn.selections.Head.cellId |> shouldBe cellId
            turn.selectionOptions |> shouldNotExist (fun cId -> cId = cellId)
        }

    [<Fact>]
    let ``Select cell should fail if current player is not active user or guest of active user``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (user1, session1, game) = createuserSessionAndGame(true)

            let! user2 = createUser()
            let session2 = getSessionForUser user2
            let playerRequest = CreatePlayerRequest.user user2
            let! response = host.Get<IPlayerManager>().addPlayer game.id playerRequest session2
            let player2 = response.game.players |> List.except game.players |> List.head

            let! resp = host.Get<IGameManager>().startGame game.id session1
            let updatedGame = resp.game

            let cellId = updatedGame.currentTurn.Value.selectionOptions.Head

            let sessionWithoutActivePlayer =
                match updatedGame.turnCycle.Head with
                | x when x = player2.id -> session1
                | _ -> session2

            //Act/Assert
            let! ex = Assert.ThrowsAsync<UnauthorizedAccessException>(fun () ->
                task {
                    return! host.Get<ITurnManager>().selectCell (updatedGame.id, cellId) sessionWithoutActivePlayer
                } :> Task
            )
            
            ex.Message |> shouldBe Security.noPrivilegeOrCurrentPlayerErrorMessage
        }

    [<Fact>]
    let ``Select cell should work if OpenParticipation and current player is not active user or guest of active user``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (user1, session1, game) = createuserSessionAndGame(true)

            let! user2 = createUser()
            let session2 = getSessionForUser user2
            let playerRequest = CreatePlayerRequest.user user2
            let! response = host.Get<IPlayerManager>().addPlayer game.id playerRequest session2
            let player2 = response.game.players |> List.except game.players |> List.head
                          
            let! resp = host.Get<IGameManager>().startGame game.id session1
            let updatedGame = resp.game

            let cellId = updatedGame.currentTurn.Value.selectionOptions.Head

            let sessionWithoutActivePlayer =
                match updatedGame.turnCycle.Head with
                | x when x = player2.id -> session1
                | _ -> session2
                |> TestUtilities.setSessionPrivileges [Privilege.OpenParticipation]

            //Act
            let! response = host.Get<ITurnManager>().selectCell (updatedGame.id, cellId) sessionWithoutActivePlayer

            //Assert
            let turn = response.game.currentTurn.Value
            turn.selections.Length |> shouldBe 1
            turn.selections.Head.cellId |> shouldBe cellId
            turn.selectionOptions |> shouldNotExist (fun cId -> cId = cellId)
        }

    [<Fact>]
    let ``Select cell should fail if invalid game ID``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true)

            let guestRequest = CreatePlayerRequest.guest (user.id, "test")
            let! _ = host.Get<IPlayerManager>().addPlayer game.id guestRequest session

            let! resp = host.Get<IGameManager>().startGame game.id session
            let updatedGame = resp.game

            let cellId = updatedGame.currentTurn.Value.selectionOptions.Head

            //Act/Assert
            let! ex = Assert.ThrowsAsync<ObjectNotFoundException>(fun () ->
                task {
                    return! host.Get<ITurnManager>().selectCell (Int32.MinValue, cellId) session
                } :> Task
            )
            
            ex.Message |> shouldBe "Game not found."
        }

    [<Fact>]
    let ``Select cell should fail if invalid cell ID``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true)

            let guestRequest = CreatePlayerRequest.guest (user.id, "test")
            let! _ = host.Get<IPlayerManager>().addPlayer game.id guestRequest session

            let! resp = host.Get<IGameManager>().startGame game.id session
            let updatedGame = resp.game

            //Act/Assert
            let! ex = Assert.ThrowsAsync<HttpException>(fun () ->
                task {
                    return! host.Get<ITurnManager>().selectCell (updatedGame.id, Int32.MinValue) session
                } :> Task
            )

            ex.statusCode |> shouldBe 404
            ex.Message |> shouldBe "Cell not found."
        }

    [<Fact>]
    let ``Select cell should fail if cell is not currently selectable``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true)

            let guestRequest = CreatePlayerRequest.guest (user.id, "test")
            let! _ = host.Get<IPlayerManager>().addPlayer game.id guestRequest session

            let! resp = host.Get<IGameManager>().startGame game.id session
            let updatedGame = resp.game

            let cellId =
                [1..100]
                |> List.find (fun n -> updatedGame.currentTurn.Value.selectionOptions
                                       |> (not << List.exists (fun cId -> cId = n)))

            //Act/Assert
            let! ex = Assert.ThrowsAsync<HttpException>(fun () ->
                task {
                    return! host.Get<ITurnManager>().selectCell (updatedGame.id, cellId) session
                } :> Task
            )

            ex.statusCode |> shouldBe 400
            ex.Message |> shouldBe (sprintf "Cell %i is not currently selectable." cellId)
        }