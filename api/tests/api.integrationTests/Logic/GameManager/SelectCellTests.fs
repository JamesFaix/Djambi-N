namespace Djambi.Api.IntegrationTests.Logic.GameManager

open System
open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.IntegrationTests
open Djambi.Api.Logic
open Djambi.Api.Model

type SelectCellTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Select cell should work if current player is active user or guest of active user``() =
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true) |> thenValue

            let guestRequest = CreatePlayerRequest.guest (user.id, "test")
            let! _ = managers.players.addPlayer game.id guestRequest session |> thenValue

            let! resp = managers.games.startGame game.id session |> thenValue
            let updatedGame = resp.game

            let cellId = updatedGame.currentTurn.Value.selectionOptions.Head

            //Act
            let! result = managers.turns.selectCell (updatedGame.id, cellId) session

            //Assert
            result |> Result.isOk |> shouldBeTrue

            let response = result |> Result.value
            let turn = response.game.currentTurn.Value
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
            let! player2 = managers.players.addPlayer game.id playerRequest session2 
                            |> thenMap (fun resp -> resp.game.players |> List.except game.players |> List.head)
                            |> thenValue
                                        
            let! resp = managers.games.startGame game.id session1 |> thenValue
            let updatedGame = resp.game

            let cellId = updatedGame.currentTurn.Value.selectionOptions.Head

            let sessionWithoutActivePlayer =
                match updatedGame.turnCycle.Head with
                | x when x = player2.id -> session1
                | _ -> session2

            //Act
            let! result = managers.turns.selectCell (updatedGame.id, cellId) sessionWithoutActivePlayer

            //Assert
            result |> shouldBeError 403 Security.noPrivilegeOrCurrentPlayerErrorMessage
        }

    [<Fact>]
    let ``Select cell should work if OpenParticipation and current player is not active user or guest of active user``() =
        task {
            //Arrange
            let! (user1, session1, game) = createuserSessionAndGame(true) |> thenValue

            let! user2 = createUser() |> thenValue
            let session2 = getSessionForUser user2.id
            let playerRequest = CreatePlayerRequest.user user2.id
            let! player2 = managers.players.addPlayer game.id playerRequest session2 
                            |> thenMap (fun resp -> resp.game.players |> List.except game.players |> List.head)
                            |> thenValue
                            
            let! resp = managers.games.startGame game.id session1 |> thenValue
            let updatedGame = resp.game

            let cellId = updatedGame.currentTurn.Value.selectionOptions.Head

            let sessionWithoutActivePlayer =
                match updatedGame.turnCycle.Head with
                | x when x = player2.id -> session1
                | _ -> session2
                |> TestUtilities.setSessionPrivileges [OpenParticipation]

            //Act
            let! result = managers.turns.selectCell (updatedGame.id, cellId) sessionWithoutActivePlayer

            //Assert
            result |> Result.isOk |> shouldBeTrue
            let response = result |> Result.value
            let turn = response.game.currentTurn.Value
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
            let! _ = managers.players.addPlayer game.id guestRequest session |> thenValue
            
            let! resp = managers.games.startGame game.id session |> thenValue
            let updatedGame = resp.game

            let cellId = updatedGame.currentTurn.Value.selectionOptions.Head

            //Act
            let! result = managers.turns.selectCell (Int32.MinValue, cellId) session

            //Assert
            result |> shouldBeError 404 "Game not found."
        }

    [<Fact>]
    let ``Select cell should fail if invalid cell ID``() =
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true) |> thenValue

            let guestRequest = CreatePlayerRequest.guest (user.id, "test")
            let! _ = managers.players.addPlayer game.id guestRequest session |> thenValue
            
            let! resp = managers.games.startGame game.id session |> thenValue
            let updatedGame = resp.game

            //Act
            let! result = managers.turns.selectCell (updatedGame.id, Int32.MinValue) session

            //Assert
            result |> shouldBeError 404 "Cell not found."
        }

    [<Fact>]
    let ``Select cell should fail if cell is not currently selectable``() =
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true) |> thenValue

            let guestRequest = CreatePlayerRequest.guest (user.id, "test")
            let! _ = managers.players.addPlayer game.id guestRequest session |> thenValue
            
            let! resp = managers.games.startGame game.id session |> thenValue
            let updatedGame = resp.game

            let cellId =
                [1..100]
                |> List.find (fun n -> updatedGame.currentTurn.Value.selectionOptions
                                       |> (not << List.exists (fun cId -> cId = n)))

            //Act
            let! result = managers.turns.selectCell (updatedGame.id, cellId) session

            //Assert
            result |> shouldBeError 400 (sprintf "Cell %i is not currently selectable." cellId)
        }