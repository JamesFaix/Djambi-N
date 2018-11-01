namespace Djambi.Api.IntegrationTests.Logic.LobbyService

open System
open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common
open Djambi.Api.IntegrationTests
open Djambi.Api.Logic.Services

type DeleteLobbyTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Delete lobby should work if admin``() =
        task {
            //Arrange
            let request = getCreateLobbyRequest()
            let session = { getSessionForUser 1 with isAdmin = true }
            let! lobby = LobbyService.createLobby request session
                         |> AsyncHttpResult.thenValue

            //Act
            let! result = LobbyService.deleteLobby lobby.id session

            //Assert
            result |> Result.isOk |> shouldBeTrue
        }

    [<Fact>]
    let ``Delete lobby should work if creating user``() =
        task {
            //Arrange
            let request = getCreateLobbyRequest()
            let session = { getSessionForUser 1 with isAdmin = false }
            let! lobby = LobbyService.createLobby request session
                         |> AsyncHttpResult.thenValue

            //Act
            let! result = LobbyService.deleteLobby lobby.id session

            //Assert
            result |> Result.isOk |> shouldBeTrue
        }

    [<Fact>]
    let ``Delete lobby should fail if not admin or creating user``() =
        task {
            //Arrange
            let request = getCreateLobbyRequest()
            let createSession = getSessionForUser 1
            let! lobby = LobbyService.createLobby request createSession
                         |> AsyncHttpResult.thenValue

            let deleteSession = { getSessionForUser 2 with isAdmin = false }

            //Act
            let! result = LobbyService.deleteLobby lobby.id deleteSession

            //Assert
            result |> shouldBeError 403 "Cannot delete a lobby created by another user."
        }

    [<Fact>]
    let ``Delete lobby should fail if invalid lobbyId``() =
        task {
            //Arrange
            let session = { getSessionForUser 1 with isAdmin = true }

            //Act
            let! result = LobbyService.deleteLobby Int32.MinValue session

            //Assert
            result |> shouldBeError 404 "Lobby not found."
        }