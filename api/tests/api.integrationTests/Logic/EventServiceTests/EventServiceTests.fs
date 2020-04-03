namespace Apex.Api.IntegrationTests.Logic.EventServiceTests

open Xunit
open Apex.Api.IntegrationTests
open Apex.Api.Model
open Apex.Api.Enums
open Apex.Api.Logic.Services
open FSharp.Control.Tasks
open Apex.Api.Common.Control

//TODO: Move to unit test project

type EventServiceTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Should apply effects in order``() =
        let host = HostFactory.createHost()
        //Arrange
        task {
            let game = TestUtilities.defaultGame
            let! user = createUser() |> AsyncHttpResult.thenValue
            let effects =
                [
                    //If the status InProgress is used here additional setup is required to make `game` valid
                    Effect.GameStatusChanged { oldValue = GameStatus.Pending; newValue = GameStatus.Canceled }
                    Effect.GameStatusChanged { oldValue = GameStatus.Canceled; newValue = GameStatus.Over }
                ]
            let eventRequest = TestUtilities.createEventRequest(user.id)(effects) //Kind doesn't matter

            //Act
            let newGame = host.Get<EventService>().applyEvent game eventRequest

            //Assert
            newGame.status |> shouldBe GameStatus.Over
        }
    [<Fact>]
    let ``Should apply CurrentTurnChanged effect``() =
        let host = HostFactory.createHost()
        //Arrange
        task {
            let game = TestUtilities.defaultGame
            let! user = createUser() |> AsyncHttpResult.thenValue
            let newTurn = Some Turn.empty
            let effect = Effect.CurrentTurnChanged { oldValue = game.currentTurn; newValue = newTurn }
            let eventRequest = TestUtilities.createEventRequest(user.id)([effect]) //Kind doesn't matter

            game.currentTurn |> shouldBe None

            //Act
            let newGame = host.Get<EventService>().applyEvent game eventRequest

            //Assert
            { game with currentTurn = newGame.currentTurn } |> shouldBe newGame

            newGame.currentTurn |> shouldBe newTurn
        }
    [<Fact>]
    let ``Should apply GameStatusChanged effect``() =
        let host = HostFactory.createHost()
        //Arrange
        task {
            let game = TestUtilities.defaultGame
            let! user = createUser() |> AsyncHttpResult.thenValue
            let newStatus = GameStatus.Canceled //Can't use InProgress here because that case is more complicated
            let effect = Effect.GameStatusChanged { oldValue = game.status; newValue = newStatus }
            let eventRequest = TestUtilities.createEventRequest(user.id)([effect]) //Kind doesn't matter

            game.status |> shouldBe GameStatus.Pending

            //Act
            let newGame = host.Get<EventService>().applyEvent game eventRequest

            //Assert
            { game with status = newGame.status } |> shouldBe newGame

            newGame.status |> shouldBe newStatus
        }
    [<Fact>]
    let ``Should apply NeutralPlayerAdded effect``() =
        let host = HostFactory.createHost()
        //Arrange
        task {
            let game = { TestUtilities.defaultGame with id = 5 }
            let! user = createUser() |> AsyncHttpResult.thenValue
            let effect = Effect.NeutralPlayerAdded { name = "p2"; placeholderPlayerId = -1 }
            let eventRequest = TestUtilities.createEventRequest(user.id)([effect]) //Kind doesn't matter

            game.players.Length |> shouldBe 0

            //Act
            let newGame = host.Get<EventService>().applyEvent game eventRequest

            //Assert
            { game with players = newGame.players } |> shouldBe newGame

            newGame.players.Length |> shouldBe 1
            let p = newGame.players.Head
            p.id |> shouldBe -1 //The real ID is generated when the event is persisted
            p.gameId |> shouldBe newGame.id
            p.userId |> shouldBe None
            p.kind |> shouldBe PlayerKind.Neutral
            p.name |> shouldBe "p2"

            //These are assigned at game start
            p.status |> shouldBe PlayerStatus.Pending
            p.colorId |> shouldBe None
            p.startingRegion |> shouldBe None
            p.startingTurnNumber |> shouldBe None
        }
    [<Fact>]
    let ``Should apply ParametersChanged effect``() =
        let host = HostFactory.createHost()
        //Arrange
        task {
            let game = TestUtilities.defaultGame
            let! user = createUser() |> AsyncHttpResult.thenValue
            let newParameters =
                {
                    allowGuests = true
                    isPublic = true
                    description = Some "test"
                    regionCount = 8
                }
            let effect = Effect.ParametersChanged { oldValue = game.parameters; newValue = newParameters }
            let eventRequest = TestUtilities.createEventRequest(user.id)([effect]) //Kind doesn't matter

            game.parameters |> shouldBe
                {
                    allowGuests = false
                    isPublic = false
                    description = None
                    regionCount = 0
                }

            //Act
            let newGame = host.Get<EventService>().applyEvent game eventRequest

            //Assert
            { game with parameters = newGame.parameters } |> shouldBe newGame

            newGame.parameters |> shouldBe newParameters
        }
    [<Fact>]
    let ``Should apply PieceAbandoned effect``() =
        let host = HostFactory.createHost()
        //Arrange
        task {
            let pieces : Piece list =
                [
                    {
                        id = 1
                        kind = PieceKind.Hunter
                        playerId = Some 0
                        originalPlayerId = 0
                        cellId = 0
                    }
                    {
                        id = 2
                        kind = PieceKind.Hunter
                        playerId = Some 0
                        originalPlayerId = 0
                        cellId = 0
                    }
                ]
            let game = { TestUtilities.defaultGame with pieces = pieces}
            let! user = createUser() |> AsyncHttpResult.thenValue
            let effect = Effect.PieceAbandoned { oldPiece = pieces.[0] }
            let eventRequest = TestUtilities.createEventRequest(user.id)([effect]) //Kind doesn't matter

            //Act
            let newGame = host.Get<EventService>().applyEvent game eventRequest

            //Assert
            { game with pieces = newGame.pieces } |> shouldBe newGame

            newGame.pieces.Length |> shouldBe 2

            newGame.pieces.[0] |> shouldBe { pieces.[0] with playerId = None }
            newGame.pieces.[1] |> shouldBe pieces.[1]
        }

    [<Fact>]
    let ``Should apply PieceDropped effect``() =
        let host = HostFactory.createHost()
        //Arrange
        task {
            let piece : Piece =
                {
                    id = 1
                    kind = PieceKind.Hunter
                    playerId = Some 0
                    originalPlayerId = 0
                    cellId = 0
                }
            let! user = createUser() |> AsyncHttpResult.thenValue
            let game = { TestUtilities.defaultGame with pieces = [piece]}
            let newPiece = { piece with cellId = 3 }
            let effect = Effect.PieceDropped { oldPiece = piece; newPiece = newPiece }
            let eventRequest = TestUtilities.createEventRequest(user.id)([effect]) //Kind doesn't matter

            //Act
            let newGame = host.Get<EventService>().applyEvent game eventRequest

            //Assert
            { game with pieces = newGame.pieces } |> shouldBe newGame

            newGame.pieces.Length |> shouldBe 1

            let newPiece = newGame.pieces.Head
            newPiece |> shouldBe { piece with cellId = newPiece.cellId }
        }

    [<Fact>]
    let ``Should apply PieceEnlisted effect``() =
        let host = HostFactory.createHost()
        //Arrange
        task {
            let pieces : Piece list =
                [
                    {
                        id = 1
                        kind = PieceKind.Hunter
                        playerId = Some 0
                        originalPlayerId = 0
                        cellId = 0
                    }
                    {
                        id = 2
                        kind = PieceKind.Hunter
                        playerId = Some 0
                        originalPlayerId = 0
                        cellId = 0
                    }
                ]
            let game = { TestUtilities.defaultGame with pieces = pieces}
            let! user = createUser() |> AsyncHttpResult.thenValue
            let newPlayerId = 1
            let effect = Effect.PieceEnlisted { oldPiece = pieces.[0]; newPlayerId = newPlayerId }
            let eventRequest = TestUtilities.createEventRequest(user.id)([effect]) //Kind doesn't matter

            //Act
            let newGame = host.Get<EventService>().applyEvent game eventRequest

            //Assert
            { game with pieces = newGame.pieces } |> shouldBe newGame

            newGame.pieces.Length |> shouldBe 2

            newGame.pieces.[0] |> shouldBe { pieces.[0] with playerId = Some newPlayerId }
            newGame.pieces.[1] |> shouldBe pieces.[1]
        }

    [<Fact>]
    let ``Should apply PieceKilled effect``() =
        let host = HostFactory.createHost()
        //Arrange
        task {
            let piece : Piece =
                {
                    id = 1
                    kind = PieceKind.Hunter
                    playerId = Some 0
                    originalPlayerId = 0
                    cellId = 0
                }
            let game = { TestUtilities.defaultGame with pieces = [piece]}
            let! user = createUser() |> AsyncHttpResult.thenValue
            let effect = Effect.PieceKilled {
                oldPiece = piece
            }
            let eventRequest = TestUtilities.createEventRequest(user.id)([effect]) //Kind doesn't matter

            //Act
            let newGame = host.Get<EventService>().applyEvent game eventRequest

            //Assert
            { game with pieces = newGame.pieces } |> shouldBe newGame

            newGame.pieces.Length |> shouldBe 1

            let newPiece = newGame.pieces.Head
            newPiece |> shouldBe { piece with kind = PieceKind.Corpse; playerId = None }
        }

    [<Fact>]
    let ``Should apply PieceMoved effect``() =
        let host = HostFactory.createHost()
        //Arrange
        task {
            let piece : Piece =
                {
                    id = 1
                    kind = PieceKind.Hunter
                    playerId = Some 0
                    originalPlayerId = 0
                    cellId = 0
                }
            let game = { TestUtilities.defaultGame with pieces = [piece]}
            let! user = createUser() |> AsyncHttpResult.thenValue
            let newCellId = 3
            let effect = Effect.PieceMoved { oldPiece = piece; newCellId = newCellId }
            let eventRequest = TestUtilities.createEventRequest(user.id)([effect]) //Kind doesn't matter

            //Act
            let newGame = host.Get<EventService>().applyEvent game eventRequest

            //Assert
            { game with pieces = newGame.pieces } |> shouldBe newGame

            newGame.pieces.Length |> shouldBe 1

            let newPiece = newGame.pieces.Head
            newPiece |> shouldBe { piece with cellId = newCellId }
        }

    [<Fact>]
    let ``Should apply PIeceVacated effect``() =
        let host = HostFactory.createHost()
        //Arrange
        task {
            let piece : Piece =
                {
                    id = 1
                    kind = PieceKind.Hunter
                    playerId = Some 0
                    originalPlayerId = 0
                    cellId = 0
                }
            let! user = createUser() |> AsyncHttpResult.thenValue
            let game = { TestUtilities.defaultGame with pieces = [piece]}
            let newCellId = 3
            let effect = Effect.PieceVacated { oldPiece = piece; newCellId = newCellId }
            let eventRequest = TestUtilities.createEventRequest(user.id)([effect]) //Kind doesn't matter

            //Act
            let newGame = host.Get<EventService>().applyEvent game eventRequest

            //Assert
            { game with pieces = newGame.pieces } |> shouldBe newGame

            newGame.pieces.Length |> shouldBe 1

            let newPiece = newGame.pieces.Head
            newPiece |> shouldBe { piece with cellId = newCellId }
        }

    [<Fact>]
    let ``Should apply PlayerAdded effect``() =
        let host = HostFactory.createHost()
        //Arrange
        task {
            let game = { TestUtilities.defaultGame with id = 5 }
            let! user = createUser() |> AsyncHttpResult.thenValue
            let playerRequest = CreatePlayerRequest.user(user)
            let effect = PlayerAddedEffect.fromRequest playerRequest
            let eventRequest = TestUtilities.createEventRequest(user.id)([effect]) //Kind doesn't matter

            game.players.Length |> shouldBe 0

            //Act
            let newGame = host.Get<EventService>().applyEvent game eventRequest

            //Assert
            { game with players = newGame.players } |> shouldBe newGame

            newGame.players.Length |> shouldBe 1
            let p = newGame.players.Head
            p.id |> shouldBe 0 //This is generated when the event is persisted
            p.gameId |> shouldBe newGame.id
            p.userId |> shouldBe playerRequest.userId
            p.kind |> shouldBe playerRequest.kind
            p.name |> shouldBe "" //This is pulled from the db for User players

            //These are assigned at game start
            p.status |> shouldBe PlayerStatus.Pending
            p.colorId |> shouldBe None
            p.startingRegion |> shouldBe None
            p.startingTurnNumber |> shouldBe None
        }
    [<Fact>]
    let ``Should apply PlayerStatusChanged effect``() =
        let host = HostFactory.createHost()
        //Arrange
        task {
            let player : Player =
                {
                    id = 1
                    kind = PlayerKind.User
                    name = "test"
                    gameId = 0
                    userId = None
                    status = PlayerStatus.Pending
                    colorId = None
                    startingRegion = None
                    startingTurnNumber = None
                }
            let! user = createUser() |> AsyncHttpResult.thenValue
            let game = { TestUtilities.defaultGame with players = [player] }
            let effect = Effect.PlayerStatusChanged {
                    playerId = player.id
                    oldStatus = player.status
                    newStatus = PlayerStatus.Eliminated
                }
            let eventRequest = TestUtilities.createEventRequest(user.id)([effect]) //Kind doesn't matter

            //Act
            let newGame = host.Get<EventService>().applyEvent game eventRequest

            //Assert
            { game with players = newGame.players } |> shouldBe newGame

            newGame.players.Length |> shouldBe 1

            let newPlayer = newGame.players.Head
            newPlayer |> shouldBe { player with status = PlayerStatus.Eliminated }
        }

    [<Fact>]
    let ``Should apply PlayersRemoved effect``() =
        let host = HostFactory.createHost()
        //Arrange
        task {
            let players : Player list =
                [
                    {
                        id = 1
                        kind = PlayerKind.User
                        name = "p1"
                        gameId = 0
                        userId = None
                        status = PlayerStatus.Pending
                        colorId = None
                        startingRegion = None
                        startingTurnNumber = None
                    }
                    {
                        id = 2
                        kind = PlayerKind.User
                        name = "p2"
                        gameId = 0
                        userId = None
                        status = PlayerStatus.Pending
                        colorId = None
                        startingRegion = None
                        startingTurnNumber = None
                    }
                ]
            let! user = createUser() |> AsyncHttpResult.thenValue
            let game = { TestUtilities.defaultGame with players = players}
            let effect = Effect.PlayerRemoved { oldPlayer = players.[0] }
            let eventRequest = TestUtilities.createEventRequest(user.id)([effect]) //Kind doesn't matter

            //Act
            let newGame = host.Get<EventService>().applyEvent game eventRequest

            //Assert
            { game with players = newGame.players } |> shouldBe newGame

            newGame.players.Length |> shouldBe 1
            newGame.players.Head |> shouldBe players.[1]
        }

    [<Fact>]
    let ``Should apply TurnCycleAdvanced effect``() =
        let host = HostFactory.createHost()
        //Arrange
        task {
            let game = TestUtilities.defaultGame
            let! user = createUser() |> AsyncHttpResult.thenValue
            let newCycle = [1;2;3]
            let effect = Effect.TurnCycleAdvanced { oldValue = game.turnCycle; newValue = newCycle }
            let eventRequest = TestUtilities.createEventRequest(user.id)([effect]) //Kind doesn't matter

            game.turnCycle |> shouldBe []

            //Act
            let newGame = host.Get<EventService>().applyEvent game eventRequest

            //Assert
            { game with turnCycle = newGame.turnCycle } |> shouldBe newGame

            newGame.turnCycle |> shouldBe newCycle
        }

    [<Fact>]
    let ``Should apply TurnCyclePlayerFellFromPower effect``() =
        let host = HostFactory.createHost()
        //Arrange
        task {
            let game = TestUtilities.defaultGame
            let! user = createUser() |> AsyncHttpResult.thenValue
            let newCycle = [1;2;3]
            let effect = Effect.TurnCyclePlayerFellFromPower { oldValue = game.turnCycle; newValue = newCycle; playerId = 1 } //PlayerID is just informative, doesn't effect processing
            let eventRequest = TestUtilities.createEventRequest(user.id)([effect]) //Kind doesn't matter

            game.turnCycle |> shouldBe []

            //Act
            let newGame = host.Get<EventService>().applyEvent game eventRequest

            //Assert
            { game with turnCycle = newGame.turnCycle } |> shouldBe newGame

            newGame.turnCycle |> shouldBe newCycle
        }

    [<Fact>]
    let ``Should apply TurnCyclePlayerRemoved effect``() =
        let host = HostFactory.createHost()
        //Arrange
        task {
            let! user = createUser() |> AsyncHttpResult.thenValue
            let game = TestUtilities.defaultGame
            let newCycle = [1;2;3]
            let effect = Effect.TurnCyclePlayerRemoved { oldValue = game.turnCycle; newValue = newCycle; playerId = 1 } //PlayerID is just informative, doesn't effect processing
            let eventRequest = TestUtilities.createEventRequest(user.id)([effect]) //Kind doesn't matter

            game.turnCycle |> shouldBe []

            //Act
            let newGame = host.Get<EventService>().applyEvent game eventRequest

            //Assert
            { game with turnCycle = newGame.turnCycle } |> shouldBe newGame

            newGame.turnCycle |> shouldBe newCycle
        }

    [<Fact>]
    let ``Should apply TurnCyclePlayerRoseToPower effect``() =
        let host = HostFactory.createHost()
        //Arrange
        task {
            let game = TestUtilities.defaultGame
            let! user = createUser() |> AsyncHttpResult.thenValue
            let newCycle = [1;2;3]
            let effect = Effect.TurnCyclePlayerRoseToPower { oldValue = game.turnCycle; newValue = newCycle; playerId = 1 } //PlayerID is just informative, doesn't effect processing
            let eventRequest = TestUtilities.createEventRequest(user.id)([effect]) //Kind doesn't matter

            game.turnCycle |> shouldBe []

            //Act
            let newGame = host.Get<EventService>().applyEvent game eventRequest

            //Assert
            { game with turnCycle = newGame.turnCycle } |> shouldBe newGame

            newGame.turnCycle |> shouldBe newCycle
        }