namespace Apex.Api.IntegrationTests.Logic.playerServ

open System
open FSharp.Control.Tasks
open Xunit
open Apex.Api.Common.Control
open Apex.Api.Common.Control.AsyncHttpResult
open Apex.Api.IntegrationTests
open Apex.Api.Model
open Apex.Api.Db.Interfaces
open Apex.Api.Logic.Interfaces
open Apex.Api.Enums

type GetRemovePlayerEventTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Should fail if removing player not in game``() =
        task {
            //Arrange
            let! (_, session, game1) = createuserSessionAndGame(false) |> thenValue
            let session = session |> TestUtilities.setSessionPrivileges [Privilege.EditPendingGames]

            let gameRequest = getGameParameters()
            let! game2 = (gameMan :> IGameManager).createGame gameRequest session |> thenValue

            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.user user.id

            let! player = (gameMan :> IPlayerManager).addPlayer game1.id request session
                          |> thenMap (fun resp -> resp.game.players |> List.except game1.players |> List.head)
                          |> thenValue

            //Act
            let error = playerServ.getRemovePlayerEvent (game2, player.id) session

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
            let! neutralPlayer = (gameRepo :> IGameRepository).addPlayer(game.id, request) |> thenValue
            let! game = (gameRepo :> IGameRepository).getGame game.id |> thenValue

            //Act
            let error = playerServ.getRemovePlayerEvent (game, neutralPlayer.id) session

            //Assert
            error |> shouldBeError 400 "Cannot remove neutral players from game."
        }

    [<Fact>]
    let ``Should fail if removing different user and not EditPendingGames or creator``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false) |> thenValue
            let adminSession = session |> TestUtilities.setSessionPrivileges [Privilege.EditPendingGames]

            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.user user.id

            let session = session |> TestUtilities.setSessionPrivileges []
                                  |> TestUtilities.setSessionUserId Int32.MinValue

            let! player = (gameMan :> IPlayerManager).addPlayer game.id request adminSession
                          |> thenMap (fun resp -> resp.game.players |> List.except game.players |> List.head)
                          |> thenValue

            let! game = (gameRepo :> IGameRepository).getGame game.id |> thenValue

            //Act
            let error = playerServ.getRemovePlayerEvent (game, player.id) session

            //Assert
            error |> shouldBeError 403 "Cannot remove other users from game."
        }

    [<Fact>]
    let ``Should work if removing guest of self``() =
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true) |> thenValue
            let request = CreatePlayerRequest.guest (user.id, "test")

            let! player = (gameMan :> IPlayerManager).addPlayer game.id request session
                          |> thenMap (fun resp -> resp.game.players |> List.except game.players |> List.head)
                          |> thenValue

            let! game = (gameRepo :> IGameRepository).getGame game.id |> thenValue

            //Act
            let event = playerServ.getRemovePlayerEvent (game, player.id) session |> Result.value

            //Assert
            event.kind |> shouldBe EventKind.PlayerRemoved
            event.effects.Length |> shouldBe 1
            match event.effects.[0] with
            | Effect.PlayerRemoved f ->
                f.oldPlayer.id |> shouldBe player.id

            | _ -> failwith "Incorrect effects"
        }

    [<Fact>]
    let ``Should work if EditPendingGames and removing different user``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false) |> thenValue
            let session = session |> TestUtilities.setSessionPrivileges [Privilege.EditPendingGames]

            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.user user.id

            let! player = (gameMan :> IPlayerManager).addPlayer game.id request session
                          |> thenMap (fun resp -> resp.game.players |> List.except game.players |> List.head)
                          |> thenValue

            let! game = (gameRepo :> IGameRepository).getGame game.id |> thenValue

            //Act
            let event = playerServ.getRemovePlayerEvent (game, player.id) session |> Result.value

            //Assert
            event.kind |> shouldBe EventKind.PlayerRemoved
            event.effects.Length |> shouldBe 1
            match event.effects.[0] with
            | Effect.PlayerRemoved f ->
                f.oldPlayer.id |> shouldBe player.id

            | _ -> failwith "Incorrect effects"
        }

    [<Fact>]
    let ``Should work if creator and removing different user``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false) |> thenValue

            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.user user.id

            let adminSession = session |> TestUtilities.setSessionPrivileges [Privilege.EditPendingGames]
            let! player = (gameMan :> IPlayerManager).addPlayer game.id request adminSession
                          |> thenMap (fun resp -> resp.game.players |> List.except game.players |> List.head)
                          |> thenValue

            let! game = (gameRepo :> IGameRepository).getGame game.id |> thenValue

            //Act
            let event = playerServ.getRemovePlayerEvent (game, player.id) session |> Result.value

            //Assert
            event.kind |> shouldBe EventKind.PlayerRemoved
            event.effects.Length |> shouldBe 1
            match event.effects.[0] with
            | Effect.PlayerRemoved f ->
                f.oldPlayer.id |> shouldBe player.id

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

            let adminSession = session |> TestUtilities.setSessionPrivileges [Privilege.EditPendingGames]

            let! userPlayer =
                (gameMan :> IPlayerManager).addPlayer game.id userPlayerRequest adminSession
                |> thenMap (fun resp -> resp.game.players |> List.except game.players |> List.head)
                |> thenValue

            let! guestPlayer =
                (gameMan :> IPlayerManager).addPlayer game.id guestPlayerRequest adminSession
                |> thenMap (fun resp -> resp.game.players |> List.except (userPlayer :: game.players) |> List.head)
                |> thenValue

            let! game = (gameRepo :> IGameRepository).getGame game.id |> thenValue

            let event = playerServ.getRemovePlayerEvent (game, userPlayer.id) session |> Result.value

            //Assert
            event.kind |> shouldBe EventKind.PlayerRemoved
            event.effects.Length |> shouldBe 2
            match (event.effects.[0], event.effects.[1]) with
            | (Effect.PlayerRemoved f1, Effect.PlayerRemoved f2) ->
                f1.oldPlayer.id |> shouldBe userPlayer.id
                f2.oldPlayer.id |> shouldBe guestPlayer.id

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

            let adminSession = session |> TestUtilities.setSessionPrivileges [Privilege.EditPendingGames]

            let! userPlayer =
                (gameMan :> IPlayerManager).addPlayer game.id userPlayerRequest adminSession
                |> thenMap (fun resp -> resp.game.players |> List.except game.players |> List.head)
                |> thenValue

            let! guestPlayer =
                (gameMan :> IPlayerManager).addPlayer game.id guestPlayerRequest adminSession
                |> thenMap (fun resp -> resp.game.players |> List.except (userPlayer :: game.players) |> List.head)
                |> thenValue

            let! game = (gameRepo :> IGameRepository).getGame game.id |> thenValue

            //Act
            let event = playerServ.getRemovePlayerEvent (game, guestPlayer.id) session |> Result.value

            //Assert
            event.kind |> shouldBe EventKind.PlayerRemoved
            event.effects.Length |> shouldBe 1
            match event.effects.[0] with
            | Effect.PlayerRemoved f ->
                f.oldPlayer.id |> shouldBe guestPlayer.id

            | _ -> failwith "Incorrect effects"
        }

    [<Fact>]
    let ``Should cancel game if creating user and game is pending``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false) |> thenValue
            let creator = game.players |> List.head

            //Act
            let event = playerServ.getRemovePlayerEvent (game, creator.id) session |> Result.value

            //Assert
            event.kind |> shouldBe EventKind.PlayerRemoved
            event.effects.Length |> shouldBe 2
            match (event.effects.[0], event.effects.[1]) with
            | (Effect.PlayerRemoved f1, Effect.GameStatusChanged f2) ->
                f1.oldPlayer.id |> shouldBe creator.id

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

            let! _ = (gameMan :> IPlayerManager).addPlayer game.id p2Request session |> thenValue

            let! resp = (gameMan :> IGameManager).startGame game.id session |> thenValue
            let game = resp.game

            //Act
            let result = playerServ.getRemovePlayerEvent (game, game.players.[0].id) session

            //Assert
            result |> shouldBeError 400 "Cannot remove players unless game is Pending."
        }