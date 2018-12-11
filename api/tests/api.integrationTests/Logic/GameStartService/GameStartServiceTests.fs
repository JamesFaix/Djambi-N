namespace Djambi.Api.IntegrationTests.Logic.GameStartService

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.IntegrationTests
open Djambi.Api.Logic.ModelExtensions
open Djambi.Api.Logic.Services
open Djambi.Api.Model

type GameStartServiceTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Get starting conditions should work``() =
        task {
            //Arrange
            let session = getSessionForUser 1
            let gameRequest = getCreateGameRequest()
            let! game = GameCrudService.createGame gameRequest session
                        |> thenBindAsync PlayerService.fillEmptyPlayerSlots
                        |> thenValue

            //Act
            let playersWithStartConditions = GameStartService.assignStartingConditions game.players

            //Assert
            Assert.Equal(game.parameters.regionCount, playersWithStartConditions.Length)

            let regions = playersWithStartConditions |> List.map (fun p -> p.startingRegion.Value) |> List.sort
            regions |> shouldBe [0..(game.parameters.regionCount-1)]

            let colors = playersWithStartConditions |> List.map (fun cond -> cond.colorId.Value)
            Assert.All(colors, fun c -> Assert.True(c >= 0 && c < Constants.maxRegions))
        }

    [<Fact>]
    let ``Create pieces should work``() =
        task {
            //Arrange
            let session = getSessionForUser 1
            let gameRequest = getCreateGameRequest()
            let! game = GameCrudService.createGame gameRequest session 
                        |> thenBindAsync PlayerService.fillEmptyPlayerSlots
                        |> thenValue
            let playersWithStartConditions = GameStartService.assignStartingConditions game.players
            let board = BoardModelUtility.getBoardMetadata(game.parameters.regionCount)

            //Act
            let pieces = GameStartService.createPieces(board, playersWithStartConditions)

            //Assert
            Assert.Equal(game.parameters.regionCount * Constants.piecesPerPlayer, pieces.Length)

            let groupByPlayer = pieces |> List.groupBy (fun p -> p.originalPlayerId)
            Assert.Equal(game.parameters.regionCount, groupByPlayer.Length)

            for (_, grp) in groupByPlayer do
                Assert.Single<Piece>(grp, (fun p -> p.kind = Chief)) |> ignore
                Assert.Single<Piece>(grp, (fun p -> p.kind = Diplomat)) |> ignore
                Assert.Single<Piece>(grp, (fun p -> p.kind = Reporter)) |> ignore
                Assert.Single<Piece>(grp, (fun p -> p.kind = Gravedigger)) |> ignore
                Assert.Single<Piece>(grp, (fun p -> p.kind = Assassin)) |> ignore
                Assert.Equal(4, grp |> List.filter (fun p -> p.kind = Thug) |> List.length)
        }

    [<Fact>]
    let ``Start game should work``() =
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true) |> thenValue

            let playerRequest = CreatePlayerRequest.guest (user.id, "test")

            let! _ = PlayerService.addPlayer (game.id, playerRequest) session |> thenValue

            //Act
            let! result = GameStartService.startGame game.id session

            //Assert
            result |> Result.isOk |> shouldBeTrue

            let updatedGame = result |> Result.value

            updatedGame.status |> shouldBe GameStatus.Started
            updatedGame.players.Length |> shouldBe game.parameters.regionCount
            updatedGame.pieces.Length |> shouldBe (9 * game.parameters.regionCount)
        }

    [<Fact>]
    let ``Start game should fail if only one non-neutral player``() =
        task {
             //Arrange
            let! (user, session, game) = createuserSessionAndGame(true) |> thenValue

            //Act
            let! result = GameStartService.startGame game.id session

            //Assert
            result |> shouldBeError 400 "Cannot start game with only one player."

            let! lobbyResult = GameRepository.getGame game.id
            lobbyResult |> Result.isOk |> shouldBeTrue
        }

    [<Fact>]
    let ``Neutral players should not be in the turn cycle``() =
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true) |> thenValue

            let playerRequest = CreatePlayerRequest.guest (user.id, "test")

            let! _ = PlayerService.addPlayer (game.id, playerRequest) session |> thenValue

            //Act
            let! updatedGame = GameStartService.startGame game.id session |> thenValue

            //Assert
            let! players = PlayerService.getGamePlayers updatedGame.id session |> thenValue

            let neutralPlayerIds =
                players
                |> List.filter (fun p -> p.kind = PlayerKind.Neutral)
                |> List.map (fun p -> p.id)
                |> Set.ofList

            updatedGame.turnCycle
            |> Set.ofList
            |> Set.intersect neutralPlayerIds
            |> Set.count
            |> shouldBe 0
        }