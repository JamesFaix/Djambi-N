namespace Apex.Api.IntegrationTests.Logic.PlayerService

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
open Apex.Api.Logic.Services

type GetRemovePlayerEventTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Should fail if removing player not in game``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (_, session, game1) = createuserSessionAndGame(false)
            let session = session |> TestUtilities.setSessionPrivileges [Privilege.EditPendingGames]

            let gameRequest = getGameParameters()
            let! game2 = host.Get<IGameManager>().createGame gameRequest session

            let! user = createUser()
            let request = CreatePlayerRequest.user user

            let! response = host.Get<IPlayerManager>().addPlayer game1.id request session
            let player = response.game.players |> List.except game1.players |> List.head

            //Act
            let error = host.Get<PlayerService>().getRemovePlayerEvent (game2, player.id) session

            //Assert
            error |> shouldBeError 404 "Player not found."
        }

    [<Fact>]
    let ``Should fail if removing neutral player``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false)

            let request =
                {
                    kind = PlayerKind.Neutral
                    name = Some "test"
                    userId = None
                }
            let! neutralPlayer = host.Get<IGameRepository>().addPlayer(game.id, request)
            let! game = host.Get<IGameRepository>().getGame game.id

            //Act
            let error = host.Get<PlayerService>().getRemovePlayerEvent (game, neutralPlayer.id) session

            //Assert
            error |> shouldBeError 400 "Cannot remove neutral players from game."
        }

    [<Fact>]
    let ``Should fail if removing different user and not EditPendingGames or creator``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false)
            let adminSession = session |> TestUtilities.setSessionPrivileges [Privilege.EditPendingGames]

            let! user = createUser()
            let request = CreatePlayerRequest.user user

            let session = session |> TestUtilities.setSessionPrivileges []
                                  |> TestUtilities.setSessionUserId Int32.MinValue

            let! response = host.Get<IPlayerManager>().addPlayer game.id request adminSession
            let player = response.game.players |> List.except game.players |> List.head

            let! game = host.Get<IGameRepository>().getGame game.id

            //Act
            let error = host.Get<PlayerService>().getRemovePlayerEvent (game, player.id) session

            //Assert
            error |> shouldBeError 403 "Cannot remove other users from game."
        }

    [<Fact>]
    let ``Should work if removing guest of self``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true)
            let request = CreatePlayerRequest.guest (user.id, "test")

            let! response = host.Get<IPlayerManager>().addPlayer game.id request session
            let player = response.game.players |> List.except game.players |> List.head

            let! game = host.Get<IGameRepository>().getGame game.id

            //Act
            let event = host.Get<PlayerService>().getRemovePlayerEvent (game, player.id) session |> Result.value

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
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false)
            let session = session |> TestUtilities.setSessionPrivileges [Privilege.EditPendingGames]

            let! user = createUser()
            let request = CreatePlayerRequest.user user

            let! response = host.Get<IPlayerManager>().addPlayer game.id request session
            let player = response.game.players |> List.except game.players |> List.head

            let! game = host.Get<IGameRepository>().getGame game.id

            //Act
            let event = host.Get<PlayerService>().getRemovePlayerEvent (game, player.id) session |> Result.value

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
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false)

            let! user = createUser()
            let request = CreatePlayerRequest.user user

            let adminSession = session |> TestUtilities.setSessionPrivileges [Privilege.EditPendingGames]
            let! response = host.Get<IPlayerManager>().addPlayer game.id request adminSession
            let player = response.game.players |> List.except game.players |> List.head

            let! game = host.Get<IGameRepository>().getGame game.id

            //Act
            let event = host.Get<PlayerService>().getRemovePlayerEvent (game, player.id) session |> Result.value

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
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(true)

            let! user = createUser()
            let userPlayerRequest = CreatePlayerRequest.user user
            let guestPlayerRequest = CreatePlayerRequest.guest (user.id, "test")

            let adminSession = session |> TestUtilities.setSessionPrivileges [Privilege.EditPendingGames]

            let! response = host.Get<IPlayerManager>().addPlayer game.id userPlayerRequest adminSession
            let userPlayer = response.game.players |> List.except game.players |> List.head

            let! response = host.Get<IPlayerManager>().addPlayer game.id guestPlayerRequest adminSession
            let guestPlayer = response.game.players |> List.except (userPlayer :: game.players) |> List.head

            let! game = host.Get<IGameRepository>().getGame game.id

            let event = host.Get<PlayerService>().getRemovePlayerEvent (game, userPlayer.id) session |> Result.value

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
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(true)

            let! user = createUser()
            let userPlayerRequest = CreatePlayerRequest.user user
            let guestPlayerRequest = CreatePlayerRequest.guest (user.id, "test")

            let adminSession = session |> TestUtilities.setSessionPrivileges [Privilege.EditPendingGames]

            let! response = host.Get<IPlayerManager>().addPlayer game.id userPlayerRequest adminSession
            let userPlayer = response.game.players |> List.except game.players |> List.head

            let! response = host.Get<IPlayerManager>().addPlayer game.id guestPlayerRequest adminSession
            let guestPlayer = response.game.players |> List.except (userPlayer :: game.players) |> List.head

            let! game = host.Get<IGameRepository>().getGame game.id

            //Act
            let event = host.Get<PlayerService>().getRemovePlayerEvent (game, guestPlayer.id) session |> Result.value

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
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false)
            let creator = game.players |> List.head

            //Act
            let event = host.Get<PlayerService>().getRemovePlayerEvent (game, creator.id) session |> Result.value

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
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true)

            let p2Request =
                {
                    userId = Some user.id
                    kind = PlayerKind.Guest
                    name = Some "p2"
                }

            let! _ = host.Get<IPlayerManager>().addPlayer game.id p2Request session

            let! resp = host.Get<IGameManager>().startGame game.id session
            let game = resp.game

            //Act
            let result = host.Get<PlayerService>().getRemovePlayerEvent (game, game.players.[0].id) session

            //Assert
            result |> shouldBeError 400 "Cannot remove players unless game is Pending."
        }