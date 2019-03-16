namespace Djambi.Api.IntegrationTests.Logic.services.players

open System
open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.IntegrationTests
open Djambi.Api.Model
open Djambi.Api.Db.Repositories

type GetRemovePlayerEventTests() =
    inherit TestsBase()
          
    [<Fact>]
    let ``Should fail if removing player not in game``() =
        task {
            //Arrange
            let! (_, session, game1) = createuserSessionAndGame(false) |> thenValue
            let session = session |> TestUtilities.setSessionPrivileges [EditPendingGames]

            let gameRequest = getGameParameters()
            let! game2 = managers.games.createGame gameRequest session |> thenValue

            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.user user.id

            let! player = managers.players.addPlayer game1.id request session
                          |> thenMap (fun resp -> resp.game.players |> List.except game1.players |> List.head)
                          |> thenValue

            //Act
            let error = services.players.getRemovePlayerEvent (game2, player.id) session

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
            let error = services.players.getRemovePlayerEvent (game, neutralPlayer.id) session

            //Assert
            error |> shouldBeError 400 "Cannot remove neutral players from game."
        }

    [<Fact>]
    let ``Should fail if removing different user and not EditPendingGames or creator``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false) |> thenValue
            let adminSession = session |> TestUtilities.setSessionPrivileges [EditPendingGames]

            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.user user.id
                        
            let session = session |> TestUtilities.setSessionPrivileges []
                                  |> TestUtilities.setSessionUserId Int32.MinValue

            let! player = managers.players.addPlayer game.id request adminSession
                          |> thenMap (fun resp -> resp.game.players |> List.except game.players |> List.head)
                          |> thenValue

            let! game = GameRepository.getGame game.id |> thenValue

            //Act
            let error = services.players.getRemovePlayerEvent (game, player.id) session

            //Assert
            error |> shouldBeError 403 "Cannot remove other users from game."
        }

    [<Fact>]
    let ``Should work if removing guest of self``() =
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true) |> thenValue
            let request = CreatePlayerRequest.guest (user.id, "test")

            let! player = managers.players.addPlayer game.id request session
                          |> thenMap (fun resp -> resp.game.players |> List.except game.players |> List.head)
                          |> thenValue
                          
            let! game = GameRepository.getGame game.id |> thenValue

            //Act
            let event = services.players.getRemovePlayerEvent (game, player.id) session |> Result.value

            //Assert
            event.kind |> shouldBe EventKind.PlayerRemoved
            event.effects.Length |> shouldBe 1
            match event.effects.[0] with 
            | Effect.PlayerRemoved f ->
                f.playerId |> shouldBe player.id

            | _ -> failwith "Incorrect effects"
        }

    [<Fact>]
    let ``Should work if EditPendingGames and removing different user``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false) |> thenValue
            let session = session |> TestUtilities.setSessionPrivileges [EditPendingGames]

            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.user user.id

            let! player = managers.players.addPlayer game.id request session
                          |> thenMap (fun resp -> resp.game.players |> List.except game.players |> List.head)
                          |> thenValue

            let! game = GameRepository.getGame game.id |> thenValue

            //Act
            let event = services.players.getRemovePlayerEvent (game, player.id) session |> Result.value

            //Assert
            event.kind |> shouldBe EventKind.PlayerRemoved
            event.effects.Length |> shouldBe 1
            match event.effects.[0] with 
            | Effect.PlayerRemoved f ->
                f.playerId |> shouldBe player.id

            | _ -> failwith "Incorrect effects"
        }

    [<Fact>]
    let ``Should work if creator and removing different user``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false) |> thenValue

            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.user user.id
            
            let adminSession = session |> TestUtilities.setSessionPrivileges [EditPendingGames]
            let! player = managers.players.addPlayer game.id request adminSession
                          |> thenMap (fun resp -> resp.game.players |> List.except game.players |> List.head)
                          |> thenValue

            let! game = GameRepository.getGame game.id |> thenValue

            //Act
            let event = services.players.getRemovePlayerEvent (game, player.id) session |> Result.value

            //Assert
            event.kind |> shouldBe EventKind.PlayerRemoved
            event.effects.Length |> shouldBe 1
            match event.effects.[0] with 
            | Effect.PlayerRemoved f ->
                f.playerId |> shouldBe player.id

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

            let adminSession = session |> TestUtilities.setSessionPrivileges [EditPendingGames]

            let! userPlayer =
                managers.players.addPlayer game.id userPlayerRequest adminSession
                |> thenMap (fun resp -> resp.game.players |> List.except game.players |> List.head)
                |> thenValue

            let! guestPlayer =
                managers.players.addPlayer game.id guestPlayerRequest adminSession
                |> thenMap (fun resp -> resp.game.players |> List.except (userPlayer :: game.players) |> List.head)
                |> thenValue

            let! game = GameRepository.getGame game.id |> thenValue

            let event = services.players.getRemovePlayerEvent (game, userPlayer.id) session |> Result.value

            //Assert
            event.kind |> shouldBe EventKind.PlayerRemoved
            event.effects.Length |> shouldBe 2
            match (event.effects.[0], event.effects.[1]) with 
            | (Effect.PlayerRemoved f1, Effect.PlayerRemoved f2) ->
                f1.playerId |> shouldBe userPlayer.id
                f2.playerId |> shouldBe guestPlayer.id

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
            
            let adminSession = session |> TestUtilities.setSessionPrivileges [EditPendingGames]

            let! userPlayer =
                managers.players.addPlayer game.id userPlayerRequest adminSession
                |> thenMap (fun resp -> resp.game.players |> List.except game.players |> List.head)
                |> thenValue

            let! guestPlayer =
                managers.players.addPlayer game.id guestPlayerRequest adminSession
                |> thenMap (fun resp -> resp.game.players |> List.except (userPlayer :: game.players) |> List.head)
                |> thenValue

            let! game = GameRepository.getGame game.id |> thenValue

            //Act
            let event = services.players.getRemovePlayerEvent (game, guestPlayer.id) session |> Result.value

            //Assert
            event.kind |> shouldBe EventKind.PlayerRemoved
            event.effects.Length |> shouldBe 1
            match event.effects.[0] with 
            | Effect.PlayerRemoved f ->
                f.playerId |> shouldBe guestPlayer.id

            | _ -> failwith "Incorrect effects"
        }
    
    [<Fact>]
    let ``Should cancel game if creating user and game is pending``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false) |> thenValue
            let creator = game.players |> List.head

            //Act
            let event = services.players.getRemovePlayerEvent (game, creator.id) session |> Result.value

            //Assert
            event.kind |> shouldBe EventKind.PlayerRemoved
            event.effects.Length |> shouldBe 2
            match (event.effects.[0], event.effects.[1]) with 
            | (Effect.PlayerRemoved f1, Effect.GameStatusChanged f2) ->
                f1.playerId |> shouldBe creator.id

                f2.oldValue |> shouldBe GameStatus.Pending
                f2.newValue |> shouldBe GameStatus.Canceled

            | _ -> failwith "Incorrect effects"
        }
           
    [<Fact>]
    let ``Should fali if game InProgress``() =
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true) |> thenValue

            let p2Request = 
                {
                    userId = Some user.id
                    kind = PlayerKind.Guest
                    name = Some "p2"
                }

            let! _ = managers.players.addPlayer game.id p2Request session |> thenValue

            let! resp = managers.games.startGame game.id session |> thenValue
            let game = resp.game

            //Act
            let result = services.players.getRemovePlayerEvent (game, game.players.[0].id) session

            //Assert
            result |> shouldBeError 400 "Cannot remove players unless game is Pending."
        }