namespace Djambi.Api.IntegrationTests.Logic.PlayerService

open System
open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.IntegrationTests
open Djambi.Api.Logic.Services
open Djambi.Api.Model
open Djambi.Api.Logic.Managers
open Djambi.Api.Db.Repositories

type GetRemovePlayerEventTests() =
    inherit TestsBase()
          
    //Should fail if Aborted, AbortedWhilePending, or Finished

    [<Fact>]
    let ``Should fail if removing player not in game``() =
        task {
            //Arrange
            let! (_, session, game1) = createuserSessionAndGame(false) |> thenValue
            let session = session |> TestUtilities.setSessionIsAdmin true

            let gameRequest = getGameParameters()
            let! game2 = GameManager.createGame gameRequest session |> thenValue

            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.user user.id

            let! player = GameManager.addPlayer game1.id request session
                          |> thenMap (fun resp -> resp.game.players |> List.except game1.players |> List.head)
                          |> thenValue

            //Act
            let error = PlayerService.getRemovePlayerEvent (game2, player.id) session

            //Assert
            error |> shouldBeError 404 "Player not found."
        }

    [<Fact>]
    let ``Should fail if removing neutral player``() =
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
            let! game = GameRepository.getGame game.id |> thenValue

            //Act
            let error = PlayerService.getRemovePlayerEvent (game, neutralPlayer.id) session

            //Assert
            error |> shouldBeError 400 "Cannot remove neutral players from game."
        }

    [<Fact>]
    let ``Should fail if removing different user and not admin or creator``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false) |> thenValue
            let adminSession = session |> TestUtilities.setSessionIsAdmin true

            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.user user.id
                        
            let session = session |> TestUtilities.setSessionIsAdmin false
                                  |> TestUtilities.setSessionUserId Int32.MinValue

            let! player = GameManager.addPlayer game.id request adminSession
                          |> thenMap (fun resp -> resp.game.players |> List.except game.players |> List.head)
                          |> thenValue

            let! game = GameRepository.getGame game.id |> thenValue

            //Act
            let error = PlayerService.getRemovePlayerEvent (game, player.id) session

            //Assert
            error |> shouldBeError 403 "Cannot remove other users from game."
        }

    [<Fact>]
    let ``Should work if removing guest of self``() =
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true) |> thenValue
            let request = CreatePlayerRequest.guest (user.id, "test")

            let! player = GameManager.addPlayer game.id request session
                          |> thenMap (fun resp -> resp.game.players |> List.except game.players |> List.head)
                          |> thenValue
                          
            let! game = GameRepository.getGame game.id |> thenValue

            //Act
            let event = PlayerService.getRemovePlayerEvent (game, player.id) session |> Result.value

            //Assert
            event.kind |> shouldBe EventKind.PlayerQuit
            event.effects.Length |> shouldBe 1
            match event.effects.[0] with 
            | Effect.PlayersRemoved f ->
                f.value |> shouldBe [player.id]

            | _ -> failwith "Incorrect effects"
        }

    [<Fact>]
    let ``Should work if admin and removing different user``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false) |> thenValue
            let session = session |> TestUtilities.setSessionIsAdmin true

            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.user user.id

            let! player = GameManager.addPlayer game.id request session
                          |> thenMap (fun resp -> resp.game.players |> List.except game.players |> List.head)
                          |> thenValue

            let! game = GameRepository.getGame game.id |> thenValue

            //Act
            let event = PlayerService.getRemovePlayerEvent (game, player.id) session |> Result.value

            //Assert
            event.kind |> shouldBe EventKind.PlayerEjected
            event.effects.Length |> shouldBe 1
            match event.effects.[0] with 
            | Effect.PlayersRemoved f ->
                f.value |> shouldBe [player.id]

            | _ -> failwith "Incorrect effects"
        }

    [<Fact>]
    let ``Should work if creator and removing different user``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false) |> thenValue

            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.user user.id
            
            let adminSession = session |> TestUtilities.setSessionIsAdmin true
            let! player = GameManager.addPlayer game.id request adminSession
                          |> thenMap (fun resp -> resp.game.players |> List.except game.players |> List.head)
                          |> thenValue

            let! game = GameRepository.getGame game.id |> thenValue

            //Act
            let event = PlayerService.getRemovePlayerEvent (game, player.id) session |> Result.value

            //Assert
            event.kind |> shouldBe EventKind.PlayerEjected
            event.effects.Length |> shouldBe 1
            match event.effects.[0] with 
            | Effect.PlayersRemoved f ->
                f.value |> shouldBe [player.id]

            | _ -> failwith "Incorrect effects"
        }

    [<Fact>]
    let ``Should remove guests of user``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(true) |> thenValue

            let! user = createUser() |> thenValue
            let userPlayerRequest = CreatePlayerRequest.user user.id
            let guestPlayerRequest = CreatePlayerRequest.guest (user.id, "test")

            let adminSession = session |> TestUtilities.setSessionIsAdmin true

            let! userPlayer =
                GameManager.addPlayer game.id userPlayerRequest adminSession
                |> thenMap (fun resp -> resp.game.players |> List.except game.players |> List.head)
                |> thenValue

            let! guestPlayer =
                GameManager.addPlayer game.id guestPlayerRequest adminSession
                |> thenMap (fun resp -> resp.game.players |> List.except (userPlayer :: game.players) |> List.head)
                |> thenValue

            let! game = GameRepository.getGame game.id |> thenValue

            let event = PlayerService.getRemovePlayerEvent (game, userPlayer.id) session |> Result.value

            //Assert
            event.kind |> shouldBe EventKind.PlayerEjected
            event.effects.Length |> shouldBe 1
            match event.effects.[0] with 
            | Effect.PlayersRemoved f ->
                f.value |> shouldBe [userPlayer.id; guestPlayer.id]

            | _ -> failwith "Incorrect effects"
        }

    [<Fact>]
    let ``Should not remove user if removing guest``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(true) |> thenValue

            let! user = createUser() |> thenValue
            let userPlayerRequest = CreatePlayerRequest.user user.id
            let guestPlayerRequest = CreatePlayerRequest.guest (user.id, "test")
            
            let adminSession = session |> TestUtilities.setSessionIsAdmin true

            let! userPlayer =
                GameManager.addPlayer game.id userPlayerRequest adminSession
                |> thenMap (fun resp -> resp.game.players |> List.except game.players |> List.head)
                |> thenValue

            let! guestPlayer =
                GameManager.addPlayer game.id guestPlayerRequest adminSession
                |> thenMap (fun resp -> resp.game.players |> List.except (userPlayer :: game.players) |> List.head)
                |> thenValue

            let! game = GameRepository.getGame game.id |> thenValue

            //Act
            let event = PlayerService.getRemovePlayerEvent (game, guestPlayer.id) session |> Result.value

            //Assert
            event.kind |> shouldBe EventKind.PlayerEjected
            event.effects.Length |> shouldBe 1
            match event.effects.[0] with 
            | Effect.PlayersRemoved f ->
                f.value |> shouldBe [guestPlayer.id]

            | _ -> failwith "Incorrect effects"
        }
    
    [<Fact>]
    let ``Should abort game if creating user and game is pending``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false) |> thenValue
            let creator = game.players |> List.head

            //Act
            let event = PlayerService.getRemovePlayerEvent (game, creator.id) session |> Result.value

            //Assert
            event.kind |> shouldBe EventKind.PlayerQuit
            event.effects.Length |> shouldBe 2
            match (event.effects.[0], event.effects.[1]) with 
            | (Effect.PlayersRemoved f1, Effect.GameStatusChanged f2) ->
                f1.value |> shouldBe [creator.id]

                f2.oldValue |> shouldBe GameStatus.Pending
                f2.newValue |> shouldBe GameStatus.AbortedWhilePending

            | _ -> failwith "Incorrect effects"
        }
           
    [<Fact>]
    let ``Should not abort game if creating user and game is started``() =
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true) |> thenValue

            let p2Request = 
                {
                    userId = Some user.id
                    kind = PlayerKind.Guest
                    name = Some "p2"
                }

            let! _ = GameManager.addPlayer game.id p2Request session |> thenValue

            let! resp = GameManager.startGame game.id session |> thenValue
            let game = resp.game

            //Act
            let event = PlayerService.getRemovePlayerEvent (game, game.players.[0].id) session |> Result.value

            //Assert
            event.kind |> shouldBe EventKind.PlayerQuit
            event.effects.Length |> shouldBe 1
            match event.effects.[0] with 
            | Effect.PlayersRemoved f ->
                f.value |> shouldBe [game.players.[0].id; game.players.[1].id]

            | _ -> failwith "Incorrect effects"
        }