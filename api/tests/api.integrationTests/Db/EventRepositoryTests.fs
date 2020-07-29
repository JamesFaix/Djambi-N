namespace Apex.Api.IntegrationTests.Db

open System.ComponentModel
open System.Threading.Tasks
open FSharp.Control.Tasks
open Xunit
open Apex.Api.Common.Control
open Apex.Api.Db.Interfaces
open Apex.Api.Enums
open Apex.Api.IntegrationTests
open Apex.Api.Logic.Interfaces
open Apex.Api.Logic.Services
open Apex.Api.Model

type EventRepositoryTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Should add players``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (user, _, game) = TestUtilities.createuserSessionAndGame(true)

            let player : Player =
                {
                    id = 0
                    gameId = game.id
                    userId = Some user.id
                    kind = PlayerKind.Guest
                    name = "p2"
                    status = PlayerStatus.Pending
                    colorId = None
                    startingRegion = None
                    startingTurnNumber = None
                }

            let newGame = { game with players = [player] }

            //Act
            let! resp = host.Get<IEventRepository>().persistEvent (TestUtilities.emptyEventRequest(user.id), game, newGame)

            //Assert
            let persistedGame = resp.game

            { newGame with players = persistedGame.players } |> shouldBe persistedGame

            persistedGame.players.Length |> shouldBe 1

            let persistedPlayer = persistedGame.players.Head
            persistedPlayer.id |> shouldNotBe 0
            { persistedPlayer with id = player.id } |> shouldBe player
        }

    [<Fact>]
    let ``Should remove players``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (user, _, game) = TestUtilities.createuserSessionAndGame(true)

            let player =
                {
                    userId = Some user.id
                    kind = PlayerKind.Guest
                    name = Some "p2"
                } |> CreatePlayerRequest.toPlayer None

            let! _ = host.Get<IPlayerRepository>().addPlayer(game.id, player)
            let! game = host.Get<IGameRepository>().getGame game.id

            let newGame = { game with players = [] }

            //Act
            let! resp = host.Get<IEventRepository>().persistEvent (TestUtilities.emptyEventRequest(user.id), game, newGame)

            //Assert
            let persistedGame = resp.game

            { newGame with players = persistedGame.players } |> shouldBe persistedGame

            persistedGame.players.Length |> shouldBe 0
        }

    [<Fact>]
    let ``Should update players``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (user, _, game) = TestUtilities.createuserSessionAndGame(true)

            let p2 =
                {
                    userId = Some user.id
                    kind = PlayerKind.Guest
                    name = Some "p2"
                } |> CreatePlayerRequest.toPlayer None

            let! _ = host.Get<IPlayerRepository>().addPlayer(game.id, p2)
            let! game = host.Get<IGameRepository>().getGame game.id

            let oldP2 = game.players.[1]
            oldP2.status |> shouldBe PlayerStatus.Pending
            oldP2.startingRegion |> shouldBe None
            oldP2.startingTurnNumber |> shouldBe None
            oldP2.colorId |> shouldBe None

            let newP2 =
                { oldP2 with
                    status = PlayerStatus.Alive
                    startingRegion = Some 1
                    startingTurnNumber = Some 1
                    colorId = Some 1
                }

            let newGame =
                { game with
                    players = [
                        game.players.[0]
                        newP2
                    ]
                }

            //Act
            let! resp = host.Get<IEventRepository>().persistEvent (TestUtilities.emptyEventRequest(user.id), game, newGame)

            //Assert
            let persistedGame = resp.game

            { newGame with players = persistedGame.players } |> shouldBe persistedGame

            persistedGame.players.Length |> shouldBe 2
            persistedGame.players.[0] |> shouldBe game.players.[0]
            persistedGame.players.[1] |> shouldBe newP2
        }

    [<Fact>]
    let ``Should update game``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (user, _, game) = TestUtilities.createuserSessionAndGame(false)

            game.status |> shouldBe GameStatus.Pending
            game.parameters |> shouldBe
                {
                    allowGuests = false
                    isPublic = false
                    regionCount = 3
                    description = Some "Test"
                }
            game.currentTurn |> shouldBe None
            game.turnCycle |> shouldBe []
            game.pieces |> shouldBe []

            let newGame =
                { game with
                    status = GameStatus.Canceled
                    parameters =
                        {
                            allowGuests = true
                            isPublic = true
                            regionCount = 8
                            description = None
                        }
                    currentTurn = Some Turn.empty
                    turnCycle = [ 1 ]
                    pieces = [
                        {
                            id = 0
                            kind = PieceKind.Hunter
                            cellId = 0
                            playerId = None
                            originalPlayerId = 0
                        }
                    ]
                }

            //Act
            let! resp = host.Get<IEventRepository>().persistEvent (TestUtilities.emptyEventRequest(user.id), game, newGame)

            //Assert
            let persistedGame = resp.game
            persistedGame |> shouldBe newGame
        }

    [<Fact>]
    let ``Should rollback all effects if any errors``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (user, _, game) = TestUtilities.createuserSessionAndGame(true)

            //Attempt to add 2 players with the same name
            //This will violate a unique index in SQL and fail the second player

            let e = Effect.NeutralPlayerAdded { name = "test"; placeholderPlayerId = -1 }
            let event = TestUtilities.createEventRequest(user.id)([e;e]) //EventKind doesn't matter

            let newGame = host.Get<EventService>().applyEvent game event

            //Act/Assert
            let! ex = Assert.ThrowsAsync<HttpException>(fun () -> 
                host.Get<IEventRepository>().persistEvent (TestUtilities.emptyEventRequest(user.id), game, newGame)
                :> Task
            )

            ex.statusCode |> shouldBe 409
            ex.Message |> shouldBe "Conflict when attempting to write Event."

            let host = HostFactory.createHost() // Must create a new host because the DbContext's tracked changes are now in a corrupt state
            let! persistedGame = host.Get<IGameRepository>().getGame game.id
            persistedGame.players.Length |> shouldBe 1 //Just the creator
            persistedGame.players |> shouldNotExist (fun p -> p.name = "test")
        }

    [<Fact>]
    let ``Should get events``() =
        let host = HostFactory.createHost()
        task {
            //Arrange
            let! (user, session, game) = TestUtilities.createuserSessionAndGame(true)

            let p2Request =
                {
                    userId = Some user.id
                    name = Some "p2"
                    kind = PlayerKind.Guest
                }

            let p3Request = { p2Request with name = Some "p3" }

            let! _ = host.Get<IPlayerManager>().addPlayer game.id p2Request session
            let! _ = host.Get<IPlayerManager>().addPlayer game.id p3Request session

            let query : EventsQuery =
                {
                    maxResults = None
                    direction = ListSortDirection.Ascending
                    thresholdEventId = None
                    thresholdTime = None
                }

            //Act
            let! events = host.Get<IEventRepository>().getEvents (game.id, query)

            //Assert
            events.Length |> shouldBe 2

            let e1 = events.[0]
            e1.kind |> shouldBe EventKind.PlayerJoined
            e1.createdBy.userId |> shouldBe user.id
            e1.createdBy.userName |> shouldBe user.name
            e1.effects.Length |> shouldBe 1

            e1.effects.[0] |> shouldBe (PlayerAddedEffect.fromRequest p2Request)

            let e2 = events.[1]
            e2.kind |> shouldBe EventKind.PlayerJoined
            e2.createdBy.userId |> shouldBe user.id
            e2.createdBy.userName |> shouldBe user.name
            e2.effects.Length |> shouldBe 1

            e2.effects.[0] |> shouldBe (PlayerAddedEffect.fromRequest p3Request)
        }