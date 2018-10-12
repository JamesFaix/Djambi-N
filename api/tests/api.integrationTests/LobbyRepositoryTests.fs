namespace Djambi.Api.IntegrationTests

open System
open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Common.Enums
open Djambi.Api.Db
open Djambi.Api.Db.Repositories
open Djambi.Api.Model.LobbyModel
open Djambi.Tests.TestUtilities

type LobbyRepositoryTests() =
    do 
        SqlUtility.connectionString <- connectionString

    //Game CRUD

    [<Fact>] 
    let ``Create game should work``() =
        let request = getCreateGameRequest()
        task {
            let! game = LobbyRepository.createGame(request) |> thenValue
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
            let! createdGame = LobbyRepository.createGame(request) |> thenValue
            let! game = LobbyRepository.getGame(createdGame.id) |> thenValue
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
            let! game = LobbyRepository.createGame(request) |> thenValue
            let! _ = LobbyRepository.deleteGame(game.id) |> thenValue
            let getGame = fun () -> LobbyRepository.getGame(game.id).Result |> ignore
            Assert.Throws<AggregateException>(getGame) |> ignore
        }

    [<Fact>]
    let ``Get open games should work``() =
        let request = getCreateGameRequest()
        task {
            let! createdGame = LobbyRepository.createGame(request) |> thenValue
            let! games = LobbyRepository.getOpenGames() |> thenValue
            Assert.Contains(createdGame, games)
            Assert.All(games, fun g -> Assert.Equal(GameStatus.Open, g.status))
        }
    
    //Player CRUD
    [<Fact>]
    let ``Add player should work``() =
        let gameRequest = getCreateGameRequest()
        let userRequest = getCreateUserRequest()
        task {
            let! game = LobbyRepository.createGame(gameRequest) |> thenValue
            let! user = UserRepository.createUser(userRequest) |> thenValue
            let! _ = LobbyRepository.addPlayerToGame(game.id, user.id) |> thenValue
            let! updatedGame = LobbyRepository.getGame(game.id) |> thenValue
            let exists = updatedGame.players |> List.exists (fun p -> p.userId = Some user.id && p.name = user.name)
            Assert.True(exists)
        }

    [<Fact>]
    let ``Repository - Add virtual player should work``() =
        let gameRequest = getCreateGameRequest()
        let userRequest = getCreateUserRequest()
        task {
            let! game = LobbyRepository.createGame(gameRequest) |> thenValue
            let! _ = LobbyRepository.addVirtualPlayerToGame(game.id, userRequest.name) |> thenValue
            let! updatedGame = LobbyRepository.getGame(game.id) |> thenValue
            let exists = updatedGame.players |> List.exists (fun p -> p.userId = None && p.name = userRequest.name)
            Assert.True(exists)
        }

    [<Fact>]
    let ``Remove player should work``() =
        let gameRequest = getCreateGameRequest()
        let userRequest = getCreateUserRequest()
        task {
            let! game = LobbyRepository.createGame(gameRequest) |> thenValue
            let! user = UserRepository.createUser(userRequest) |> thenValue
            let! _ = LobbyRepository.addPlayerToGame(game.id, user.id) |> thenValue
            let! _ = LobbyRepository.removePlayerFromGame(game.id, user.id) |> thenValue
            let! updatedGame = LobbyRepository.getGame(game.id) |> thenValue
            let exists = updatedGame.players |> List.exists (fun p -> p.userId = Some user.id && p.name = user.name)
            Assert.False(exists)
        }