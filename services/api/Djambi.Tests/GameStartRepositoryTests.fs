module PlayRepositoryTests

open System
open Giraffe
open Xunit

open Djambi.Api.Persistence
open Djambi.Api.Domain.LobbyModels
open Djambi.Api.Common.Enums

open TestUtilities

let private getLobbyRepo() =
    let cnStr = getConnectionString()
    new LobbyRepository(cnStr)

let private getRepository() = 
    let cnStr = getConnectionString()
    let lobbyRepo = new LobbyRepository(cnStr)
    new GameStartRepository(cnStr, lobbyRepo)
    
let private getCreateGameRequest() : CreateGameRequest =
    {
        boardRegionCount = 3
        description = Some "Test"
    }

let private getCreateUserRequest() : CreateUserRequest = 
    {
        name = "Test_" + Guid.NewGuid().ToString()
    }

[<Fact>]
let ``Add virtual player should work``() =
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