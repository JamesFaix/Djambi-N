namespace Djambi.Api.IntegrationTests.Logic.LobbyService

open System
open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common
open Djambi.Api.IntegrationTests
open Djambi.Api.Logic.Services

type DeleteGameTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Delete game should work if admin``() =
        task {
            //Arrange
            let request = getGameParameters()
            let session = { getSessionForUser 1 with isAdmin = true }
            let! game = GameCrudService.createGame request session
                         |> AsyncHttpResult.thenValue

            //Act
            let! result = GameCrudService.deleteGame game.id session

            //Assert
            result |> Result.isOk |> shouldBeTrue
        }

    [<Fact>]
    let ``Delete game should work if creating user``() =
        task {
            //Arrange
            let request = getGameParameters()
            let session = { getSessionForUser 1 with isAdmin = false }
            let! game = GameCrudService.createGame request session
                         |> AsyncHttpResult.thenValue

            //Act
            let! result = GameCrudService.deleteGame game.id session

            //Assert
            result |> Result.isOk |> shouldBeTrue
        }

    [<Fact>]
    let ``Delete game should fail if not admin or creating user``() =
        task {
            //Arrange
            let request = getGameParameters()
            let createSession = getSessionForUser 1
            let! game = GameCrudService.createGame request createSession
                         |> AsyncHttpResult.thenValue

            let deleteSession = { getSessionForUser 2 with isAdmin = false }

            //Act
            let! result = GameCrudService.deleteGame game.id deleteSession

            //Assert
            result |> shouldBeError 403 "Cannot delete a game created by another user."
        }

    [<Fact>]
    let ``Delete game should fail if invalid gameId``() =
        task {
            //Arrange
            let session = { getSessionForUser 1 with isAdmin = true }

            //Act
            let! result = GameCrudService.deleteGame Int32.MinValue session

            //Assert
            result |> shouldBeError 404 "Game not found."
        }