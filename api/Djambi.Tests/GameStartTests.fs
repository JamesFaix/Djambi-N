module PlayRepositoryTests

open System
open Giraffe
open Xunit

open Djambi.Api.Persistence
open Djambi.Api.Domain.LobbyModels
open Djambi.Api.Common.Enums
open Djambi.Api.Domain.PlayModels
open TestUtilities
open Djambi.Api.Domain
open Djambi.Api.Common

let private getLobbyRepo() =
    let cnStr = getConnectionString()
    new LobbyRepository(cnStr)

let private getRepository() = 
    let cnStr = getConnectionString()
    let lobbyRepo = new LobbyRepository(cnStr)
    new GameStartRepository(cnStr, lobbyRepo)

let private getService() =
    let repo = getRepository()
    let playService = new PlayService(new PlayRepository(getConnectionString()))
    new GameStartService(repo, playService)
    
let private getCreateGameRequest() : CreateGameRequest =
    {
        boardRegionCount = 3
        description = Some "Test"
    }

let private getCreateUserRequest() : CreateUserRequest = 
    {
        name = "Test_" + Guid.NewGuid().ToString()
        isGuest = false
    }

[<Fact>]
let ``Repository - Add virtual player should work``() =
    let gameRequest = getCreateGameRequest()
    let userRequest = getCreateUserRequest()
    let lobbyRepo = getLobbyRepo()
    let repo = getRepository()
    task {
        let! game = lobbyRepo.createGame(gameRequest)
        let! _ = repo.addVirtualPlayerToGame(game.id, userRequest.name)
        let! updatedGame = repo.getGame(game.id)
        let exists = updatedGame.players |> List.exists (fun p -> p.userId = None && p.name = userRequest.name)
        Assert.True(exists)
    }

[<Fact>]
let ``Service - Add virtual players should work``() =
    let gameRequest = getCreateGameRequest()
    let lobbyRepo = getLobbyRepo()
    let service = getService()
    task {
        let! game = lobbyRepo.createGame gameRequest
        let! players = service.addVirtualPlayers game
        let! updatedGame = lobbyRepo.getGame game.id
        Assert.Equal(gameRequest.boardRegionCount, updatedGame.players.Length)
        Assert.Equal<LobbyPlayer list>(players, updatedGame.players)
    }

[<Fact>]
let ``Service = Get starting conditions should work``() =
    let gameRequest = getCreateGameRequest()
    let lobbyRepo = getLobbyRepo()
    let service = getService()
    task {
        let! game = lobbyRepo.createGame gameRequest
        let! players = service.addVirtualPlayers game
        let startingConditions = service.getStartingConditions players

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
    let lobbyRepo = getLobbyRepo()
    let service = getService()
    task {
        let! game = lobbyRepo.createGame gameRequest
        let! players = service.addVirtualPlayers game
        let startingConditions = service.getStartingConditions players
        let board = BoardUtility.getBoardMetadata(game.boardRegionCount)
        let pieces = service.createPieces(board, startingConditions)

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
    let lobbyRepo = getLobbyRepo()
    let service = getService()
    task {
        let! game = lobbyRepo.createGame gameRequest
        let! gameState = service.startGame game.id

        let! updated = lobbyRepo.getGame game.id

        Assert.Equal(GameStatus.Started, updated.status)
    }