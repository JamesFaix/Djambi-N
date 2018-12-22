namespace Djambi.Api.IntegrationTests.Logic.TurnService

open System
open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.IntegrationTests
open Djambi.Api.Logic.Services
open Djambi.Api.Model
open Djambi.Api.Logic.Managers

type SelectCellTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Select cell should work if current player is active user or guest of active user``() =
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true) |> thenValue

            let guestRequest = CreatePlayerRequest.guest (user.id, "test")
            let! _ = GameManager.addPlayer game.id guestRequest session |> thenValue

            let! updatedGame = GameStartService.startGame game |> thenValue

            let cellId = updatedGame.currentTurn.Value.selectionOptions.Head

            //Act
            let! result = TurnService.selectCell (updatedGame.id, cellId) session

            //Assert
            result |> Result.isOk |> shouldBeTrue

            let turn = result |> Result.value
            turn.selections.Length |> shouldBe 1
            turn.selections.Head.cellId |> shouldBe cellId
            turn.selectionOptions |> shouldNotExist (fun cId -> cId = cellId)
        }

    [<Fact>]
    let ``Select cell should fail if current player is not active user or guest of active user``() =
        task {
            //Arrange
            let! (user1, session1, game) = createuserSessionAndGame(true) |> thenValue

            let! user2 = createUser() |> thenValue
            let session2 = getSessionForUser user2.id
            let playerRequest = CreatePlayerRequest.user user2.id
            let! player2 = GameManager.addPlayer game.id playerRequest session2 
                            |> thenMap (fun resp -> resp.game.players |> List.except game.players |> List.head)
                            |> thenValue

            let! updatedGame = GameStartService.startGame game |> thenValue

            let cellId = updatedGame.currentTurn.Value.selectionOptions.Head

            let sessionWithoutActivePlayer =
                match updatedGame.turnCycle.Head with
                | x when x = player2.id -> session1
                | _ -> session2

            //Act
            let! result = TurnService.selectCell (updatedGame.id, cellId) sessionWithoutActivePlayer

            //Assert
            result |> shouldBeError 400 "Cannot perform this action during another player's turn."
        }

    [<Fact>]
    let ``Select cell should work if admin and current player is not active user or guest of active user``() =
        task {
            //Arrange
            let! (user1, session1, game) = createuserSessionAndGame(true) |> thenValue

            let! user2 = createUser() |> thenValue
            let session2 = getSessionForUser user2.id
            let playerRequest = CreatePlayerRequest.user user2.id
            let! player2 = GameManager.addPlayer game.id playerRequest session2 
                            |> thenMap (fun resp -> resp.game.players |> List.except game.players |> List.head)
                            |> thenValue

            let! updatedGame = GameStartService.startGame game |> thenValue

            let cellId = updatedGame.currentTurn.Value.selectionOptions.Head

            let sessionWithoutActivePlayer =
                match updatedGame.turnCycle.Head with
                | x when x = player2.id -> session1
                | _ -> session2

            //Act
            let! result = TurnService.selectCell (updatedGame.id, cellId)
                                                 { sessionWithoutActivePlayer with isAdmin = true }

            //Assert
            result |> Result.isOk |> shouldBeTrue

            let turn = result |> Result.value
            turn.selections.Length |> shouldBe 1
            turn.selections.Head.cellId |> shouldBe cellId
            turn.selectionOptions |> shouldNotExist (fun cId -> cId = cellId)
        }

    [<Fact>]
    let ``Select cell should fail if invalid game ID``() =
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true) |> thenValue

            let guestRequest = CreatePlayerRequest.guest (user.id, "test")
            let! _ = GameManager.addPlayer game.id guestRequest session |> thenValue

            let! updatedGame = GameStartService.startGame game |> thenValue

            let cellId = updatedGame.currentTurn.Value.selectionOptions.Head

            //Act
            let! result = TurnService.selectCell (Int32.MinValue, cellId) session

            //Assert
            result |> shouldBeError 404 "Game not found."
        }

    [<Fact>]
    let ``Select cell should fail if invalid cell ID``() =
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true) |> thenValue

            let guestRequest = CreatePlayerRequest.guest (user.id, "test")
            let! _ = GameManager.addPlayer game.id guestRequest session |> thenValue

            let! updatedGame = GameStartService.startGame game |> thenValue

            //Act
            let! result = TurnService.selectCell (updatedGame.id, Int32.MinValue) session

            //Assert
            result |> shouldBeError 404 "Cell not found."
        }

    [<Fact>]
    let ``Select cell should fail if cell is not currently selectable``() =
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true) |> thenValue

            let guestRequest = CreatePlayerRequest.guest (user.id, "test")
            let! _ = GameManager.addPlayer game.id guestRequest session |> thenValue

            let! updatedGame = GameStartService.startGame game |> thenValue

            let cellId =
                [1..100]
                |> List.find (fun n -> updatedGame.currentTurn.Value.selectionOptions
                                       |> (not << List.exists (fun cId -> cId = n)))

            //Act
            let! result = TurnService.selectCell (updatedGame.id, cellId) session

            //Assert
            result |> shouldBeError 400 (sprintf "Cell %i is not currently selectable." cellId)
        }