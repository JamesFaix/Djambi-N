namespace Apex.Api.IntegrationTests.Logic.SearchManager

open FSharp.Control.Tasks
open Xunit
open Apex.Api.Common.Control
open Apex.Api.Common.Control.AsyncHttpResult
open Apex.Api.IntegrationTests
open Apex.Api.Model
open Apex.Api.Db.Repositories
open Apex.Api.Logic.Interfaces
open Apex.Api.Enums
open Apex.Api.Db.Interfaces

type SearchGamesTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Search games should filter on createdByUserName``() =
        task {
            //Arrange
            let! (user1, _, game1) = TestUtilities.createuserSessionAndGame(false) |> thenValue
            let! (_, _, game2) = TestUtilities.createuserSessionAndGame(false) |> thenValue
            let session = getSessionForUser user1.id |> TestUtilities.setSessionPrivileges [Privilege.ViewGames]
            let query = { GamesQuery.empty with createdByUserName = Some user1.name }

            //Act
            let! result = Host.get<ISearchManager>().searchGames query session
                          |> AsyncHttpResult.thenValue

            //Assert
            result |> shouldExist (fun l -> l.id = game1.id)
            result |> shouldNotExist (fun l -> l.id = game2.id)
        }

    [<Fact>]
    let ``Search games should filter on allowGuests``() =
        task {
            //Arrange
            let! (_, _, game1) = TestUtilities.createuserSessionAndGame(false) |> thenValue
            let! (user2, _, game2) = TestUtilities.createuserSessionAndGame(true) |> thenValue
            let session = getSessionForUser user2.id |> TestUtilities.setSessionPrivileges [Privilege.ViewGames]

            let query = { GamesQuery.empty with allowGuests = Some true }

            //Act
            let! result = Host.get<ISearchManager>().searchGames query session
                          |> AsyncHttpResult.thenValue

            //Assert
            result |> shouldNotExist (fun l -> l.id = game1.id)
            result |> shouldExist (fun l -> l.id = game2.id)
        }

    [<Fact>]
    let ``Search games should filter on isPublic``() =
        task {
            //Arrange

            let! (_, _, game1) = TestUtilities.createuserSessionAndGame(false) |> thenValue
            let! (user2, _, game2) = TestUtilities.createuserSessionAndGame(true) |> thenValue
            let! _ = Host.get<IGameRepository>().updateGame({ game2 with parameters = { game2.parameters with isPublic = true }}) |> thenValue

            let session = getSessionForUser user2.id |> TestUtilities.setSessionPrivileges [Privilege.ViewGames]
            let query = { GamesQuery.empty with isPublic = Some true }

            //Act
            let! result = Host.get<ISearchManager>().searchGames query session
                          |> AsyncHttpResult.thenValue

            //Assert
            result |> shouldNotExist (fun l -> l.id = game1.id)
            result |> shouldExist (fun l -> l.id = game2.id)
        }

    [<Fact>]
    let ``Search games should filter on playerUserName``() =
        task {
            //Arrange
            let! (user1, _, game1) = TestUtilities.createuserSessionAndGame(false) |> thenValue
            let! (user2, _, game2) = TestUtilities.createuserSessionAndGame(false) |> thenValue
            let adminSession = getSessionForUser user2.id |> TestUtilities.setSessionPrivileges [Privilege.EditPendingGames; Privilege.ViewGames]

            let playerRequest = { getCreatePlayerRequest with userId = Some user2.id }
            let! _ = Host.get<IPlayerManager>().addPlayer game1.id playerRequest adminSession |> thenValue

            let query = { GamesQuery.empty with playerUserName = Some user2.name }

            //Act
            let! result = Host.get<ISearchManager>().searchGames query adminSession
                          |> AsyncHttpResult.thenValue

            //Assert
            result |> shouldExist (fun l -> l.id = game1.id)
            result |> shouldExist (fun l -> l.id = game2.id)
        }

    [<Fact>]
    let ``Search games should filter on status``() =
        task {
            //Arrange
            let! (_, _, game1) = TestUtilities.createuserSessionAndGame(false) |> thenValue
            let! (user2, _, game2) = TestUtilities.createuserSessionAndGame(false) |> thenValue
            let session = getSessionForUser user2.id |> TestUtilities.setSessionPrivileges [Privilege.ViewGames]

            let! _ = Host.get<IGameRepository>().updateGame({ game1 with status = GameStatus.Canceled });

            let query = { GamesQuery.empty with statuses = [GameStatus.Pending] }

            //Act
            let! result = Host.get<ISearchManager>().searchGames query session
                          |> AsyncHttpResult.thenValue

            //Assert
            result |> shouldNotExist (fun l -> l.id = game1.id)
            result |> shouldExist (fun l -> l.id = game2.id)
        }

    [<Fact>]
    let ``Search games should filter non-public games current user is not in, if no ViewGames privilege``() =
        task {
            //Arrange
            let! (user1, session1, game1) = TestUtilities.createuserSessionAndGame(true) |> thenValue
            let! (user2, session2, game2) = TestUtilities.createuserSessionAndGame(true) |> thenValue
            let! game3 = Host.get<IGameManager>().createGame { getGameParameters() with isPublic = true } session2
                          |> AsyncHttpResult.thenValue

            let playerRequest = { getCreatePlayerRequest with userId = Some user1.id }
            let! _ = Host.get<IPlayerManager>().addPlayer game1.id playerRequest session1

            let query = GamesQuery.empty

            //Act
            let! result = Host.get<ISearchManager>().searchGames query session1
                          |> AsyncHttpResult.thenValue

            //Assert
            result |> shouldExist (fun l -> l.id = game1.id)
            result |> shouldNotExist (fun l -> l.id = game2.id)
            result |> shouldExist (fun l -> l.id = game3.id)
        }