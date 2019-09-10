namespace Djambi.Api.IntegrationTests.Logic.GameStartService

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.IntegrationTests
open Djambi.Api.Model
open Djambi.Api.Logic

type GetGameStartEventsTests() =
    inherit TestsBase()

    let createUserSessionAndGameWith3Players() =
        task {
            let! (user, session, game) = createuserSessionAndGame(true) |> thenValue

            let player2request =
                { TestUtilities.getCreatePlayerRequest with
                    userId = Some user.id
                    kind = PlayerKind.Guest
                    name = Some "p2"
                }
            let! _ = db.games.addPlayer (game.id, player2request) |> thenValue

            let player3request = { player2request with name = Some "p3" }
            let! _ = db.games.addPlayer (game.id, player3request) |> thenValue

            let! game = db.games.getGame game.id |> thenValue

            return Ok (user, session, game)
        }

    [<Fact>]
    let ``Should fail not creator or EditPendingGames privilege``() =
        task {
            //Arrange
            let! (user, session, game) = createUserSessionAndGameWith3Players() |> thenValue
            let session = session |> TestUtilities.setSessionUserId (session.user.id+1)

            //Act
            let! result = services.gameStart.getGameStartEvents game session

            //Assert
            result |> shouldBeError 403 Security.noPrivilegeOrCreatorErrorMessage
        }

    [<Fact>]
    let ``Should fail if only one player``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(true) |> thenValue

            //Act
            let! result = services.gameStart.getGameStartEvents game session

            //Assert
            result |> shouldBeError 400 "Cannot start game with only one player."
        }

    [<Fact>]
    let ``Should work if EditPendingGames privilege``() =
        task {
            //Arrange
            let! (user, session, game) = createUserSessionAndGameWith3Players() |> thenValue
            let session = session |> TestUtilities.setSessionUserId (session.user.id+1)
                                  |> TestUtilities.setSessionPrivileges [EditPendingGames]

            //Act
            let! events = services.gameStart.getGameStartEvents game session |> thenValue

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
        task {
            //Arrange
            let! (user, session, game) = createUserSessionAndGameWith3Players() |> thenValue

            //Act
            let! events = services.gameStart.getGameStartEvents game session |> thenValue

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
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true) |> thenValue

            let p2Request =
                {
                    userId = Some user.id
                    kind = PlayerKind.Guest
                    name = Some "p2"
                }

            let! _ = db.games.addPlayer(game.id, p2Request)
            let! game = db.games.getGame game.id |> thenValue

            //Act
            let! events = services.gameStart.getGameStartEvents game session |> thenValue
            
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