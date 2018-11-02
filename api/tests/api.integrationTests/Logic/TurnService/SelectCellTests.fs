namespace Djambi.Api.IntegrationTests.Logic.TurnService

open System
open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.IntegrationTests
open Djambi.Api.Logic.Services
open Djambi.Api.Model.PlayerModel

type SelectCellTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Select cell should work if current player is active user or guest of active user``() =
        task {
            //Arrange
            let! (user, session, lobby) = createUserSessionAndLobby(true) |> thenValue

            let guestRequest : CreatePlayerRequest =
                {
                    lobbyId = lobby.id
                    userId = Some user.id
                    name = Some "test"
                    playerType = PlayerType.Guest
                }
            let! _ = PlayerService.addPlayerToLobby guestRequest session |> thenValue

            let! gameStart = GameStartService.startGame lobby.id session |> thenValue

            let cellId = gameStart.turnState.selectionOptions.Head

            //Act
            let! result = TurnService.selectCell (gameStart.gameId, cellId) session

            //Assert
            result |> Result.isOk |> shouldBeTrue

            let turnState = result |> Result.value
            turnState.selections.Length |> shouldBe 1
            turnState.selections.Head.cellId |> shouldBe cellId
            turnState.selectionOptions |> shouldNotExist (fun cId -> cId = cellId)
        }

    [<Fact>]
    let ``Select cell should fail if current player is not active user or guest of active user``() =
        task {
            //Arrange
            let! (user1, session1, lobby) = createUserSessionAndLobby(true) |> thenValue

            let! user2 = createUser() |> thenValue
            let session2 = getSessionForUser user2.id
            let playerRequest : CreatePlayerRequest =
                {
                    lobbyId = lobby.id
                    userId = Some user2.id
                    name = None
                    playerType = PlayerType.User
                }
            let! player2 = PlayerService.addPlayerToLobby playerRequest session2 |> thenValue

            let! gameStart = GameStartService.startGame lobby.id session1 |> thenValue

            let cellId = gameStart.turnState.selectionOptions.Head

            let sessionWithoutActivePlayer =
                match gameStart.gameState.turnCycle.Head with
                | x when x = player2.id -> session1
                | _ -> session2

            //Act
            let! result = TurnService.selectCell (gameStart.gameId, cellId) sessionWithoutActivePlayer

            //Assert
            result |> shouldBeError 400 "Cannot perform this action during another player's turn."
        }

    [<Fact>]
    let ``Select cell should work if admin and current player is not active user or guest of active user``() =
        task {
            //Arrange
            let! (user1, session1, lobby) = createUserSessionAndLobby(true) |> thenValue

            let! user2 = createUser() |> thenValue
            let session2 = getSessionForUser user2.id
            let playerRequest : CreatePlayerRequest =
                {
                    lobbyId = lobby.id
                    userId = Some user2.id
                    name = None
                    playerType = PlayerType.User
                }
            let! player2 = PlayerService.addPlayerToLobby playerRequest session2 |> thenValue

            let! gameStart = GameStartService.startGame lobby.id session1 |> thenValue

            let cellId = gameStart.turnState.selectionOptions.Head

            let sessionWithoutActivePlayer =
                match gameStart.gameState.turnCycle.Head with
                | x when x = player2.id -> session1
                | _ -> session2

            //Act
            let! result = TurnService.selectCell (gameStart.gameId, cellId)
                                                 { sessionWithoutActivePlayer with isAdmin = true }

            //Assert
            result |> Result.isOk |> shouldBeTrue

            let turnState = result |> Result.value
            turnState.selections.Length |> shouldBe 1
            turnState.selections.Head.cellId |> shouldBe cellId
            turnState.selectionOptions |> shouldNotExist (fun cId -> cId = cellId)
        }

    [<Fact>]
    let ``Select cell should fail if invalid game ID``() =
        task {
            //Arrange
            let! (user, session, lobby) = createUserSessionAndLobby(true) |> thenValue

            let guestRequest : CreatePlayerRequest =
                {
                    lobbyId = lobby.id
                    userId = Some user.id
                    name = Some "test"
                    playerType = PlayerType.Guest
                }
            let! _ = PlayerService.addPlayerToLobby guestRequest session |> thenValue

            let! gameStart = GameStartService.startGame lobby.id session |> thenValue

            let cellId = gameStart.turnState.selectionOptions.Head

            //Act
            let! result = TurnService.selectCell (Int32.MinValue, cellId) session

            //Assert
            result |> shouldBeError 404 "Game not found."
        }

    [<Fact>]
    let ``Select cell should fail if invalid cell ID``() =
        task {
            //Arrange
            let! (user, session, lobby) = createUserSessionAndLobby(true) |> thenValue

            let guestRequest : CreatePlayerRequest =
                {
                    lobbyId = lobby.id
                    userId = Some user.id
                    name = Some "test"
                    playerType = PlayerType.Guest
                }
            let! _ = PlayerService.addPlayerToLobby guestRequest session |> thenValue

            let! gameStart = GameStartService.startGame lobby.id session |> thenValue

            //Act
            let! result = TurnService.selectCell (gameStart.gameId, Int32.MinValue) session

            //Assert
            result |> shouldBeError 404 "Cell not found."
        }

    [<Fact>]
    let ``Select cell should fail if cell is not currently selectable``() =
        task {
            //Arrange
            let! (user, session, lobby) = createUserSessionAndLobby(true) |> thenValue

            let guestRequest : CreatePlayerRequest =
                {
                    lobbyId = lobby.id
                    userId = Some user.id
                    name = Some "test"
                    playerType = PlayerType.Guest
                }
            let! _ = PlayerService.addPlayerToLobby guestRequest session |> thenValue

            let! gameStart = GameStartService.startGame lobby.id session |> thenValue

            let cellId =
                [1..100]
                |> List.find (fun n -> gameStart.turnState.selectionOptions
                                       |> (not << List.exists (fun cId -> cId = n)))

            //Act
            let! result = TurnService.selectCell (gameStart.gameId, cellId) session

            //Assert
            result |> shouldBeError 400 (sprintf "Cell %i is not currently selectable." cellId)
        }