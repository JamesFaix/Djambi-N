namespace Apex.Api.IntegrationTests.Logic.PlayerService

open FSharp.Control.Tasks
open Xunit
open Apex.Api.Common.Control
open Apex.Api.IntegrationTests
open Apex.Api.Model
open Apex.Api.Logic.Interfaces
open Apex.Api.Db.Interfaces
open Apex.Api.Enums
open Apex.Api.Logic.Services

type GetAddPlayerEventTests() =
    inherit TestsBase()

    //USER PLAYER

    let assertSuccess (eventRequest : CreateEventRequest) (request : CreatePlayerRequest) : unit =
        eventRequest.kind |> shouldBe EventKind.PlayerJoined
        eventRequest.effects.Length |> shouldBe 1
        eventRequest.effects.[0] |> shouldBe (PlayerAddedEffect.fromRequest request)

    [<Fact>]
    let ``Add user player should work if adding self``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (_, _, game) = createuserSessionAndGame(false)

            let! user = createUser()
            let session = getSessionForUser user
            let request = CreatePlayerRequest.user user

            //Act
            let eventRequest = host.Get<PlayerService>().getAddPlayerEvent (game, request) session |> Result.value

            //Assert
            assertSuccess eventRequest request
        }

    [<Fact>]
    let ``Add user player should work if EditPendingGames and adding different user``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false)
            let session = session |> TestUtilities.setSessionPrivileges [Privilege.EditPendingGames]
            let! user = createUser()
            let request = CreatePlayerRequest.user user

            //Act
            let eventRequest = host.Get<PlayerService>().getAddPlayerEvent (game, request) session |> Result.value

            //Assert
            assertSuccess eventRequest request
        }

    [<Fact>]
    let ``Add user player should fail if not EditPendingGames and adding different user``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false)
            let session = session |> TestUtilities.setSessionPrivileges []

            let! user = createUser()
            let request = CreatePlayerRequest.user user

            //Act
            let error = host.Get<PlayerService>().getAddPlayerEvent (game, request) session

            //Assert
            error |> shouldBeError 403 "Cannot add other users to a game."
        }

    [<Fact>]
    let ``Add user player should fail if not passing user id``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false)
            let session = session |> TestUtilities.setSessionPrivileges [Privilege.EditPendingGames]

            let! user = createUser()

            let request : CreatePlayerRequest =
                {
                    userId = None
                    name = None
                    kind = PlayerKind.User
                }

            //Act
            let error = host.Get<PlayerService>().getAddPlayerEvent (game, request) session

            //Assert
            error |> shouldBeError 400 "UserID must be provided when adding a user player."
        }

    [<Fact>]
    let ``Add user player should fail if passing name``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false)
            let session = session |> TestUtilities.setSessionPrivileges [Privilege.EditPendingGames]

            let! user = createUser()    

            let request : CreatePlayerRequest =
                {
                    userId = Some user.id
                    name = Some "test"
                    kind = PlayerKind.User
                }

            //Act
            let error = host.Get<PlayerService>().getAddPlayerEvent (game, request) session

            //Assert
            error |> shouldBeError 400 "Cannot provide name when adding a user player."
        }

    [<Fact>]
    let ``Add user player should fail if user already in lobby``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(false)
            let session = session |> TestUtilities.setSessionPrivileges [Privilege.EditPendingGames]
            let request = CreatePlayerRequest.user user

            //Act
            let error = host.Get<PlayerService>().getAddPlayerEvent (game, request) session

            //Assert
            error |> shouldBeError 409 "User is already a player."
        }

    //GUEST PLAYER

    [<Fact>]
    let ``Add guest player should work if adding guest to self``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true)
            let request = CreatePlayerRequest.guest (user.id, "test")

            //Act
            let eventRequest = host.Get<PlayerService>().getAddPlayerEvent (game, request) session |> Result.value

            //Assert
            assertSuccess eventRequest request
        }

    [<Fact>]
    let ``Add guest player should work if EditPendingGames and adding guest to different user``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(true)
            let session = session |> TestUtilities.setSessionPrivileges [Privilege.EditPendingGames]

            let! user = createUser()
            let request = CreatePlayerRequest.guest (user.id, "test")

            //Act
            let eventRequest = host.Get<PlayerService>().getAddPlayerEvent (game, request) session |> Result.value

            //Assert
            assertSuccess eventRequest request
        }

    [<Fact>]
    let ``Add guest player should fail if not EditPendingGames and adding guest to different user``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(true)
            let session = session |> TestUtilities.setSessionPrivileges []

            let! user = createUser()
            let request = CreatePlayerRequest.guest (user.id, "test")

            //Act
            let error = host.Get<PlayerService>().getAddPlayerEvent (game, request) session

            //Assert
            error |> shouldBeError 403 "Cannot add guests for other users to a game."
        }

    [<Fact>]
    let ``Add guest player should fail if not passing user id``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(true)

            let request : CreatePlayerRequest =
                {
                    userId = None
                    name = Some "test"
                    kind = PlayerKind.Guest
                }

            //Act
            let error = host.Get<PlayerService>().getAddPlayerEvent (game, request) session

            //Assert
            error |> shouldBeError 400 "UserID must be provided when adding a guest player."
        }

    [<Fact>]
    let ``Add guest player should fail if not passing name``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true)

            let request : CreatePlayerRequest =
                {
                    userId = Some user.id
                    name = None
                    kind = PlayerKind.Guest
                }

            //Act
            let error = host.Get<PlayerService>().getAddPlayerEvent (game, request) session

            //Assert
            error |> shouldBeError 400 "Must provide name when adding a guest player."
        }

    [<Fact>]
    let ``Add guest player should fail if duplicate name``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true)
            let request = CreatePlayerRequest.guest (user.id, user.name)

            //Act
            let error = host.Get<PlayerService>().getAddPlayerEvent (game, request) session

            //Assert
            error |> shouldBeError 409 "A player with that name already exists."
        }

    [<Fact>]
    let ``Add guest player should fail if lobby does not allow guests``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(false)
            let request = CreatePlayerRequest.guest (user.id, "test")

            //Act
            let error = host.Get<PlayerService>().getAddPlayerEvent (game, request) session

            //Assert
            error |> shouldBeError 400 "Game does not allow guest players."
        }

    //NEUTRAL PLAYER

    [<Fact>]
    let ``Add neutral player should fail``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (_, session, game) = createuserSessionAndGame(false)
            let session = session |> TestUtilities.setSessionPrivileges [Privilege.EditPendingGames]
            let request = CreatePlayerRequest.neutral ("test")

            //Act
            let error = host.Get<PlayerService>().getAddPlayerEvent (game, request) session

            //Assert
            error |> shouldBeError 400 "Cannot directly add neutral players to a game."
        }

    //GENERAL

    [<Fact>]
    let ``Add player should fail if player count at capacity``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true)

            let request1 = CreatePlayerRequest.guest (user.id, "test")
            let request2 = { request1 with name = Some "test2" }
            let request3 = { request1 with name = Some "test3" }

            let! _ = host.Get<IPlayerManager>().addPlayer game.id request1 session
            let! _ = host.Get<IPlayerManager>().addPlayer game.id request2 session

            let! game = host.Get<IGameRepository>().getGame game.id

            //Act
            let error = host.Get<PlayerService>().getAddPlayerEvent (game, request3) session

            //Assert
            error |> shouldBeError 400 "Max player count reached."
        }

    [<Fact>]
    let ``Add player should fail if game already InProgress``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true)
            let request = CreatePlayerRequest.guest (user.id, "test")
            let game = { game with status = GameStatus.InProgress }

            //Act
            let error = host.Get<PlayerService>().getAddPlayerEvent (game, request) session

            //Assert
            error |> shouldBeError 400 "Can only add players to pending games."
        }