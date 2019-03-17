namespace Djambi.Api.IntegrationTests.Logic.GameManager

open System
open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.IntegrationTests
open Djambi.Api.Model

type RemovePlayerTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Remove player should work if removing self``() =
        task {
            //Arrange
            let! (_, _, game) = createuserSessionAndGame(false) |> thenValue

            let! user = createUser() |> thenValue
            let session = getSessionForUser user.id
            let request = CreatePlayerRequest.user user.id

            let! player = managers.players.addPlayer game.id request session
                          |> thenMap (fun resp -> resp.game.players |> List.except game.players |> List.head)
                          |> thenValue

            //Act
            let! resp = managers.players.removePlayer (game.id, player.id) session
                        |> thenValue

            //Assert
            resp.game.players |> shouldNotExist (fun p -> p.id = player.id)
        }

    [<Fact>]
    let ``Remove player should fail if invalid gameId``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false) |> thenValue

            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.user user.id
            let session = session |> TestUtilities.setSessionPrivileges [EditPendingGames]

            let! player = managers.players.addPlayer game.id request session
                          |> thenMap (fun resp -> resp.game.players |> List.except game.players |> List.head)
                          |> thenValue

            //Act
            let! error = managers.players.removePlayer (Int32.MinValue, player.id) session

            //Assert
            error |> shouldBeError 404 "Game not found."
            
            let! game = managers.games.getGame game.id session |> thenValue
            game.players |> shouldExist (fun p -> p.id = player.id)
        }