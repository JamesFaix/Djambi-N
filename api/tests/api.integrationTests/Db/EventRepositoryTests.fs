namespace Djambi.Api.IntegrationTests.Db

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.IntegrationTests
open Djambi.Api.Model

type EventRepositoryTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Should add players``() =
        task {
            //Arrange
            let! (user, _, game) = TestUtilities.createuserSessionAndGame(true) |> thenValue

            let player : Player = 
                {
                    id = 0
                    gameId = game.id
                    userId = Some user.id
                    kind = PlayerKind.Guest
                    name = "p2"
                    isAlive = None
                    colorId = None
                    startingRegion = None
                    startingTurnNumber = None
                }

            let newGame = { game with players = [player] }

            //Act
            let! persistedGame = EventRepository.persistEvent (game, newGame) |> thenValue

            //Assert
            { newGame with players = persistedGame.players } |> shouldBe persistedGame

            persistedGame.players.Length |> shouldBe 1

            let persistedPlayer = persistedGame.players.Head
            persistedPlayer.id |> shouldNotBe 0
            { persistedPlayer with id = player.id } |> shouldBe player
        }

    [<Fact>]
    let ``Should remove players``() =
        task {
            //Arrange
            let! (user, _, game) = TestUtilities.createuserSessionAndGame(true) |> thenValue

            let playerRequest = 
                {
                    userId = Some user.id
                    kind = PlayerKind.Guest
                    name = Some "p2"
                }

            let! _ = GameRepository.addPlayer(game.id, playerRequest) |> thenValue
            let! game = GameRepository.getGame game.id |> thenValue

            let newGame = { game with players = List.empty }

            //Act
            let! persistedGame = EventRepository.persistEvent (game, newGame) |> thenValue

            //Assert
            { newGame with players = persistedGame.players } |> shouldBe persistedGame

            persistedGame.players.Length |> shouldBe 0
        }

    [<Fact>]
    let ``Should update players``() =
        task {
            //Arrange
            let! (user, _, game) = TestUtilities.createuserSessionAndGame(true) |> thenValue

            let p2Request = 
                {
                    userId = Some user.id
                    kind = PlayerKind.Guest
                    name = Some "p2"
                }

            let! _ = GameRepository.addPlayer(game.id, p2Request) |> thenValue
            let! game = GameRepository.getGame game.id |> thenValue

            let oldP2 = game.players.[1]
            oldP2.isAlive |> shouldBe None
            oldP2.startingRegion |> shouldBe None
            oldP2.startingTurnNumber |> shouldBe None
            oldP2.colorId |> shouldBe None

            let newP2 = 
                { oldP2 with
                    isAlive = Some true
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
            let! persistedGame = EventRepository.persistEvent (game, newGame) |> thenValue

            //Assert
            { newGame with players = persistedGame.players } |> shouldBe persistedGame

            persistedGame.players.Length |> shouldBe 2
            persistedGame.players.[0] |> shouldBe game.players.[0]
            persistedGame.players.[1] |> shouldBe newP2
        }

    [<Fact>]
    let ``Should update game``() =
        task {
            //Arrange
            let! (_, _, game) = TestUtilities.createuserSessionAndGame(false) |> thenValue

            game.status |> shouldBe GameStatus.Pending
            game.parameters |> shouldBe 
                {
                    allowGuests = false
                    isPublic = false
                    regionCount = 3
                    description = Some "Test"
                }
            game.currentTurn |> shouldBe None
            game.turnCycle |> shouldBe List.empty
            game.pieces |> shouldBe List.empty
            
            let newGame = 
                { game with
                    status = GameStatus.Aborted
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
                            kind = PieceKind.Assassin
                            cellId = 0
                            playerId = None
                            originalPlayerId = 0
                        }
                    ]
                }

            //Act
            let! persistedGame = EventRepository.persistEvent (game, newGame) |> thenValue

            //Assert
            persistedGame |> shouldBe newGame
        }

    //TODO: Should rollback transactionally