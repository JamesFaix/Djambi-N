namespace Djambi.Tests

open Giraffe
open Xunit
open Djambi.Api.Common
open Djambi.Api.Common.Enums
open Djambi.Api.Db
open Djambi.Api.Db.Repositories
open Djambi.Api.Logic.ModelExtensions
open Djambi.Api.Logic.Services
open Djambi.Api.Model.LobbyModel
open Djambi.Api.Model.PlayModel
open Djambi.Tests.TestUtilities

type GameStartTests() =
    do 
        SqlUtility.connectionString <- connectionString

    [<Fact>]
    let ``Repository - Add virtual player should work``() =
        let gameRequest = getCreateGameRequest()
        let userRequest = getCreateUserRequest()
        task {
            let! game = LobbyRepository.createGame(gameRequest)
            let! _ = GameStartRepository.addVirtualPlayerToGame(game.id, userRequest.name)
            let! updatedGame = LobbyRepository.getGame(game.id)
            let exists = updatedGame.players |> List.exists (fun p -> p.userId = None && p.name = userRequest.name)
            Assert.True(exists)
        }

    [<Fact>]
    let ``Service - Add virtual players should work``() =
        let gameRequest = getCreateGameRequest()
        task {
            let! game = LobbyRepository.createGame gameRequest
            let! players = GameStartService.addVirtualPlayers game
            let! updatedGame = LobbyRepository.getGame game.id
            Assert.Equal(gameRequest.boardRegionCount, updatedGame.players.Length)
            Assert.Equal<LobbyPlayer list>(players, updatedGame.players)
        }

    [<Fact>]
    let ``Service = Get starting conditions should work``() =
        let gameRequest = getCreateGameRequest()
        task {
            let! game = LobbyRepository.createGame gameRequest
            let! players = GameStartService.addVirtualPlayers game
            let startingConditions = GameStartService.getStartingConditions players

            Assert.Equal(game.boardRegionCount, startingConditions.Length)

            let turnNumbers = startingConditions |> List.map (fun cond -> cond.turnNumber) |> List.sort
            Assert.Equal<int list>([0..(game.boardRegionCount-1)], turnNumbers)

            let regions = startingConditions |> List.map (fun cond -> cond.region) |> List.sort
            Assert.Equal<int list>([0..(game.boardRegionCount-1)], regions)

            let colors = startingConditions |> List.map (fun cond -> cond.color)
            Assert.All(colors, fun c -> Assert.True(c >= 0 && c < Constants.maxRegions))
        }

    [<Fact>]
    let ``Service - Create pieces should work``() =    
        let gameRequest = getCreateGameRequest()
        task {
            let! game = LobbyRepository.createGame gameRequest
            let! players = GameStartService.addVirtualPlayers game
            let startingConditions = GameStartService.getStartingConditions players
            let board = BoardModelUtility.getBoardMetadata(game.boardRegionCount)
            let pieces = GameStartService.createPieces(board, startingConditions)

            Assert.Equal(players.Length * Constants.piecesPerPlayer, pieces.Length)

            let groupByPlayer = pieces |> List.groupBy (fun p -> p.originalPlayerId)
            Assert.Equal(players.Length, groupByPlayer.Length)

            for (_, grp) in groupByPlayer do
                Assert.Single<Piece>(grp, (fun p -> p.pieceType = Chief)) |> ignore
                Assert.Single<Piece>(grp, (fun p -> p.pieceType = Diplomat)) |> ignore
                Assert.Single<Piece>(grp, (fun p -> p.pieceType = Reporter)) |> ignore
                Assert.Single<Piece>(grp, (fun p -> p.pieceType = Gravedigger)) |> ignore
                Assert.Single<Piece>(grp, (fun p -> p.pieceType = Assassin)) |> ignore
                Assert.Equal(4, grp |> List.filter (fun p -> p.pieceType = Thug) |> List.length)
        }

    [<Fact>]
    let ``Service - Start game should work``() =
        let gameRequest = getCreateGameRequest()
        task {
            let! game = LobbyRepository.createGame gameRequest
            let! gameState = GameStartService.startGame game.id

            let! updated = LobbyRepository.getGame game.id

            Assert.Equal(GameStatus.Started, updated.status)
        }