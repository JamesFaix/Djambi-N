namespace Djambi.Api.IntegrationTests.Logic.EventServiceTests

open Xunit
open Djambi.Api.IntegrationTests
open Djambi.Api.Model
open Djambi.Api.Logic.Services

//TODO: Move to unit test project

type EventServiceTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Should apply effects in order``() =
        //Arrange
        let game = TestUtilities.defaultGame

        let effects = 
            [
                //If the status Started is used here additional setup is required to make `game` valid
                Effect.gameStatusChanged(GameStatus.Pending, GameStatus.AbortedWhilePending)
                Effect.gameStatusChanged(GameStatus.AbortedWhilePending, GameStatus.Aborted)
            ]

        let event = Event.create(EventKind.GameStarted, effects) //Doesn't matter what kind
        
        //Act
        let newGame = EventService.applyEvent game event

        //Assert
        newGame.status |> shouldBe GameStatus.Aborted

    [<Fact>]
    let ``Should apply AddPlayer effect``() =
        //Arrange
        let game = { TestUtilities.defaultGame with id = 5 }
        let userId = 1
        let playerRequest = CreatePlayerRequest.user(userId)
        let effect = Effect.playerAdded(playerRequest)
        let event = Event.create(EventKind.GameStarted, [effect])

        game.players.Length |> shouldBe 0

        //Act
        let newGame = EventService.applyEvent game event

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
        p.isAlive |> shouldBe None
        p.colorId |> shouldBe None
        p.startingRegion |> shouldBe None
        p.startingTurnNumber |> shouldBe None

    [<Fact>]
    let ``Should apply CurrentTurnChanged effect``() =
        //Arrange
        let game = TestUtilities.defaultGame
        let newTurn = Some Turn.empty
        let effect = Effect.currentTurnChanged(game.currentTurn, newTurn)
        let event = Event.create(EventKind.GameStarted, [effect]) //Kind doesn't matter

        game.currentTurn |> shouldBe None

        //Act
        let newGame = EventService.applyEvent game event

        //Assert
        { game with currentTurn = newGame.currentTurn } |> shouldBe newGame

        newGame.currentTurn |> shouldBe newTurn
            
    [<Fact>]
    let ``Should apply GameStatusChanged effect``() =
        //Arrange
        let game = TestUtilities.defaultGame
        let newStatus = GameStatus.AbortedWhilePending //Can't use Started here because that case is more complicated
        let effect = Effect.gameStatusChanged(game.status, newStatus)
        let event = Event.create(EventKind.GameStarted, [effect]) //Kind doesn't matter

        game.status |> shouldBe GameStatus.Pending

        //Act
        let newGame = EventService.applyEvent game event

        //Assert
        { game with status = newGame.status } |> shouldBe newGame

        newGame.status |> shouldBe newStatus
        
    [<Fact>]
    let ``Should apply ParametersChanged effect``() =
        //Arrange
        let game = TestUtilities.defaultGame
        let newParameters = 
            {
                allowGuests = true
                isPublic = true
                description = Some "test"
                regionCount = 8
            }
        let effect = Effect.parametersChanged(game.parameters, newParameters)
        let event = Event.create(EventKind.GameStarted, [effect]) //Kind doesn't matter

        game.parameters |> shouldBe 
            {
                allowGuests = false
                isPublic = false
                description = None
                regionCount = 0
            }

        //Act
        let newGame = EventService.applyEvent game event

        //Assert
        { game with parameters = newGame.parameters } |> shouldBe newGame

        newGame.parameters |> shouldBe newParameters   

    [<Fact>]
    let ``Should apply PieceKilled effect``() =
        //Arrange
        let piece : Piece = 
            {
                id = 1
                kind = PieceKind.Assassin
                playerId = Some 0
                originalPlayerId = 0
                cellId = 0
            }
        let game = { TestUtilities.defaultGame with pieces = [piece]}

        let effect = Effect.pieceKilled(piece.id)
        let event = Event.create(EventKind.GameStarted, [effect]) //Kind doesn't matter

        //Act
        let newGame = EventService.applyEvent game event

        //Assert
        { game with pieces = newGame.pieces } |> shouldBe newGame

        newGame.pieces.Length |> shouldBe 1
        
        let newPiece = newGame.pieces.Head
        newPiece |> shouldBe { piece with kind = PieceKind.Corpse; playerId = None }

    [<Fact>]
    let ``Should apply PieceMoved effect``() =
        //Arrange
        let piece : Piece = 
            {
                id = 1
                kind = PieceKind.Assassin
                playerId = Some 0
                originalPlayerId = 0
                cellId = 0
            }
        let game = { TestUtilities.defaultGame with pieces = [piece]}
        let newCellId = 3
        let effect = Effect.pieceMoved(piece.id, piece.cellId, newCellId)
        let event = Event.create(EventKind.GameStarted, [effect]) //Kind doesn't matter

        //Act
        let newGame = EventService.applyEvent game event

        //Assert
        { game with pieces = newGame.pieces } |> shouldBe newGame

        newGame.pieces.Length |> shouldBe 1
        
        let newPiece = newGame.pieces.Head
        newPiece |> shouldBe { piece with cellId = newCellId }

    [<Fact>]
    let ``Should apply PiecesOwnershipChanged effect``() =
        //Arrange
        let pieces : Piece list =
            [
                {
                    id = 1
                    kind = PieceKind.Assassin
                    playerId = Some 0
                    originalPlayerId = 0
                    cellId = 0
                }
                {
                    id = 2
                    kind = PieceKind.Assassin
                    playerId = Some 0
                    originalPlayerId = 0
                    cellId = 0
                }
                {
                    id = 3
                    kind = PieceKind.Assassin
                    playerId = Some 0
                    originalPlayerId = 0
                    cellId = 0
                }
            ]
        let game = { TestUtilities.defaultGame with pieces = pieces}
        let newPlayerId = Some 1
        let effect = Effect.piecesOwnershipChanged([1;2], pieces.[0].playerId, newPlayerId)
        let event = Event.create(EventKind.GameStarted, [effect]) //Kind doesn't matter

        //Act
        let newGame = EventService.applyEvent game event

        //Assert
        { game with pieces = newGame.pieces } |> shouldBe newGame

        newGame.pieces.Length |> shouldBe 3
        
        newGame.pieces.[0] |> shouldBe { pieces.[0] with playerId = newPlayerId }
        newGame.pieces.[1] |> shouldBe { pieces.[1] with playerId = newPlayerId }
        newGame.pieces.[2] |> shouldBe pieces.[2]

    [<Fact>]
    let ``Should apply PlayerEliminated effect``() =
        //Arrange
        let player : Player = 
            {
                id = 1
                kind = PlayerKind.User
                name = "test"
                gameId = 0
                userId = None
                isAlive = None
                colorId = None
                startingRegion = None
                startingTurnNumber = None
            }
        let game = { TestUtilities.defaultGame with players = [player] }
        let effect = Effect.playerEliminated(player.id)
        let event = Event.create(EventKind.GameStarted, [effect]) //Kind doesn't matter

        //Act
        let newGame = EventService.applyEvent game event

        //Assert
        { game with players = newGame.players } |> shouldBe newGame

        newGame.players.Length |> shouldBe 1
        
        let newPlayer = newGame.players.Head
        newPlayer |> shouldBe { player with isAlive = Some false }

    [<Fact>]
    let ``Should apply PlayersRemoved effect``() =
        //Arrange
        let players : Player list =
            [
                {
                    id = 1
                    kind = PlayerKind.User
                    name = "p1"
                    gameId = 0
                    userId = None
                    isAlive = None
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
                    isAlive = None
                    colorId = None
                    startingRegion = None
                    startingTurnNumber = None
                }
                {
                    id = 3
                    kind = PlayerKind.User
                    name = "p3"
                    gameId = 0
                    userId = None
                    isAlive = None
                    colorId = None
                    startingRegion = None
                    startingTurnNumber = None
                }
            ]
        let game = { TestUtilities.defaultGame with players = players}
        let effect = Effect.playersRemoved([1;2])
        let event = Event.create(EventKind.GameStarted, [effect]) //Kind doesn't matter

        //Act
        let newGame = EventService.applyEvent game event

        //Assert
        { game with players = newGame.players } |> shouldBe newGame

        newGame.players.Length |> shouldBe 1
        newGame.players.Head |> shouldBe players.[2]

    [<Fact>]
    let ``Should apply TurnCycleChanged effect``() =
        //Arrange
        let game = TestUtilities.defaultGame
        let newCycle = [1;2;3]
        let effect = Effect.turnCycleChanged(game.turnCycle, newCycle)
        let event = Event.create(EventKind.GameStarted, [effect]) //Kind doesn't matter

        game.turnCycle |> shouldBe List.empty

        //Act
        let newGame = EventService.applyEvent game event

        //Assert
        { game with turnCycle = newGame.turnCycle } |> shouldBe newGame

        newGame.turnCycle |> shouldBe newCycle   