namespace Djambi.Api.IntegrationTests.Logic.LobbyService

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common
open Djambi.Api.IntegrationTests
open Djambi.Api.Logic.Services
open Djambi.Api.Model.LobbyModel

type GetLobbiesTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Get lobbies should filter on createdByUserId``() =
        task {
            //Arrange
            let request = getCreateLobbyRequest()
            let session1 = getSessionForUser 1
            let session2 = getSessionForUser 2
            let adminSession = { getSessionForUser 3 with isAdmin = true }

            let! lobby1 = LobbyService.createLobby request session1
                          |> AsyncHttpResult.thenValue
            let! lobby2 = LobbyService.createLobby request session2
                          |> AsyncHttpResult.thenValue

            let query = { LobbiesQuery.empty with createdByUserId = Some 1 }

            //Act
            let! result = LobbyService.getLobbies query adminSession
                          |> AsyncHttpResult.thenValue

            //Assert
            result |> shouldExist (fun l -> l.id = lobby1.id)
            result |> shouldNotExist (fun l -> l.id = lobby2.id)
        }

    [<Fact>]
    let ``Get lobbies should filter on allowGuests``() =
        task {
            //Arrange
            let request = getCreateLobbyRequest()
            let session1 = getSessionForUser 1
            let session2 = getSessionForUser 2
            let adminSession = { getSessionForUser 3 with isAdmin = true }

            let! lobby1 = LobbyService.createLobby request session1
                          |> AsyncHttpResult.thenValue
            let! lobby2 = LobbyService.createLobby { request with allowGuests = true } session2
                          |> AsyncHttpResult.thenValue

            let query = { LobbiesQuery.empty with allowGuests = Some true }

            //Act
            let! result = LobbyService.getLobbies query adminSession
                          |> AsyncHttpResult.thenValue

            //Assert
            result |> shouldNotExist (fun l -> l.id = lobby1.id)
            result |> shouldExist (fun l -> l.id = lobby2.id)
        }

    [<Fact>]
    let ``Get lobbies should filter on isPublic``() =
        task {
            //Arrange
            let request = getCreateLobbyRequest()
            let session1 = getSessionForUser 1
            let session2 = getSessionForUser 2
            let adminSession = { getSessionForUser 3 with isAdmin = true }

            let! lobby1 = LobbyService.createLobby request session1
                          |> AsyncHttpResult.thenValue
            let! lobby2 = LobbyService.createLobby { request with isPublic = true } session2
                          |> AsyncHttpResult.thenValue

            let query = { LobbiesQuery.empty with isPublic = Some true }

            //Act
            let! result = LobbyService.getLobbies query adminSession
                          |> AsyncHttpResult.thenValue

            //Assert
            result |> shouldNotExist (fun l -> l.id = lobby1.id)
            result |> shouldExist (fun l -> l.id = lobby2.id)
        }

    [<Fact>]
    let ``Get lobbies should filter on playerUserId``() =
        task {
            //Arrange
            let request = getCreateLobbyRequest()
            let session1 = getSessionForUser 1
            let session2 = getSessionForUser 2
            let adminSession = { getSessionForUser 3 with isAdmin = true }

            let! lobby1 = LobbyService.createLobby request session1
                          |> AsyncHttpResult.thenValue
            let! lobby2 = LobbyService.createLobby request session2
                          |> AsyncHttpResult.thenValue

            let playerRequest = { getCreatePlayerRequest lobby1.id with userId = Some 1 }
            let! _ = PlayerService.addPlayerToLobby playerRequest adminSession

            let query = { LobbiesQuery.empty with playerUserId = Some 1 }

            //Act
            let! result = LobbyService.getLobbies query adminSession
                          |> AsyncHttpResult.thenValue

            //Assert
            result |> shouldExist (fun l -> l.id = lobby1.id)
            result |> shouldNotExist (fun l -> l.id = lobby2.id)
        }

    [<Fact>]
    let ``Get lobbies should filter non-public lobbies current user is not in, if not admin``() =
        task {
            //Arrange
            let request = getCreateLobbyRequest()
            let session1 = getSessionForUser 1
            let session2 = getSessionForUser 2

            let! lobby1 = LobbyService.createLobby request session1
                          |> AsyncHttpResult.thenValue
            let! lobby2 = LobbyService.createLobby request session2
                          |> AsyncHttpResult.thenValue
            let! lobby3 = LobbyService.createLobby { request with isPublic = true } session2
                          |> AsyncHttpResult.thenValue

            let playerRequest = { getCreatePlayerRequest lobby1.id with userId = Some 1 }
            let! _ = PlayerService.addPlayerToLobby playerRequest session1

            let query = LobbiesQuery.empty

            //Act
            let! result = LobbyService.getLobbies query session1
                          |> AsyncHttpResult.thenValue

            //Assert
            result |> shouldExist (fun l -> l.id = lobby1.id)
            result |> shouldNotExist (fun l -> l.id = lobby2.id)
            result |> shouldExist (fun l -> l.id = lobby3.id)
        }