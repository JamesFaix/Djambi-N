namespace Djambi.Api.IntegrationTests.Logic.GameManager

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.IntegrationTests
open Djambi.Api.Model
open Djambi.Api.Db.Repositories

type GetGamesTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Get games should filter on createdByUserName``() =
        task {
            //Arrange
            let! (user1, _, game1) = TestUtilities.createuserSessionAndGame(false) |> thenValue
            let! (_, _, game2) = TestUtilities.createuserSessionAndGame(false) |> thenValue
            let adminSession = getSessionForUser 3 |> TestUtilities.setSessionPrivileges [ViewGames]
            let query = { GamesQuery.empty with createdByUserName = Some user1.name }

            //Act
            let! result = managers.games.getGames query adminSession
                          |> AsyncHttpResult.thenValue

            //Assert
            result |> shouldExist (fun l -> l.id = game1.id)
            result |> shouldNotExist (fun l -> l.id = game2.id)
        }

    [<Fact>]
    let ``Get games should filter on allowGuests``() =
        task {
            //Arrange
            let! (_, _, game1) = TestUtilities.createuserSessionAndGame(false) |> thenValue
            let! (_, _, game2) = TestUtilities.createuserSessionAndGame(true) |> thenValue
            let adminSession = getSessionForUser 3 |> TestUtilities.setSessionPrivileges [ViewGames]

            let query = { GamesQuery.empty with allowGuests = Some true }

            //Act
            let! result = managers.games.getGames query adminSession
                          |> AsyncHttpResult.thenValue

            //Assert
            result |> shouldNotExist (fun l -> l.id = game1.id)
            result |> shouldExist (fun l -> l.id = game2.id)
        }

    [<Fact>]
    let ``Get games should filter on isPublic``() =
        task {
            //Arrange

            let! (_, _, game1) = TestUtilities.createuserSessionAndGame(false) |> thenValue
            let! (_, _, game2) = TestUtilities.createuserSessionAndGame(true) |> thenValue
            let! _ = (db.games :?> GameRepository).updateGame({ game2 with parameters = { game2.parameters with isPublic = true }}) |> thenValue

            let adminSession = getSessionForUser 3 |> TestUtilities.setSessionPrivileges [ViewGames]
            let query = { GamesQuery.empty with isPublic = Some true }

            //Act
            let! result = managers.games.getGames query adminSession
                          |> AsyncHttpResult.thenValue

            //Assert
            result |> shouldNotExist (fun l -> l.id = game1.id)
            result |> shouldExist (fun l -> l.id = game2.id)
        }

    [<Fact>]
    let ``Get games should filter on playerUserName``() =
        task {
            //Arrange
            let! (user1, _, game1) = TestUtilities.createuserSessionAndGame(false) |> thenValue
            let! (user2, _, game2) = TestUtilities.createuserSessionAndGame(false) |> thenValue
            let adminSession = getSessionForUser 3 |> TestUtilities.setSessionPrivileges [EditPendingGames; ViewGames]

            let playerRequest = { getCreatePlayerRequest with userId = Some user2.id }
            let! _ = managers.players.addPlayer game1.id playerRequest adminSession

            let query = { GamesQuery.empty with playerUserName = Some user2.name }

            //Act
            let! result = managers.games.getGames query adminSession
                          |> AsyncHttpResult.thenValue

            //Assert
            result |> shouldExist (fun l -> l.id = game1.id)
            result |> shouldExist (fun l -> l.id = game2.id)
        }

    [<Fact>]
    let ``Get games should filter on status``() =
        task {
            //Arrange
            let! (_, _, game1) = TestUtilities.createuserSessionAndGame(false) |> thenValue
            let! (_, _, game2) = TestUtilities.createuserSessionAndGame(false) |> thenValue
            let adminSession = getSessionForUser 3 |> TestUtilities.setSessionPrivileges [ViewGames]

            let! _ = (db.games :?> GameRepository).updateGame({ game1 with status = GameStatus.Canceled });

            let query = { GamesQuery.empty with status = Some GameStatus.Pending }

            //Act
            let! result = managers.games.getGames query adminSession
                          |> AsyncHttpResult.thenValue

            //Assert
            result |> shouldNotExist (fun l -> l.id = game1.id)
            result |> shouldExist (fun l -> l.id = game2.id)
        }

    [<Fact>]
    let ``Get games should filter non-public games current user is not in, if no ViewGames privilege``() =
        task {
            //Arrange
            let! (_, session1, game1) = TestUtilities.createuserSessionAndGame(true) |> thenValue
            let! (_, session2, game2) = TestUtilities.createuserSessionAndGame(true) |> thenValue
            let! game3 = managers.games.createGame { getGameParameters() with isPublic = true } session2
                          |> AsyncHttpResult.thenValue

            let playerRequest = { getCreatePlayerRequest with userId = Some 1 }
            let! _ = managers.players.addPlayer game1.id playerRequest session1

            let query = GamesQuery.empty

            //Act
            let! result = managers.games.getGames query session1
                          |> AsyncHttpResult.thenValue

            //Assert
            result |> shouldExist (fun l -> l.id = game1.id)
            result |> shouldNotExist (fun l -> l.id = game2.id)
            result |> shouldExist (fun l -> l.id = game3.id)
        }