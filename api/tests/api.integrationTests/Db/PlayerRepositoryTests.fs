namespace Djambi.Api.IntegrationTests.Db

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.IntegrationTests
open Djambi.Api.Model

type PlayerRepositoryTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Add user player should work``() =
        //Arrange
        let userId = 1
        let lobbyRequest = getCreateLobbyRequest()
        let userRequest = getCreateUserRequest()
        task {
            let! lobby = LobbyRepository.createLobby (lobbyRequest, userId) |> thenValue
            let! user = UserRepository.createUser userRequest |> thenValue
            let request = CreatePlayerRequest.user user.id

            //Act
            let! _ = PlayerRepository.addPlayerToLobby (lobby.id, request) |> thenValue

            //Assert
            let! players = PlayerRepository.getPlayersForLobby lobby.id |> thenValue
            let exists = players
                         |> List.exists (fun p -> p.userId = Some user.id
                                                  && p.name = user.name
                                                  && p.kind = PlayerKind.User)
            Assert.True(exists)
        }

    [<Fact>]
    let ``Add virtual player should work``() =
        //Arrange
        let userId = 1
        let lobbyRequest = getCreateLobbyRequest()
        task {
            let! lobby = LobbyRepository.createLobby (lobbyRequest, userId) |> thenValue
            let request = CreatePlayerRequest.neutral "test"

            //Act
            let! _ = PlayerRepository.addPlayerToLobby (lobby.id, request) |> thenValue

            //Assert
            let! players = PlayerRepository.getPlayersForLobby lobby.id |> thenValue
            let exists = players |> List.exists (fun p ->
                p.userId = None
                && p.name = request.name.Value
                && p.kind = PlayerKind.Neutral)
            Assert.True(exists)
        }

    [<Fact>]
    let ``Add guest player should work``() =
        //Arrange
        let userId = 1
        let lobbyRequest = getCreateLobbyRequest()
        let userRequest = getCreateUserRequest()
        task {
            let! lobby = LobbyRepository.createLobby (lobbyRequest, userId) |> thenValue
            let! user = UserRepository.createUser userRequest |> thenValue
            let request = CreatePlayerRequest.guest (user.id, "test")

            //Act
            let! _ = PlayerRepository.addPlayerToLobby (lobby.id, request) |> thenValue

            //Assert
            let! players = PlayerRepository.getPlayersForLobby lobby.id |> thenValue
            let exists = players |> List.exists (fun p ->
                p.userId = Some user.id
                && p.name = request.name.Value
                && p.kind = PlayerKind.Guest)
            Assert.True(exists)
        }

    [<Fact>]
    let ``Remove player should work``() =
        //Arrange
        let userId = 1
        let lobbyRequest = getCreateLobbyRequest()
        let userRequest = getCreateUserRequest()
        task {
            let! lobby = LobbyRepository.createLobby (lobbyRequest, userId) |> thenValue
            let! user = UserRepository.createUser userRequest |> thenValue
            let playerRequest = CreatePlayerRequest.user user.id
            let! player = PlayerRepository.addPlayerToLobby (lobby.id, playerRequest) |> thenValue

            //Act
            let! _ = PlayerRepository.removePlayerFromLobby player.id |> thenValue

            //Assert
            let! players = PlayerRepository.getPlayersForLobby lobby.id |> thenValue
            let exists = players |> List.exists (fun p -> p.id = player.id)
            Assert.False(exists)
        }