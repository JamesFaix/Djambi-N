namespace Djambi.Api.IntegrationTests.Logic.GameManager

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common
open Djambi.Api.IntegrationTests
open Djambi.Api.Model
open Djambi.Api.Logic.Managers

type GetGamesTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Get games should filter on createdByUserId``() =
        task {
            //Arrange
            let request = getGameParameters()
            let session1 = getSessionForUser 1
            let session2 = getSessionForUser 2
            let adminSession = { getSessionForUser 3 with isAdmin = true }

            let! resp1 = GameManager.createGame request session1
                          |> AsyncHttpResult.thenValue
            let! resp2 = GameManager.createGame request session2
                          |> AsyncHttpResult.thenValue

            let query = { GamesQuery.empty with createdByUserId = Some 1 }

            //Act
            let! result = GameManager.getGames query adminSession
                          |> AsyncHttpResult.thenValue

            //Assert
            result |> shouldExist (fun l -> l.id = resp1.game.id)
            result |> shouldNotExist (fun l -> l.id = resp2.game.id)
        }

    [<Fact>]
    let ``Get games should filter on allowGuests``() =
        task {
            //Arrange
            let request = getGameParameters()
            let session1 = getSessionForUser 1
            let session2 = getSessionForUser 2
            let adminSession = { getSessionForUser 3 with isAdmin = true }

            let! resp1 = GameManager.createGame request session1
                          |> AsyncHttpResult.thenValue
            let! resp2 = GameManager.createGame { request with allowGuests = true } session2
                          |> AsyncHttpResult.thenValue

            let query = { GamesQuery.empty with allowGuests = Some true }

            //Act
            let! result = GameManager.getGames query adminSession
                          |> AsyncHttpResult.thenValue

            //Assert
            result |> shouldNotExist (fun l -> l.id = resp1.game.id)
            result |> shouldExist (fun l -> l.id = resp2.game.id)
        }

    [<Fact>]
    let ``Get games should filter on isPublic``() =
        task {
            //Arrange
            let request = getGameParameters()
            let session1 = getSessionForUser 1
            let session2 = getSessionForUser 2
            let adminSession = { getSessionForUser 3 with isAdmin = true }

            let! resp1 = GameManager.createGame request session1
                          |> AsyncHttpResult.thenValue
            let! resp2 = GameManager.createGame { request with isPublic = true } session2
                          |> AsyncHttpResult.thenValue

            let query = { GamesQuery.empty with isPublic = Some true }

            //Act
            let! result = GameManager.getGames query adminSession
                          |> AsyncHttpResult.thenValue

            //Assert
            result |> shouldNotExist (fun l -> l.id = resp1.game.id)
            result |> shouldExist (fun l -> l.id = resp2.game.id)
        }

    [<Fact>]
    let ``Get games should filter on playerUserId``() =
        task {
            //Arrange
            let request = getGameParameters()
            let session1 = getSessionForUser 1
            let session2 = getSessionForUser 2
            let adminSession = { getSessionForUser 3 with isAdmin = true }

            let! resp1 = GameManager.createGame request session1
                          |> AsyncHttpResult.thenValue
            let! resp2 = GameManager.createGame request session2
                          |> AsyncHttpResult.thenValue

            let playerRequest = { getCreatePlayerRequest with userId = Some 1 }
            let! _ = GameManager.addPlayer resp1.game.id playerRequest adminSession

            let query = { GamesQuery.empty with playerUserId = Some 1 }

            //Act
            let! result = GameManager.getGames query adminSession
                          |> AsyncHttpResult.thenValue

            //Assert
            result |> shouldExist (fun l -> l.id = resp1.game.id)
            result |> shouldNotExist (fun l -> l.id = resp2.game.id)
        }

    [<Fact>]
    let ``Get games should filter non-public games current user is not in, if not admin``() =
        task {
            //Arrange
            let request = getGameParameters()
            let session1 = getSessionForUser 1
            let session2 = getSessionForUser 2

            let! resp1 = GameManager.createGame request session1
                          |> AsyncHttpResult.thenValue
            let! resp2 = GameManager.createGame request session2
                          |> AsyncHttpResult.thenValue
            let! resp3 = GameManager.createGame { request with isPublic = true } session2
                          |> AsyncHttpResult.thenValue

            let playerRequest = { getCreatePlayerRequest with userId = Some 1 }
            let! _ = GameManager.addPlayer resp1.game.id playerRequest session1

            let query = GamesQuery.empty

            //Act
            let! result = GameManager.getGames query session1
                          |> AsyncHttpResult.thenValue

            //Assert
            result |> shouldExist (fun l -> l.id = resp1.game.id)
            result |> shouldNotExist (fun l -> l.id = resp2.game.id)
            result |> shouldExist (fun l -> l.id = resp3.game.id)
        }