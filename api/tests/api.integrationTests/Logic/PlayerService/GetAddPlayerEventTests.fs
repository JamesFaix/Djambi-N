namespace Djambi.Api.IntegrationTests.Logic.PlayerService

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.IntegrationTests
open Djambi.Api.Logic.Services
open Djambi.Api.Model
open Djambi.Api.Logic.Managers
open Djambi.Api.Db.Repositories

type GetAddPlayerEventTests() =
    inherit TestsBase()

    //USER PLAYER

    let assertSuccess (eventRequest : CreateEventRequest) (request : CreatePlayerRequest) : unit =        
        eventRequest.kind |> shouldBe EventKind.PlayerJoined
        eventRequest.effects.Length |> shouldBe 1

        match eventRequest.effects.[0] with
        | Effect.PlayerAdded f ->
            f.value |> shouldBe request

        | _ -> failwith "Incorrect effects"

    [<Fact>]
    let ``Add user player should work if adding self``() =
        task {
            //Arrange
            let! (_, _, game) = createuserSessionAndGame(false) |> thenValue

            let! user = createUser() |> thenValue
            let session = getSessionForUser user.id
            let request = CreatePlayerRequest.user user.id

            //Act
            let eventRequest = PlayerService.getAddPlayerEvent (game, request) session |> Result.value

            //Assert
            assertSuccess eventRequest request
        }

    [<Fact>]
    let ``Add user player should work if admin and adding different user``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false) |> thenValue
            let session = session |> TestUtilities.setSessionIsAdmin true
            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.user user.id
            
            //Act
            let eventRequest = PlayerService.getAddPlayerEvent (game, request) session |> Result.value

            //Assert
            assertSuccess eventRequest request
        }

    [<Fact>]
    let ``Add user player should fail if not admin and adding different user``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false) |> thenValue
            let session = session |> TestUtilities.setSessionIsAdmin false

            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.user user.id

            //Act
            let error = PlayerService.getAddPlayerEvent (game, request) session

            //Assert
            error |> shouldBeError 403 "Cannot add other users to a game."
        }

    [<Fact>]
    let ``Add user player should fail if not passing user id``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false) |> thenValue
            let session = session |> TestUtilities.setSessionIsAdmin true

            let! user = createUser() |> thenValue

            let request : CreatePlayerRequest =
                {
                    userId = None
                    name = None
                    kind = PlayerKind.User
                }

            //Act
            let error = PlayerService.getAddPlayerEvent (game, request) session

            //Assert
            error |> shouldBeError 400 "UserID must be provided when adding a user player."
        }

    [<Fact>]
    let ``Add user player should fail if passing name``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false) |> thenValue
            let session = session |> TestUtilities.setSessionIsAdmin true

            let! user = createUser() |> thenValue

            let request : CreatePlayerRequest =
                {
                    userId = Some user.id
                    name = Some "test"
                    kind = PlayerKind.User
                }

            //Act
            let error = PlayerService.getAddPlayerEvent (game, request) session

            //Assert
            error |> shouldBeError 400 "Cannot provide name when adding a user player."
        }

    [<Fact>]
    let ``Add user player should fail if user already in lobby``() =
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(false) |> thenValue
            let session = session |> TestUtilities.setSessionIsAdmin true
            let request = CreatePlayerRequest.user user.id

            //Act
            let error = PlayerService.getAddPlayerEvent (game, request) session

            //Assert
            error |> shouldBeError 409 "User is already a player."
        }

    //GUEST PLAYER

    [<Fact>]
    let ``Add guest player should work if adding guest to self``() =
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true) |> AsyncHttpResult.thenValue
            let request = CreatePlayerRequest.guest (user.id, "test")
                        
            //Act
            let eventRequest = PlayerService.getAddPlayerEvent (game, request) session |> Result.value

            //Assert
            assertSuccess eventRequest request
        }

    [<Fact>]
    let ``Add guest player should work if admin and adding guest to different user``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(true) |> thenValue
            let session = session |> TestUtilities.setSessionIsAdmin true

            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.guest (user.id, "test")
                        
            //Act
            let eventRequest = PlayerService.getAddPlayerEvent (game, request) session |> Result.value

            //Assert
            assertSuccess eventRequest request
        }

    [<Fact>]
    let ``Add guest player should fail if not admin and adding guest to different user``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(true) |> thenValue
            let session = session |> TestUtilities.setSessionIsAdmin false

            let! user = createUser() |> thenValue
            let request = CreatePlayerRequest.guest (user.id, "test")

            //Act
            let error = PlayerService.getAddPlayerEvent (game, request) session

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
            let error = PlayerService.getAddPlayerEvent (game, request) session

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
            let error = PlayerService.getAddPlayerEvent (game, request) session

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
            let error = PlayerService.getAddPlayerEvent (game, request) session

            //Assert
            error |> shouldBeError 409 "A player with that name already exists."
        }

    [<Fact>]
    let ``Add guest player should fail if lobby does not allow guests``() =
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(false) |> thenValue
            let request = CreatePlayerRequest.guest (user.id, "test")

            //Act
            let error = PlayerService.getAddPlayerEvent (game, request) session

            //Assert
            error |> shouldBeError 400 "Game does not allow guest players."
        }

    //NEUTRAL PLAYER

    [<Fact>]
    let ``Add neutral player should fail``() =
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false) |> thenValue
            let session = session |> TestUtilities.setSessionIsAdmin true
            let request = CreatePlayerRequest.neutral ("test")

            //Act
            let error = PlayerService.getAddPlayerEvent (game, request) session

            //Assert
            error |> shouldBeError 400 "Cannot directly add neutral players to a game."
        }

    //GENERAL

    [<Fact>]
    let ``Add player should fail if player count at capacity``() =
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true) |> thenValue

            let request1 = CreatePlayerRequest.guest (user.id, "test")
            let request2 = { request1 with name = Some "test2" }
            let request3 = { request1 with name = Some "test3" }

            let! _ = GameManager.addPlayer game.id request1 session |> thenValue
            let! _ = GameManager.addPlayer game.id request2 session |> thenValue

            let! game = GameRepository.getGame game.id |> thenValue

            //Act
            let error = PlayerService.getAddPlayerEvent (game, request3) session

            //Assert
            error |> shouldBeError 400 "Max player count reached."
        }

    [<Fact>]
    let ``Add player should fail if game already started``() =
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true) |> thenValue
            let request = CreatePlayerRequest.guest (user.id, "test")
            let game = { game with status = GameStatus.Started }

            //Act
            let error = PlayerService.getAddPlayerEvent (game, request) session

            //Assert
            error |> shouldBeError 400 "Can only add players to pending games."
        }