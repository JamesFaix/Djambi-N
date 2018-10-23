namespace Djambi.Api.IntegrationTests.Db

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db
open Djambi.Api.Db.Repositories
open Djambi.Api.Model.PlayerModel
open Djambi.Tests.TestUtilities

type PlayerRepositoryTests() =
    do 
        SqlUtility.connectionString <- connectionString
   
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
            let! _ = PlayerRepository.addPlayerToLobby request |> thenValue

            //Assert
            let! players = PlayerRepository.getPlayers lobby.id |> thenValue
            let exists = players 
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
            let! _ = PlayerRepository.addPlayerToLobby request |> thenValue

            //Assert
            let! players = PlayerRepository.getPlayers lobby.id |> thenValue
            let exists = players |> List.exists (fun p -> 
                p.userId = None
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
            let! _ = PlayerRepository.addPlayerToLobby request |> thenValue

            //Assert
            let! players = PlayerRepository.getPlayers lobby.id |> thenValue
            let exists = players |> List.exists (fun p -> 
                p.userId = Some user.id
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
            let! playerId = PlayerRepository.addPlayerToLobby playerRequest |> thenValue

            //Act
            let! _ = PlayerRepository.removePlayerFromLobby playerId |> thenValue

            //Assert
            let! players = PlayerRepository.getPlayers lobby.id |> thenValue
            let exists = players |> List.exists (fun p -> p.id = playerId)
            Assert.False(exists)
        }