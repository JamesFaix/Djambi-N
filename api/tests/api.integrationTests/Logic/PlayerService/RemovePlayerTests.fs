namespace Djambi.Api.IntegrationTests.Logic.PlayerService

open System
open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common
open Djambi.Api.IntegrationTests
open Djambi.Api.Logic.Services
open Djambi.Api.Model
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult

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

            let! player = PlayerService.addPlayer (game.id, request) session
                          |> thenValue

            //Act
            let! result = PlayerService.removePlayer (game.id, player.id) session

            //Assert
            result |> Result.isOk |> shouldBeTrue

            let! players = PlayerService.getGamePlayers game.id session
                           |> thenValue

            players |> shouldNotExist (fun p -> p.id = player.id)
        }

    [<Fact>]
    let ``Remove player should work if removing guest of self``() =
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true) |> thenValue
            let request = CreatePlayerRequest.guest (user.id, "test")

            let! player = PlayerService.addPlayer (game.id, request) session
                          |> thenValue

            //Act
            let! result = PlayerService.removePlayer (game.id, player.id) session

            //Assert
            result |> Result.isOk |> shouldBeTrue

            let! players = PlayerService.getGamePlayers game.id session
                           |> thenValue

            players |> shouldNotExist (fun p -> p.id = player.id)
        }

    [<Fact>]
    let ``Remove player should work if admin and removing different user``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false) |> thenValue

            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.user user.id

            let! player = PlayerService.addPlayer (game.id, request) { session with isAdmin = true }
                          |> thenValue

            //Act
            let! result = PlayerService.removePlayer (game.id, player.id) { session with isAdmin = true; userId = Int32.MinValue }

            //Assert
            result |> Result.isOk |> shouldBeTrue

            let! players = PlayerService.getGamePlayers game.id session
                           |> thenValue

            players |> shouldNotExist (fun p -> p.id = player.id)
        }

    [<Fact>]
    let ``Remove player should remove guests of user``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(true) |> thenValue

            let! user = createUser() |> thenValue
            let userPlayerRequest = CreatePlayerRequest.user user.id
            let guestPlayerRequest = CreatePlayerRequest.guest (user.id, "test")

            let! userPlayer =
                PlayerService.addPlayer (game.id, userPlayerRequest) { session with isAdmin = true }
                |> thenValue

            let! guestPlayer =
                PlayerService.addPlayer (game.id, guestPlayerRequest) { session with isAdmin = true }
                |> thenValue

            //Act
            let! result = PlayerService.removePlayer (game.id, userPlayer.id) { session with isAdmin = true }

            //Assert
            result |> Result.isOk |> shouldBeTrue

            let! players = PlayerService.getGamePlayers game.id session
                           |> thenValue

            players |> shouldNotExist (fun p -> p.id = userPlayer.id)
            players |> shouldNotExist (fun p -> p.id = guestPlayer.id)
        }

    [<Fact>]
    let ``Remove guest should not remove user``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(true) |> thenValue

            let! user = createUser() |> thenValue
            let userPlayerRequest = CreatePlayerRequest.user user.id
            let guestPlayerRequest = CreatePlayerRequest.guest (user.id, "test")

            let! userPlayer =
                PlayerService.addPlayer (game.id, userPlayerRequest) { session with isAdmin = true }
                |> thenValue

            let! guestPlayer =
                PlayerService.addPlayer (game.id, guestPlayerRequest) { session with isAdmin = true }
                |> thenValue

            //Act
            let! result = PlayerService.removePlayer (game.id, guestPlayer.id) { session with isAdmin = true }

            //Assert
            result |> Result.isOk |> shouldBeTrue

            let! players = PlayerService.getGamePlayers game.id session
                           |> thenValue

            players |> shouldExist (fun p -> p.id = userPlayer.id)
            players |> shouldNotExist (fun p -> p.id = guestPlayer.id)
        }

    [<Fact>]
    let ``Remove player should close game if creating user``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false) |> thenValue
            let! players = PlayerService.getGamePlayers game.id session |> thenValue
            let creator = players |> List.head

            //Act
            let! result = PlayerService.removePlayer (game.id, creator.id) session

            //Assert
            result |> Result.isOk |> shouldBeTrue

            let! game = GameService.getGame game.id session |> thenValue
            game.status |> shouldBe GameStatus.AbortedWhilePending
        }

    [<Fact>]
    let ``Remove player should work if creator and removing different user``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false) |> thenValue

            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.user user.id
            
            let! player = PlayerService.addPlayer (game.id, request) { session with isAdmin = true }
                          |> thenValue

            //Act
            let! result = PlayerService.removePlayer (game.id, player.id) { session with isAdmin = false }

            //Assert
            result |> Result.isOk |> shouldBeTrue

            let! players = PlayerService.getGamePlayers game.id session
                           |> thenValue

            players |> shouldNotExist (fun p -> p.id = player.id)
        }

    [<Fact>]
    let ``Remove player should fail if removing different user and not admin or creator``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false) |> thenValue

            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.user user.id

            let! player = PlayerService.addPlayer (game.id, request) { session with isAdmin = true }
                          |> thenValue

            //Act
            let! error = PlayerService.removePlayer (game.id, player.id) { session with isAdmin = false; userId = Int32.MinValue }

            //Assert
            error |> shouldBeError 403 "Cannot remove other users from game."

            let! players = PlayerService.getGamePlayers game.id session
                           |> thenValue

            players |> shouldExist (fun p -> p.id = player.id)
        }

    [<Fact>]
    let ``Remove player should fail if removing guest of different user and not admin or creator``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(true) |> thenValue

            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.guest (user.id, "test")

            let! player = PlayerService.addPlayer (game.id, request) { session with isAdmin = true }
                          |> thenValue

            //Act
            let! error = PlayerService.removePlayer (game.id, player.id) { session with isAdmin = false; userId = Int32.MinValue }

            //Assert
            error |> shouldBeError 403 "Cannot remove other users from game."

            let! players = PlayerService.getGamePlayers game.id session
                           |> thenValue

            players |> shouldExist (fun p -> p.id = player.id)
        }

    [<Fact>]
    let ``Remove player should fail if removing neutral player``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false) |> thenValue

            let! _ = PlayerService.fillEmptyPlayerSlots game |> thenValue
            let! updatedPlayers = PlayerService.getGamePlayers game.id session |> thenValue
            let neutralPlayer = updatedPlayers
                                |> List.filter(fun p -> p.kind = PlayerKind.Neutral)
                                |> List.head

            //Act
            let! error = PlayerService.removePlayer (game.id, neutralPlayer.id) session

            //Assert
            error |> shouldBeError 400 "Cannot remove neutral players from game."
        }

    [<Fact>]
    let ``Remove player should fail if removing player not in game``() =
        task {
            //Arrange
            let! (_, session, game1) = createuserSessionAndGame(false) |> thenValue

            let gameRequest = getGameParameters()
            let! game2 = GameCrudService.createGame gameRequest session |> thenValue

            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.user user.id

            let! player = PlayerService.addPlayer (game1.id, request) { session with isAdmin = true }
                          |> thenValue

            //Act
            let! error = PlayerService.removePlayer (game2.id, player.id) { session with isAdmin = true }

            //Assert
            error |> shouldBeError 404 "Player not found."

            let! players = PlayerService.getGamePlayers game1.id session
                           |> thenValue

            players |> shouldExist (fun p -> p.id = player.id)
        }

    [<Fact>]
    let ``Remove player should fail if invalid gameId``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false) |> thenValue

            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.user user.id

            let! player = PlayerService.addPlayer (game.id, request) { session with isAdmin = true }
                          |> thenValue

            //Act
            let! error = PlayerService.removePlayer (Int32.MinValue, player.id) { session with isAdmin = true }

            //Assert
            error |> shouldBeError 404 "Game not found."

            let! players = PlayerService.getGamePlayers game.id session
                           |> thenValue

            players |> shouldExist (fun p -> p.id = player.id)
        }

    [<Fact>]
    let ``Remove player should fail if invalid playerId``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false) |> thenValue

            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.user user.id

            let! player = PlayerService.addPlayer (game.id, request) { session with isAdmin = true }
                          |> thenValue

            //Act
            let! error = PlayerService.removePlayer (game.id, Int32.MinValue) { session with isAdmin = true }

            //Assert
            error |> shouldBeError 404 "Player not found."

            let! players = PlayerService.getGamePlayers game.id session
                           |> thenValue

            players |> shouldExist (fun p -> p.id = player.id)
        }

    [<Fact>]
    let ``Remove player should work if game already started``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false) |> thenValue

            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.user user.id

            let! player = PlayerService.addPlayer (game.id, request) { session with isAdmin = true }
                          |> thenValue

            let! _ = GameStartService.startGame game.id session |> thenValue

            //Act
            let! result = PlayerService.removePlayer (game.id, player.id) { session with isAdmin = true }

            //Assert
            result |> Result.isOk |> shouldBeTrue

            //TODO: Assert about game still containing player
        }