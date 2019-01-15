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

            let gameRequest = getGameParameters()
            let! resp2 = GameManager.createGame gameRequest session |> thenValue
            let game2 = resp2.game

            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.user user.id

            let! player = GameManager.addPlayer game1.id request { session with isAdmin = true }
                          |> thenMap (fun resp -> resp.game.players |> List.except game1.players |> List.head)
                          |> thenValue

            //Act
            let error = PlayerService.getRemovePlayerEvent (game2, player.id) { session with isAdmin = true }

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

            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.user user.id

            let! player = GameManager.addPlayer game.id request { session with isAdmin = true }
                          |> thenMap (fun resp -> resp.game.players |> List.except game.players |> List.head)
                          |> thenValue

            let! game = GameRepository.getGame game.id |> thenValue

            //Act
            let error = PlayerService.getRemovePlayerEvent (game, player.id) { session with isAdmin = false; userId = Int32.MinValue }

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

            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.user user.id

            let! player = GameManager.addPlayer game.id request { session with isAdmin = true }
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
            
            let! player = GameManager.addPlayer game.id request { session with isAdmin = true }
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

            let! userPlayer =
                GameManager.addPlayer game.id userPlayerRequest { session with isAdmin = true }
                |> thenMap (fun resp -> resp.game.players |> List.except game.players |> List.head)
                |> thenValue

            let! guestPlayer =
                GameManager.addPlayer game.id guestPlayerRequest { session with isAdmin = true }
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

            let! userPlayer =
                GameManager.addPlayer game.id userPlayerRequest { session with isAdmin = true }
                |> thenMap (fun resp -> resp.game.players |> List.except game.players |> List.head)
                |> thenValue

            let! guestPlayer =
                GameManager.addPlayer game.id guestPlayerRequest { session with isAdmin = true }
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
            let! (user, session, game) = createuserSessionAndGame(false) |> thenValue
            let player = game.players.Head

            let! game = 
                GameRepository.getGame game.id 
                |> thenBindAsync GameStartService.startGame
                |> thenValue

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