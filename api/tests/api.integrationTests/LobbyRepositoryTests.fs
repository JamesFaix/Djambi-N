namespace Djambi.Api.IntegrationTests

open System
open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db
open Djambi.Api.Db.Repositories
open Djambi.Api.Model.Enums
open Djambi.Api.Model.LobbyModel
open Djambi.Tests.TestUtilities

type LobbyRepositoryTests() =
    do 
        SqlUtility.connectionString <- connectionString

    //Game CRUD

    [<Fact>] 
    let ``Create lobby should work``() =
        //Arrange
        let request = getCreateLobbyRequest()
        task {
            //Act
            let! lobby = LobbyRepository.createLobby request |> thenValue

            //Assert
            Assert.NotEqual(0, lobby.id)
            Assert.Equal(request.regionCount, lobby.regionCount)
            Assert.Equal(request.description, lobby.description)
          //  Assert.Equal(GameStatus.Open, game.status)
        }

    [<Fact>]
    let ``Get lobby should work`` () =
        //Arrange
        let request = getCreateLobbyRequest()
        task {
            let! createdLobby = LobbyRepository.createLobby request |> thenValue

            //Act
            let! lobby = LobbyRepository.getLobby createdLobby.id |> thenValue

            //Assert
            Assert.Equal(createdLobby.id, lobby.id)
            Assert.Equal(createdLobby.description, lobby.description)
            Assert.Equal(createdLobby.regionCount, lobby.regionCount)
            //Assert.Equal(createdLobby.status, lobby.status)
        }

    [<Fact>]
    let ``Delete lobby should work``() =
        //Arrange
        let request = getCreateLobbyRequest()
        task {
            let! game = LobbyRepository.createLobby request |> thenValue

            //Act
            let! _ = LobbyRepository.deleteLobby game.id |> thenValue

            //Assert
            let getGame = fun () -> LobbyRepository.getLobby(game.id).Result |> ignore
            Assert.Throws<AggregateException>(getGame) |> ignore
        }

    [<Fact>]
    let ``Get games should work``() =
        //Arrange
        let request = getCreateLobbyRequest()
        task {
            let! createdGame = LobbyRepository.createLobby request |> thenValue
            let query = LobbiesQuery.empty

            //Act
            let! games = LobbyRepository.getLobbies query |> thenValue

            //Assert
            Assert.Contains(createdGame, games)
          //  Assert.All(games, fun g -> Assert.Equal(GameStatus.Open, g.status))
        }
    
    //Player CRUD
    [<Fact>]
    let ``Add user player should work``() =
        //Arrange
        let lobbyRequest = getCreateLobbyRequest()
        let userRequest = getCreateUserRequest()
        task {
            let! lobby = LobbyRepository.createLobby lobbyRequest |> thenValue
            let! user = UserRepository.createUser userRequest |> thenValue
            let request = CreatePlayerRequest.user(lobby.id, user.id)
           
            //Act
            let! _ = LobbyRepository.addPlayerToLobby request |> thenValue

            //Assert
            let! updatedLobby = LobbyRepository.getLobbyWithPlayers lobby.id |> thenValue
            let exists = updatedLobby.players 
                         |> List.exists (fun p -> p.userId = Some user.id
                                                  && p.name = user.name 
                                                  && p.playerType = PlayerType.User)
            Assert.True(exists)
        }

    [<Fact>]
    let ``Add virtual player should work``() =
        //Arrange
        let lobbyRequest = getCreateLobbyRequest()
        task {
            let! lobby = LobbyRepository.createLobby lobbyRequest |> thenValue
            let request = CreatePlayerRequest.``virtual``(lobby.id, "test")
           
            //Act
            let! _ = LobbyRepository.addPlayerToLobby request |> thenValue

            //Assert
            let! updatedLobby = LobbyRepository.getLobbyWithPlayers lobby.id |> thenValue
            let exists = updatedLobby.players 
                         |> List.exists (fun p -> p.userId = None
                                                  && p.name = request.name.Value
                                                  && p.playerType = PlayerType.Virtual)
            Assert.True(exists)
        }
    
    [<Fact>]
    let ``Add guest player should work``() =
        //Arrange
        let lobbyRequest = getCreateLobbyRequest()
        let userRequest = getCreateUserRequest()
        task {
            let! lobby = LobbyRepository.createLobby lobbyRequest |> thenValue
            let! user = UserRepository.createUser userRequest |> thenValue
            let request = CreatePlayerRequest.guest(lobby.id, user.id, "test")
           
            //Act
            let! _ = LobbyRepository.addPlayerToLobby request |> thenValue

            //Assert
            let! updatedLobby = LobbyRepository.getLobbyWithPlayers lobby.id |> thenValue
            let exists = updatedLobby.players 
                         |> List.exists (fun p -> p.userId = Some user.id
                                                  && p.name = request.name.Value
                                                  && p.playerType = PlayerType.Guest)
            Assert.True(exists)
        }

    [<Fact>]
    let ``Remove player should work``() =
        //Arrange
        let lobbyRequest = getCreateLobbyRequest()
        let userRequest = getCreateUserRequest()
        task {
            let! lobby = LobbyRepository.createLobby lobbyRequest |> thenValue
            let! user = UserRepository.createUser userRequest |> thenValue
            let playerRequest = CreatePlayerRequest.user(lobby.id, user.id)
            let! playerId = LobbyRepository.addPlayerToLobby playerRequest |> thenValue

            //Act
            let! _ = LobbyRepository.removePlayerFromLobby playerId |> thenValue

            //Assert
            let! updatedLobby = LobbyRepository.getLobbyWithPlayers lobby.id |> thenValue
            let exists = updatedLobby.players |> List.exists (fun p -> p.id = playerId)
            Assert.False(exists)
        }