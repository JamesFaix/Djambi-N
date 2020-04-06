namespace Apex.Api.IntegrationTests.Logic.GameStartService

open FSharp.Control.Tasks
open Xunit
open Apex.Api.Common.Control.AsyncHttpResult
open Apex.Api.IntegrationTests
open Apex.Api.Model
open Apex.Api.Logic
open Apex.Api.Db.Interfaces
open Apex.Api.Enums
open Apex.Api.Logic.Services
open System.Threading.Tasks
open Apex.Api.Common.Control

type GetGameStartEventsTests() =
    inherit TestsBase()

    let createUserSessionAndGameWith3Players() =
        let host = HostFactory.createHost()
        task {
            let! (user, session, game) = createuserSessionAndGame(true)

            let player2request =
                { TestUtilities.getCreatePlayerRequest with
                    userId = Some user.id
                    kind = PlayerKind.Guest
                    name = Some "p2"
                }
            let! _ = host.Get<IGameRepository>().addPlayer (game.id, player2request)

            let player3request = { player2request with name = Some "p3" }
            let! _ = host.Get<IGameRepository>().addPlayer (game.id, player3request)

            let! game = host.Get<IGameRepository>().getGame game.id

            return Ok (user, session, game)
        }

    [<Fact>]
    let ``Should fail not creator or EditPendingGames privilege``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (user, session, game) = createUserSessionAndGameWith3Players() |> thenValue
            let session = session |> TestUtilities.setSessionUserId (session.user.id+1)

            //Act/Assert
            let! ex = Assert.ThrowsAsync<HttpException>(fun () ->
                host.Get<GameStartService>().getGameStartEvents game session :> Task
            )

            ex.statusCode |> shouldBe 403
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
            let! (user, session, game) = createUserSessionAndGameWith3Players() |> thenValue
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
            let! (user, session, game) = createUserSessionAndGameWith3Players() |> thenValue

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

            let p2Request =
                {
                    userId = Some user.id
                    kind = PlayerKind.Guest
                    name = Some "p2"
                }

            let! _ = host.Get<IGameRepository>().addPlayer(game.id, p2Request)
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