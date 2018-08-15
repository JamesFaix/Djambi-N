module LobbyRepositoryTests
    
open System
open Giraffe
open Xunit

open Djambi.Api.Persistence
open Djambi.Api.Domain.LobbyModels
open Djambi.Api.Common.Enums

open TestUtilities

let private getRepository() = 
    let cnStr = getConnectionString()
    new LobbyRepository(cnStr)

let private getCreateUserRequest() : CreateUserRequest = 
    {
        name = "Test_" + Guid.NewGuid().ToString()
    }

let private getCreateGameRequest() : CreateGameRequest =
    {
        boardRegionCount = 3
        description = Some "Test"
    }

//User CRUD

[<Fact>]
let ``Create user should work``() =
    let request = getCreateUserRequest()
    let repo = getRepository()
    task {
        let! user = repo.createUser(request)
        Assert.NotEqual(0, user.id)
        Assert.Equal(request.name, user.name)
    }

[<Fact>]
let ``Get user should work``() =
    let request = getCreateUserRequest()
    let repo = getRepository()
    task {
        let! createdUser = repo.createUser(request)
        let! user = repo.getUser(createdUser.id)        
        Assert.Equal(createdUser.id, user.id)
        Assert.Equal(createdUser.name, user.name)
    }

[<Fact>]
let ``Delete user should work``() =
    let request = getCreateUserRequest()
    let repo = getRepository()
    task {
        let! user = repo.createUser(request)
        let! _ = repo.deleteUser(user.id) 
        let getUser = fun () -> repo.getUser(user.id).Result |> ignore
        Assert.Throws<AggregateException>(getUser) |> ignore
    }

//Game CRUD

[<Fact>] 
let ``Create game should work``() =
    let request = getCreateGameRequest()
    let repo = getRepository()
    task {
        let! game = repo.createGame(request)
        Assert.NotEqual(0, game.id)
        Assert.Equal(request.boardRegionCount, game.boardRegionCount)
        Assert.Equal(request.description, game.description)
        Assert.Equal(GameStatus.Open, game.status)
        Assert.Equal(0, game.players.Length)
    }

[<Fact>]
let ``Get game should work`` () =
    let request = getCreateGameRequest()
    let repo = getRepository()
    task {
        let! createdGame = repo.createGame(request)
        let! game = repo.getGame(createdGame.id)
        Assert.Equal(createdGame.id, game.id)
        Assert.Equal(createdGame.description, game.description)
        Assert.Equal(createdGame.boardRegionCount, game.boardRegionCount)
        Assert.Equal(createdGame.status, game.status)
        Assert.Equal<User list>(createdGame.players, game.players)
    }

[<Fact>]
let ``Delete game should work``() =
    let request = getCreateGameRequest()
    let repo = getRepository()
    task {
        let! game = repo.createGame(request)
        let! _ = repo.deleteGame(game.id)
        let getGame = fun () -> repo.getGame(game.id).Result |> ignore
        Assert.Throws<AggregateException>(getGame) |> ignore
    }

[<Fact>]
let ``Get open games should work``() =
    let request = getCreateGameRequest()
    let repo = getRepository()
    task {
        let! createdGame = repo.createGame(request)
        let! games = repo.getOpenGames()
        Assert.Contains(createdGame, games)
        Assert.All(games, fun g -> Assert.Equal(GameStatus.Open, g.status))
    }

//Player CRUD
[<Fact>]
let ``Add player should work``() =
    let gameRequest = getCreateGameRequest()
    let userRequest = getCreateUserRequest()
    let repo = getRepository()
    task {
        let! game = repo.createGame(gameRequest)
        let! user = repo.createUser(userRequest)
        let! _ = repo.addPlayerToGame(game.id, user.id)
        let! updatedGame = repo.getGame(game.id)
        Assert.Contains(user, updatedGame.players)
    }

[<Fact>]
let ``Remove player should work``() =
    let gameRequest = getCreateGameRequest()
    let userRequest = getCreateUserRequest()
    let repo = getRepository()
    task {
        let! game = repo.createGame(gameRequest)
        let! user = repo.createUser(userRequest)
        let! _ = repo.addPlayerToGame(game.id, user.id)
        let! _ = repo.removePlayerFromGame(game.id, user.id)
        let! updatedGame = repo.getGame(game.id)
        Assert.DoesNotContain(user, updatedGame.players)
    }