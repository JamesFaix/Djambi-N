namespace Apex.Api.IntegrationTests.Logic.GameStartService

open System
open System.Threading.Tasks
open FSharp.Control.Tasks
open Xunit
open Apex.Api.Common.Control
open Apex.Api.Db.Interfaces
open Apex.Api.Enums
open Apex.Api.IntegrationTests
open Apex.Api.Logic
open Apex.Api.Logic.Services
open Apex.Api.Model

type GetGameStartEventsTests() =
    inherit TestsBase()

    let createUserSessionAndGameWith3Players() =
        let host = HostFactory.createHost()
        task {
            let! (user, session, game) = createuserSessionAndGame(true)

            let player2 =
                { TestUtilities.getCreatePlayerRequest with
                    userId = Some user.id
                    kind = PlayerKind.Guest
                    name = Some "p2"
                } |> CreatePlayerRequest.toPlayer None
            let! _ = host.Get<IPlayerRepository>().addPlayer (game.id, player2)

            let player3 = { player2 with name = "p3" }
            let! _ = host.Get<IPlayerRepository>().addPlayer (game.id, player3)

            let! game = host.Get<IGameRepository>().getGame game.id

            return user, session, game
        }

    [<Fact>]
    let ``Should fail not creator or EditPendingGames privilege``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (user, session, game) = createUserSessionAndGameWith3Players()
            let session = session |> TestUtilities.setSessionUserId (session.user.id+1)

            //Act/Assert
            let! ex = Assert.ThrowsAsync<UnauthorizedAccessException>(fun () ->
                host.Get<GameStartService>().getGameStartEvents game session :> Task
            )

            ex.Message |> shouldBe Security.noPrivilegeOrCreatorErrorMessage
        }

    [<Fact>]
    let ``Should fail if only one player``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(true)

            //Act/Assert
            let! ex = Assert.ThrowsAsync<HttpException>(fun () ->
                host.Get<GameStartService>().getGameStartEvents game session :> Task
            )

            ex.statusCode |> shouldBe 400
            ex.Message |> shouldBe "Cannot start game with only one player."
        }

    [<Fact>]
    let ``Should work if EditPendingGames privilege``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (user, session, game) = createUserSessionAndGameWith3Players()
            let session = session |> TestUtilities.setSessionUserId (session.user.id+1)
                                  |> TestUtilities.setSessionPrivileges [Privilege.EditPendingGames]

            //Act
            let! events = host.Get<GameStartService>().getGameStartEvents game session

            //Assert
            let (addNeutralPlayers, startGame) = events
            addNeutralPlayers |> shouldBeNone

            startGame.kind |> shouldBe EventKind.GameStarted
            startGame.effects.Length |> shouldBe 1
            match startGame.effects.[0] with
            | Effect.GameStatusChanged f ->
                f.oldValue |> shouldBe GameStatus.Pending
                f.newValue |> shouldBe GameStatus.InProgress

            | _ -> failwith "Incorrect effects"
        }

    [<Fact>]
    let ``Should work if creator``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (user, session, game) = createUserSessionAndGameWith3Players()

            //Act
            let! events = host.Get<GameStartService>().getGameStartEvents game session

            //Assert
            let (addNeutralPlayers, startGame) = events
            addNeutralPlayers |> shouldBeNone

            startGame.kind |> shouldBe EventKind.GameStarted
            startGame.effects.Length |> shouldBe 1
            match startGame.effects.[0] with
            | Effect.GameStatusChanged f ->
                f.oldValue |> shouldBe GameStatus.Pending
                f.newValue |> shouldBe GameStatus.InProgress

            | _ -> failwith "Incorrect effects"
        }

    [<Fact>]
    let ``Should add players if not at capacity``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true)

            let p2 =
                {
                    userId = Some user.id
                    kind = PlayerKind.Guest
                    name = Some "p2"
                } |> CreatePlayerRequest.toPlayer None

            let! _ = host.Get<IPlayerRepository>().addPlayer(game.id, p2)
            let! game = host.Get<IGameRepository>().getGame game.id

            //Act
            let! events = host.Get<GameStartService>().getGameStartEvents game session
            
            //Assert
            let (addNeutralPlayers, startGame) = events
            addNeutralPlayers |> shouldBeSome
            addNeutralPlayers.Value.effects.Length |> shouldBe 1
            match addNeutralPlayers.Value.effects.[0] with
            | Effect.NeutralPlayerAdded f ->
                f.placeholderPlayerId |> shouldBeLessThan 0
            | _ -> failwith "Incorrect effects"

            startGame.kind |> shouldBe EventKind.GameStarted
            startGame.effects.Length |> shouldBe 1
            match startGame.effects.[0] with
            | Effect.GameStatusChanged f ->
                f.oldValue |> shouldBe GameStatus.Pending
                f.newValue |> shouldBe GameStatus.InProgress

            | _ -> failwith "Incorrect effects"
        }