namespace Djambi.Api.IntegrationTests.Logic.GameManager

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.IntegrationTests
open Djambi.Api.Model
open Djambi.Api.Logic.Managers

type GetGamesTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Get games should filter on createdByUserName``() =
        task {
            //Arrange
            let! (user1, _, game1) = TestUtilities.createuserSessionAndGame(false) |> thenValue
            let! (_, _, game2) = TestUtilities.createuserSessionAndGame(false) |> thenValue
            let adminSession = getSessionForUser 3 |> TestUtilities.setSessionIsAdmin true
            let query = { GamesQuery.empty with createdByUserName = Some user1.name }

            //Act
            let! result = GameManager.getGames query adminSession
                          |> AsyncHttpResult.thenValue

            //Assert
            result |> shouldExist (fun l -> l.id = game1.id)
            result |> shouldNotExist (fun l -> l.id = game2.id)
        }

    [<Fact>]
    let ``Get games should filter on allowGuests``() =
        task {
            //Arrange
            let request = getGameParameters()
            let session1 = getSessionForUser 1
            let session2 = getSessionForUser 2
            let adminSession = getSessionForUser 3 |> TestUtilities.setSessionIsAdmin true

            let! game1 = GameManager.createGame request session1
                          |> AsyncHttpResult.thenValue
            let! game2 = GameManager.createGame { request with allowGuests = true } session2
                          |> AsyncHttpResult.thenValue

            let query = { GamesQuery.empty with allowGuests = Some true }

            //Act
            let! result = GameManager.getGames query adminSession
                          |> AsyncHttpResult.thenValue

            //Assert
            result |> shouldNotExist (fun l -> l.id = game1.id)
            result |> shouldExist (fun l -> l.id = game2.id)
        }

    [<Fact>]
    let ``Get games should filter on isPublic``() =
        task {
            //Arrange
            let request = getGameParameters()
            let session1 = getSessionForUser 1
            let session2 = getSessionForUser 2
            let adminSession = getSessionForUser 3 |> TestUtilities.setSessionIsAdmin true

            let! game1 = GameManager.createGame request session1
                          |> AsyncHttpResult.thenValue
            let! game2 = GameManager.createGame { request with isPublic = true } session2
                          |> AsyncHttpResult.thenValue

            let query = { GamesQuery.empty with isPublic = Some true }

            //Act
            let! result = GameManager.getGames query adminSession
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
            let adminSession = getSessionForUser 3 |> TestUtilities.setSessionIsAdmin true

            let playerRequest = { getCreatePlayerRequest with userId = Some user2.id }
            let! _ = GameManager.addPlayer game1.id playerRequest adminSession

            let query = { GamesQuery.empty with playerUserName = Some user2.name }

            //Act
            let! result = GameManager.getGames query adminSession
                          |> AsyncHttpResult.thenValue

            //Assert
            result |> shouldExist (fun l -> l.id = game1.id)
            result |> shouldExist (fun l -> l.id = game2.id)
        }

    [<Fact>]
    let ``Get games should filter non-public games current user is not in, if not admin``() =
        task {
            //Arrange
            let request = getGameParameters()
            let session1 = getSessionForUser 1
            let session2 = getSessionForUser 2

            let! game1 = GameManager.createGame request session1
                          |> AsyncHttpResult.thenValue
            let! game2 = GameManager.createGame request session2
                          |> AsyncHttpResult.thenValue
            let! game3 = GameManager.createGame { request with isPublic = true } session2
                          |> AsyncHttpResult.thenValue

            let playerRequest = { getCreatePlayerRequest with userId = Some 1 }
            let! _ = GameManager.addPlayer game1.id playerRequest session1

            let query = GamesQuery.empty

            //Act
            let! result = GameManager.getGames query session1
                          |> AsyncHttpResult.thenValue

            //Assert
            result |> shouldExist (fun l -> l.id = game1.id)
            result |> shouldNotExist (fun l -> l.id = game2.id)
            result |> shouldExist (fun l -> l.id = game3.id)
        }