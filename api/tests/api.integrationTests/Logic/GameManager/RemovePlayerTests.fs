namespace Djambi.Api.IntegrationTests.Logic.GameManager

open System
open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.IntegrationTests
open Djambi.Api.Logic.Managers
open Djambi.Api.Logic.Services
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

            let! player = GameManager.addPlayer game.id request session
                          |> thenMap (fun resp -> resp.game.players |> List.except game.players |> List.head)
                          |> thenValue

            //Act
            let! resp = GameManager.removePlayer (game.id, player.id) session
                        |> thenValue

            //Assert
            resp.game.players |> shouldNotExist (fun p -> p.id = player.id)
        }

    [<Fact>]
    let ``Remove player should work if removing guest of self``() =
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true) |> thenValue
            let request = CreatePlayerRequest.guest (user.id, "test")

            let! player = GameManager.addPlayer game.id request session
                          |> thenMap (fun resp -> resp.game.players |> List.except game.players |> List.head)
                          |> thenValue

            //Act
            let! resp = GameManager.removePlayer (game.id, player.id) session
                        |> thenValue

            //Assert
            resp.game.players |> shouldNotExist (fun p -> p.id = player.id)
        }

    [<Fact>]
    let ``Remove player should work if admin and removing different user``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false) |> thenValue

            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.user user.id

            let! player = GameManager.addPlayer game.id request { session with isAdmin = true }
                          |> thenMap (fun resp -> resp.game.players |> List.except game.players |> List.head)
                          |> thenValue

            //Act
            let! resp = GameManager.removePlayer (game.id, player.id) { session with isAdmin = true; userId = Int32.MinValue }
                        |> thenValue

            //Assert
            resp.game.players |> shouldNotExist (fun p -> p.id = player.id)
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
                GameManager.addPlayer game.id userPlayerRequest { session with isAdmin = true }
                |> thenMap (fun resp -> resp.game.players |> List.except game.players |> List.head)
                |> thenValue

            let! guestPlayer =
                GameManager.addPlayer game.id guestPlayerRequest { session with isAdmin = true }
                |> thenMap (fun resp -> resp.game.players |> List.except (userPlayer :: game.players) |> List.head)
                |> thenValue

            //Act
            let! resp = GameManager.removePlayer (game.id, userPlayer.id) { session with isAdmin = true }
                        |> thenValue

            //Assert
            resp.game.players |> shouldNotExist (fun p -> p.id = userPlayer.id)
            resp.game.players |> shouldNotExist (fun p -> p.id = guestPlayer.id)
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
                GameManager.addPlayer game.id userPlayerRequest { session with isAdmin = true }
                |> thenMap (fun resp -> resp.game.players |> List.except game.players |> List.head)
                |> thenValue

            let! guestPlayer =
                GameManager.addPlayer game.id guestPlayerRequest { session with isAdmin = true }
                |> thenMap (fun resp -> resp.game.players |> List.except (userPlayer :: game.players) |> List.head)
                |> thenValue

            //Act
            let! resp = GameManager.removePlayer (game.id, guestPlayer.id) { session with isAdmin = true }
                        |> thenValue

            //Assert
            resp.game.players |> shouldExist (fun p -> p.id = userPlayer.id)
            resp.game.players |> shouldNotExist (fun p -> p.id = guestPlayer.id)
        }

    [<Fact>]
    let ``Remove player should close game if creating user``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false) |> thenValue
            let creator = game.players |> List.head

            //Act
            let! resp = GameManager.removePlayer (game.id, creator.id) session
                        |> thenValue

            //Assert
            resp.game.status |> shouldBe GameStatus.AbortedWhilePending
        }

    [<Fact>]
    let ``Remove player should work if creator and removing different user``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false) |> thenValue

            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.user user.id
            
            let! player = GameManager.addPlayer game.id request { session with isAdmin = true }
                          |> thenMap (fun resp -> resp.game.players |> List.except game.players |> List.head)
                          |> thenValue

            //Act
            let! resp = GameManager.removePlayer (game.id, player.id) { session with isAdmin = false }
                        |> thenValue

            //Assert
            resp.game.players |> shouldNotExist (fun p -> p.id = player.id)
        }

    [<Fact>]
    let ``Remove player should fail if removing different user and not admin or creator``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false) |> thenValue

            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.user user.id

            let! player = GameManager.addPlayer game.id request { session with isAdmin = true }
                          |> thenMap (fun resp -> resp.game.players |> List.except game.players |> List.head)
                          |> thenValue

            //Act
            let! error = GameManager.removePlayer (game.id, player.id) { session with isAdmin = false; userId = Int32.MinValue }

            //Assert
            error |> shouldBeError 403 "Cannot remove other users from game."

            let! game = GameManager.getGame game.id session |> thenValue
            game.players |> shouldExist (fun p -> p.id = player.id)
        }

    [<Fact>]
    let ``Remove player should fail if removing guest of different user and not admin or creator``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(true) |> thenValue

            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.guest (user.id, "test")

            let! player = GameManager.addPlayer game.id request { session with isAdmin = true }
                          |> thenMap (fun resp -> resp.game.players |> List.except game.players |> List.head)
                          |> thenValue

            //Act
            let! error = GameManager.removePlayer (game.id, player.id) { session with isAdmin = false; userId = Int32.MinValue }

            //Assert
            error |> shouldBeError 403 "Cannot remove other users from game."

            let! game = GameManager.getGame game.id session |> thenValue
            game.players |> shouldExist (fun p -> p.id = player.id)
        }

    [<Fact>]
    let ``Remove player should fail if removing neutral player``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false) |> thenValue

            let request = 
                {
                    kind = PlayerKind.Neutral
                    name = Some "test"
                    userId = None
                }
            let! neutralPlayer = GameRepository.addPlayer(game.id, request) |> thenValue

            //Act
            let! error = GameManager.removePlayer (game.id, neutralPlayer.id) session

            //Assert
            error |> shouldBeError 400 "Cannot remove neutral players from game."
        }

    [<Fact>]
    let ``Remove player should fail if removing player not in game``() =
        task {
            //Arrange
            let! (_, session, game1) = createuserSessionAndGame(false) |> thenValue

            let gameRequest = getGameParameters()
            let! resp2 = GameManager.createGame gameRequest session |> thenValue
            let game2 = resp2.game

            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.user user.id

            let! player = GameManager.addPlayer game1.id request { session with isAdmin = true }
                          |> thenMap (fun resp -> resp.game.players |> List.except game1.players |> List.head)
                          |> thenValue

            //Act
            let! error = GameManager.removePlayer (game2.id, player.id) { session with isAdmin = true }

            //Assert
            error |> shouldBeError 404 "Player not found."

            let! game = GameManager.getGame game1.id session |> thenValue
            game.players |> shouldExist (fun p -> p.id = player.id)
        }

    [<Fact>]
    let ``Remove player should fail if invalid gameId``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false) |> thenValue

            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.user user.id

            let! player = GameManager.addPlayer game.id request { session with isAdmin = true }
                          |> thenMap (fun resp -> resp.game.players |> List.except game.players |> List.head)
                          |> thenValue

            //Act
            let! error = GameManager.removePlayer (Int32.MinValue, player.id) { session with isAdmin = true }

            //Assert
            error |> shouldBeError 404 "Game not found."
            
            let! game = GameManager.getGame game.id session |> thenValue
            game.players |> shouldExist (fun p -> p.id = player.id)
        }

    [<Fact>]
    let ``Remove player should fail if invalid playerId``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false) |> thenValue

            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.user user.id

            let! player = GameManager.addPlayer game.id request { session with isAdmin = true }
                          |> thenMap (fun resp -> resp.game.players |> List.except game.players |> List.head)
                          |> thenValue

            //Act
            let! error = GameManager.removePlayer (game.id, Int32.MinValue) { session with isAdmin = true }

            //Assert
            error |> shouldBeError 404 "Player not found."

            let! game = GameManager.getGame game.id session |> thenValue
            game.players |> shouldExist (fun p -> p.id = player.id)
        }

    //TODO: This should be changed when player statuses are added
    [<Fact>]
    let ``Remove player should work if game already started``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false) |> thenValue

            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.user user.id

            let! player = GameManager.addPlayer game.id request { session with isAdmin = true }
                          |> thenMap (fun resp -> resp.game.players |> List.except game.players |> List.head)
                          |> thenValue

            let! _ = GameStartService.startGame game |> thenValue

            //Act
            let! result = GameManager.removePlayer (game.id, player.id) { session with isAdmin = true }

            //Assert
            result |> Result.isOk |> shouldBeTrue

            //TODO: Assert about game still containing player
        }