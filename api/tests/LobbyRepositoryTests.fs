namespace Djambi.Tests

open System
open Giraffe
open Xunit

open Djambi.Api.Persistence
open Djambi.Api.Domain.LobbyModels
open Djambi.Api.Common.Enums

type LobbyRepositoryTests() =
    do 
        SqlUtility.connectionString <- TestUtilities.connectionString

    let getCreateUserRequest() : CreateUserRequest = 
        {
            name = "Test_" + Guid.NewGuid().ToString()
            isGuest = false
        }

    let getCreateGameRequest() : CreateGameRequest =
        {
            boardRegionCount = 3
            description = Some "Test"
        }

    //Game CRUD

    [<Fact>] 
    let ``Create game should work``() =
        let request = getCreateGameRequest()
        task {
            let! game = LobbyRepository.createGame(request)
            Assert.NotEqual(0, game.id)
            Assert.Equal(request.boardRegionCount, game.boardRegionCount)
            Assert.Equal(request.description, game.description)
            Assert.Equal(GameStatus.Open, game.status)
            Assert.Equal(0, game.players.Length)
        }

    [<Fact>]
    let ``Get game should work`` () =
        let request = getCreateGameRequest()
        task {
            let! createdGame = LobbyRepository.createGame(request)
            let! game = LobbyRepository.getGame(createdGame.id)
            Assert.Equal(createdGame.id, game.id)
            Assert.Equal(createdGame.description, game.description)
            Assert.Equal(createdGame.boardRegionCount, game.boardRegionCount)
            Assert.Equal(createdGame.status, game.status)
            Assert.Equal<LobbyPlayer list>(createdGame.players, game.players)
        }

    [<Fact>]
    let ``Delete game should work``() =
        let request = getCreateGameRequest()
        task {
            let! game = LobbyRepository.createGame(request)
            let! _ = LobbyRepository.deleteGame(game.id)
            let getGame = fun () -> LobbyRepository.getGame(game.id).Result |> ignore
            Assert.Throws<AggregateException>(getGame) |> ignore
        }

    [<Fact>]
    let ``Get open games should work``() =
        let request = getCreateGameRequest()
        task {
            let! createdGame = LobbyRepository.createGame(request)
            let! games = LobbyRepository.getOpenGames()
            Assert.Contains(createdGame, games)
            Assert.All(games, fun g -> Assert.Equal(GameStatus.Open, g.status))
        }
    
    //Player CRUD
    [<Fact>]
    let ``Add player should work``() =
        let gameRequest = getCreateGameRequest()
        let userRequest = getCreateUserRequest()
        task {
            let! game = LobbyRepository.createGame(gameRequest)
            let! user = UserRepository.createUser(userRequest)
            let! _ = LobbyRepository.addPlayerToGame(game.id, user.id)
            let! updatedGame = LobbyRepository.getGame(game.id)
            let exists = updatedGame.players |> List.exists (fun p -> p.userId = Some user.id && p.name = user.name)
            Assert.True(exists)
        }

    [<Fact>]
    let ``Remove player should work``() =
        let gameRequest = getCreateGameRequest()
        let userRequest = getCreateUserRequest()
        task {
            let! game = LobbyRepository.createGame(gameRequest)
            let! user = UserRepository.createUser(userRequest)
            let! _ = LobbyRepository.addPlayerToGame(game.id, user.id)
            let! _ = LobbyRepository.removePlayerFromGame(game.id, user.id)
            let! updatedGame = LobbyRepository.getGame(game.id)
            let exists = updatedGame.players |> List.exists (fun p -> p.userId = Some user.id && p.name = user.name)
            Assert.False(exists)
        }