namespace Djambi.Api.IntegrationTests.Logic.PlayerService

open System
open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.IntegrationTests
open Djambi.Api.Logic.Services
open Djambi.Api.Model

type AddPlayerTests() =
    inherit TestsBase()

    //USER PLAYER

    [<Fact>]
    let ``Add user player should work if adding self``() =
        task {
            //Arrange
            let! (_, _, game) = createuserSessionAndGame(false) |> thenValue

            let! user = createUser() |> thenValue
            let session = getSessionForUser user.id
            let request = CreatePlayerRequest.user user.id

            //Act
            let! player = PlayerService.addPlayer (game.id, request) session |> thenValue

            //Assert
            player.id |> shouldNotBe 0
            player.gameId |> shouldBe game.id
            player.name |> shouldBe user.name
            player.userId |> shouldBe (Some user.id)
            player.kind |> shouldBe PlayerKind.User
        }

    [<Fact>]
    let ``Add user player should work if admin and adding different user``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false) |> thenValue

            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.user user.id

            //Act
            let! player = PlayerService.addPlayer (game.id, request) { session with isAdmin = true } |> thenValue

            //Assert
            player.id |> shouldNotBe 0
            player.gameId |> shouldBe game.id
            player.name |> shouldBe user.name
            player.userId |> shouldBe (Some user.id)
            player.kind |> shouldBe PlayerKind.User
        }

    [<Fact>]
    let ``Add user player should fail if not admin and adding different user``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false) |> thenValue

            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.user user.id

            //Act
            let! error = PlayerService.addPlayer (game.id, request) { session with isAdmin = false }

            //Assert
            error |> shouldBeError 403 "Cannot add other users to a game."
        }

    [<Fact>]
    let ``Add user player should fail if not passing user id``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false) |> thenValue

            let! user = createUser() |> thenValue

            let request : CreatePlayerRequest =
                {
                    userId = None
                    name = None
                    kind = PlayerKind.User
                }

            //Act
            let! error = PlayerService.addPlayer (game.id, request) { session with isAdmin = true }

            //Assert
            error |> shouldBeError 400 "UserID must be provided when adding a user player."
        }

    [<Fact>]
    let ``Add user player should fail if passing name``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false) |> thenValue

            let! user = createUser() |> thenValue

            let request : CreatePlayerRequest =
                {
                    userId = Some user.id
                    name = Some "test"
                    kind = PlayerKind.User
                }

            //Act
            let! error = PlayerService.addPlayer (game.id, request) { session with isAdmin = true }

            //Assert
            error |> shouldBeError 400 "Cannot provide name when adding a user player."
        }

    [<Fact>]
    let ``Add user player should fail if user already in lobby``() =
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(false) |> thenValue
            let request = CreatePlayerRequest.user user.id

            //Act
            let! error = PlayerService.addPlayer (game.id, request) { session with isAdmin = true }

            //Assert
            error |> shouldBeError 409 "Conflict when attempting to write Player."
        }

    //GUEST PLAYER

    [<Fact>]
    let ``Add guest player should work if adding guest to self``() =
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true) |> AsyncHttpResult.thenValue
            let request = CreatePlayerRequest.guest (user.id, "test")

            //Act
            let! player = PlayerService.addPlayer (game.id, request) session |> thenValue

            //Assert
            player.id |> shouldNotBe 0
            player.gameId |> shouldBe game.id
            player.name |> shouldBe request.name.Value
            player.userId |> shouldBe (Some user.id)
            player.kind |> shouldBe PlayerKind.Guest
        }

    [<Fact>]
    let ``Add guest player should work if admin and adding guest to different user``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(true) |> thenValue

            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.guest (user.id, "test")

            //Act
            let! player = PlayerService.addPlayer (game.id, request) { session with isAdmin = true }
                          |> thenValue

            //Assert
            player.id |> shouldNotBe 0
            player.gameId |> shouldBe game.id
            player.name |> shouldBe request.name.Value
            player.userId |> shouldBe (Some user.id)
            player.kind |> shouldBe PlayerKind.Guest
        }

    [<Fact>]
    let ``Add guest player should fail if not admin and adding guest to different user``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(true) |> thenValue

            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.guest (user.id, "test")

            //Act
            let! error = PlayerService.addPlayer (game.id, request) { session with isAdmin = false }

            //Assert
            error |> shouldBeError 403 "Cannot add guests for other users to a game."
        }

    [<Fact>]
    let ``Add guest player should fail if not passing user id``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(true) |> thenValue

            let request : CreatePlayerRequest =
                {
                    userId = None
                    name = Some "test"
                    kind = PlayerKind.Guest
                }

            //Act
            let! error = PlayerService.addPlayer (game.id, request) session

            //Assert
            error |> shouldBeError 400 "UserID must be provided when adding a guest player."
        }

    [<Fact>]
    let ``Add guest player should fail if not passing name``() =
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true) |> thenValue

            let request : CreatePlayerRequest =
                {
                    userId = Some user.id
                    name = None
                    kind = PlayerKind.Guest
                }

            //Act
            let! error = PlayerService.addPlayer (game.id, request) session

            //Assert
            error |> shouldBeError 400 "Must provide name when adding a guest player."
        }

    [<Fact>]
    let ``Add guest player should fail if duplicate name``() =
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true) |> thenValue
            let request = CreatePlayerRequest.guest (user.id, user.name)

            //Act
            let! error = PlayerService.addPlayer (game.id, request) session

            //Assert
            error |> shouldBeError 409 "Conflict when attempting to write Player."
        }

    [<Fact>]
    let ``Add guest player should fail if lobby does not allow guests``() =
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(false) |> thenValue
            let request = CreatePlayerRequest.guest (user.id, "test")

            //Act
            let! error = PlayerService.addPlayer (game.id, request) session

            //Assert
            error |> shouldBeError 400 "Game does not allow guest players."
        }

    //NEUTRAL PLAYER

    [<Fact>]
    let ``Add neutral player should fail``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false) |> thenValue
            let request = CreatePlayerRequest.neutral ("test")

            //Act
            let! error = PlayerService.addPlayer (game.id, request) { session with isAdmin = true }

            //Assert
            error |> shouldBeError 400 "Cannot directly add neutral players to a game."
        }

    //GENERAL

    [<Fact>]
    let ``Add player should fail if invalid lobby id``() =
        task {
            //Arrange
            let! (user, session, _) = createuserSessionAndGame(true) |> thenValue
            let request = CreatePlayerRequest.guest (user.id, "test")

            //Act
            let! error = PlayerService.addPlayer (Int32.MinValue, request) { session with isAdmin = true }

            //Assert
            error |> shouldBeError 404 "Game not found."
        }

    [<Fact>]
    let ``Add player should fail if lobby at capacity``() =
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true) |> thenValue

            let request1 = CreatePlayerRequest.guest (user.id, "test")
            let request2 = { request1 with name = Some "test2" }
            let request3 = { request1 with name = Some "test3" }

            let! _ = PlayerService.addPlayer (game.id, request1) session |> thenValue
            let! _ = PlayerService.addPlayer (game.id, request2) session |> thenValue

            //Act
            let! error = PlayerService.addPlayer (game.id, request3) session

            //Assert
            error |> shouldBeError 400 "Max player count reached."
        }

    [<Fact>]
    let ``Add player should fail if game already started``() =
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true) |> thenValue
            let request1 = CreatePlayerRequest.guest (user.id, "test")
            let request2 = { request1 with name = Some "test2" }

            let! _ = PlayerService.addPlayer (game.id, request1) session |> thenValue
            let! _ = GameStartService.startGame game.id session |> thenValue

            //Act
            let! error = PlayerService.addPlayer (game.id, request2) session

            //Assert
            error |> shouldBeError 400 "Can only add players to pending games."
        }