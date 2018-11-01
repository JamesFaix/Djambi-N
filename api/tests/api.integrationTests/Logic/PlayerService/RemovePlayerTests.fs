namespace Djambi.Api.IntegrationTests.Logic.PlayerService

open System
open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common
open Djambi.Api.IntegrationTests
open Djambi.Api.Logic.Services
open Djambi.Api.Model.PlayerModel

type RemovePlayerTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Remove player should work if removing self``() =
        task {
            //Arrange
            let! (_, _, lobby) = createUserSessionAndLobby(false) |> AsyncHttpResult.thenValue

            let! user = createUser() |> AsyncHttpResult.thenValue
            let session = getSessionForUser user.id

            let request : CreatePlayerRequest =
                {
                    lobbyId = lobby.id
                    userId = Some user.id
                    name = None
                    playerType = PlayerType.User
                }

            let! player = PlayerService.addPlayerToLobby request session
                          |> AsyncHttpResult.thenValue

            //Act
            let! result = PlayerService.removePlayerFromLobby (lobby.id, player.id) session

            //Assert
            result |> Result.isOk |> shouldBeTrue

            let! players = PlayerService.getLobbyPlayers lobby.id session
                           |> AsyncHttpResult.thenValue

            players |> shouldNotExist (fun p -> p.id = player.id)
        }

    [<Fact>]
    let ``Remove player should work if removing guest of self``() =
        task {
            //Arrange
            let! (user, session, lobby) = createUserSessionAndLobby(true) |> AsyncHttpResult.thenValue

            let request : CreatePlayerRequest =
                {
                    lobbyId = lobby.id
                    userId = Some user.id
                    name = Some "test"
                    playerType = PlayerType.Guest
                }

            let! player = PlayerService.addPlayerToLobby request session
                          |> AsyncHttpResult.thenValue

            //Act
            let! result = PlayerService.removePlayerFromLobby (lobby.id, player.id) session

            //Assert
            result |> Result.isOk |> shouldBeTrue

            let! players = PlayerService.getLobbyPlayers lobby.id session
                           |> AsyncHttpResult.thenValue

            players |> shouldNotExist (fun p -> p.id = player.id)
        }

    [<Fact>]
    let ``Remove player should work if admin and removing different user``() =
        task {
            //Arrange
            let! (_, session, lobby) = createUserSessionAndLobby(false) |> AsyncHttpResult.thenValue

            let! user = createUser() |> AsyncHttpResult.thenValue

            let request : CreatePlayerRequest =
                {
                    lobbyId = lobby.id
                    userId = Some user.id
                    name = None
                    playerType = PlayerType.User
                }

            let! player = PlayerService.addPlayerToLobby request { session with isAdmin = true }
                          |> AsyncHttpResult.thenValue

            //Act
            let! result = PlayerService.removePlayerFromLobby (lobby.id, player.id) { session with isAdmin = true }

            //Assert
            result |> Result.isOk |> shouldBeTrue

            let! players = PlayerService.getLobbyPlayers lobby.id session
                           |> AsyncHttpResult.thenValue

            players |> shouldNotExist (fun p -> p.id = player.id)
        }

    [<Fact>]
    let ``Remove player should remove guests of user``() =
        task {
            //Arrange
            let! (_, session, lobby) = createUserSessionAndLobby(true) |> AsyncHttpResult.thenValue

            let! user = createUser() |> AsyncHttpResult.thenValue

            let userPlayerRequest : CreatePlayerRequest =
                {
                    lobbyId = lobby.id
                    userId = Some user.id
                    name = None
                    playerType = PlayerType.User
                }

            let guestPlayerRequest : CreatePlayerRequest =
                {
                    lobbyId = lobby.id
                    userId = Some user.id
                    name = Some "test"
                    playerType = PlayerType.Guest
                }

            let! userPlayer =
                PlayerService.addPlayerToLobby userPlayerRequest { session with isAdmin = true }
                |> AsyncHttpResult.thenValue

            let! guestPlayer =
                PlayerService.addPlayerToLobby guestPlayerRequest { session with isAdmin = true }
                |> AsyncHttpResult.thenValue

            //Act
            let! result = PlayerService.removePlayerFromLobby (lobby.id, userPlayer.id) { session with isAdmin = true }

            //Assert
            result |> Result.isOk |> shouldBeTrue

            let! players = PlayerService.getLobbyPlayers lobby.id session
                           |> AsyncHttpResult.thenValue

            players |> shouldNotExist (fun p -> p.id = userPlayer.id)
            players |> shouldNotExist (fun p -> p.id = guestPlayer.id)
        }

    [<Fact>]
    let ``Remove guest should not remove user``() =
        task {
            //Arrange
            let! (_, session, lobby) = createUserSessionAndLobby(true) |> AsyncHttpResult.thenValue

            let! user = createUser() |> AsyncHttpResult.thenValue

            let userPlayerRequest : CreatePlayerRequest =
                {
                    lobbyId = lobby.id
                    userId = Some user.id
                    name = None
                    playerType = PlayerType.User
                }

            let guestPlayerRequest : CreatePlayerRequest =
                {
                    lobbyId = lobby.id
                    userId = Some user.id
                    name = Some "test"
                    playerType = PlayerType.Guest
                }

            let! userPlayer =
                PlayerService.addPlayerToLobby userPlayerRequest { session with isAdmin = true }
                |> AsyncHttpResult.thenValue

            let! guestPlayer =
                PlayerService.addPlayerToLobby guestPlayerRequest { session with isAdmin = true }
                |> AsyncHttpResult.thenValue

            //Act
            let! result = PlayerService.removePlayerFromLobby (lobby.id, guestPlayer.id) { session with isAdmin = true }

            //Assert
            result |> Result.isOk |> shouldBeTrue

            let! players = PlayerService.getLobbyPlayers lobby.id session
                           |> AsyncHttpResult.thenValue

            players |> shouldExist (fun p -> p.id = userPlayer.id)
            players |> shouldNotExist (fun p -> p.id = guestPlayer.id)
        }

    [<Fact>]
    let ``Remove player should close lobby if creating user``() =
        task {
            //Arrange
            let! (_, session, lobby) = createUserSessionAndLobby(false) |> AsyncHttpResult.thenValue
            let! players = PlayerService.getLobbyPlayers lobby.id session |> AsyncHttpResult.thenValue
            let creator = players |> List.head

            //Act
            let! result = PlayerService.removePlayerFromLobby (lobby.id, creator.id) session

            //Assert
            result |> Result.isOk |> shouldBeTrue

            let! error = PlayerService.getLobbyPlayers lobby.id session

            error |> shouldBeError 404 "Lobby not found."
        }

    [<Fact>]
    let ``Remove player should fail if removing different user and not admin``() =
        task {
            //Arrange
            let! (_, session, lobby) = createUserSessionAndLobby(false) |> AsyncHttpResult.thenValue

            let! user = createUser() |> AsyncHttpResult.thenValue

            let request : CreatePlayerRequest =
                {
                    lobbyId = lobby.id
                    userId = Some user.id
                    name = None
                    playerType = PlayerType.User
                }

            let! player = PlayerService.addPlayerToLobby request { session with isAdmin = true }
                          |> AsyncHttpResult.thenValue

            //Act
            let! error = PlayerService.removePlayerFromLobby (lobby.id, player.id) { session with isAdmin = false }

            //Assert
            error |> shouldBeError 403 "Cannot remove other users from lobby."

            let! players = PlayerService.getLobbyPlayers lobby.id session
                           |> AsyncHttpResult.thenValue

            players |> shouldExist (fun p -> p.id = player.id)
        }

    [<Fact>]
    let ``Remove player should fail if removing guest of different user and not admin``() =
        task {
            //Arrange
            let! (_, session, lobby) = createUserSessionAndLobby(true) |> AsyncHttpResult.thenValue

            let! user = createUser() |> AsyncHttpResult.thenValue

            let request : CreatePlayerRequest =
                {
                    lobbyId = lobby.id
                    userId = Some user.id
                    name = Some "test"
                    playerType = PlayerType.Guest
                }

            let! player = PlayerService.addPlayerToLobby request { session with isAdmin = true }
                          |> AsyncHttpResult.thenValue

            //Act
            let! error = PlayerService.removePlayerFromLobby (lobby.id, player.id) { session with isAdmin = false }

            //Assert
            error |> shouldBeError 403 "Cannot remove other users from lobby."

            let! players = PlayerService.getLobbyPlayers lobby.id session
                           |> AsyncHttpResult.thenValue

            players |> shouldExist (fun p -> p.id = player.id)
        }

    //TODO: Remove player should fail if removing virtual player
    [<Fact>]
    let ``Remove player should work if removing self``() =
        task {
            //Arrange
            let! (_, session, lobby) = createUserSessionAndLobby(false) |> AsyncHttpResult.thenValue

            let! players = PlayerService.getLobbyPlayers lobby.id session |> AsyncHttpResult.thenValue
            let! _ = PlayerService.fillEmptyPlayerSlots lobby players |> AsyncHttpResult.thenValue
            let! updatedPlayers = PlayerService.getLobbyPlayers lobby.id session |> AsyncHttpResult.thenValue
            let virtualPlayer = updatedPlayers
                                |> List.filter(fun p -> p.playerType = PlayerType.Virtual)
                                |> List.head

            //Act
            let! error = PlayerService.removePlayerFromLobby (lobby.id, virtualPlayer.id) session

            //Assert
            error |> shouldBeError 400 "Cannot remove virtual players from lobby."
        }

    [<Fact>]
    let ``Remove player should fail if removing player not in lobby``() =
        task {
            //Arrange
            let! (_, session, lobby1) = createUserSessionAndLobby(false) |> AsyncHttpResult.thenValue

            let lobbyRequest = getCreateLobbyRequest()
            let! lobby2 = LobbyService.createLobby lobbyRequest session |> AsyncHttpResult.thenValue

            let! user = createUser() |> AsyncHttpResult.thenValue

            let request : CreatePlayerRequest =
                {
                    lobbyId = lobby1.id
                    userId = Some user.id
                    name = None
                    playerType = PlayerType.User
                }

            let! player = PlayerService.addPlayerToLobby request { session with isAdmin = true }
                          |> AsyncHttpResult.thenValue

            //Act
            let! error = PlayerService.removePlayerFromLobby (lobby2.id, player.id) { session with isAdmin = true }

            //Assert
            error |> shouldBeError 404 "Player not found."

            let! players = PlayerService.getLobbyPlayers lobby1.id session
                           |> AsyncHttpResult.thenValue

            players |> shouldExist (fun p -> p.id = player.id)
        }

    [<Fact>]
    let ``Remove player should fail if invalid lobbyId``() =
        task {
            //Arrange
            let! (_, session, lobby) = createUserSessionAndLobby(false) |> AsyncHttpResult.thenValue

            let! user = createUser() |> AsyncHttpResult.thenValue

            let request : CreatePlayerRequest =
                {
                    lobbyId = lobby.id
                    userId = Some user.id
                    name = None
                    playerType = PlayerType.User
                }

            let! player = PlayerService.addPlayerToLobby request { session with isAdmin = true }
                          |> AsyncHttpResult.thenValue

            //Act
            let! error = PlayerService.removePlayerFromLobby (Int32.MinValue, player.id) { session with isAdmin = true }

            //Assert
            error |> shouldBeError 404 "Lobby not found."

            let! players = PlayerService.getLobbyPlayers lobby.id session
                           |> AsyncHttpResult.thenValue

            players |> shouldExist (fun p -> p.id = player.id)
        }

    [<Fact>]
    let ``Remove player should fail if invalid playerId``() =
        task {
            //Arrange
            let! (_, session, lobby) = createUserSessionAndLobby(false) |> AsyncHttpResult.thenValue

            let! user = createUser() |> AsyncHttpResult.thenValue

            let request : CreatePlayerRequest =
                {
                    lobbyId = lobby.id
                    userId = Some user.id
                    name = None
                    playerType = PlayerType.User
                }

            let! player = PlayerService.addPlayerToLobby request { session with isAdmin = true }
                          |> AsyncHttpResult.thenValue

            //Act
            let! error = PlayerService.removePlayerFromLobby (lobby.id, Int32.MinValue) { session with isAdmin = true }

            //Assert
            error |> shouldBeError 404 "Player not found."

            let! players = PlayerService.getLobbyPlayers lobby.id session
                           |> AsyncHttpResult.thenValue

            players |> shouldExist (fun p -> p.id = player.id)
        }

    [<Fact>]
    let ``Remove player should fail if game already started``() =
        task {
            //Arrange
            let! (_, session, lobby) = createUserSessionAndLobby(false) |> AsyncHttpResult.thenValue

            let! user = createUser() |> AsyncHttpResult.thenValue

            let request : CreatePlayerRequest =
                {
                    lobbyId = lobby.id
                    userId = Some user.id
                    name = None
                    playerType = PlayerType.User
                }

            let! player = PlayerService.addPlayerToLobby request { session with isAdmin = true }
                          |> AsyncHttpResult.thenValue

            let! _ = GameStartService.startGame lobby.id session |> AsyncHttpResult.thenValue

            //Act
            let! error = PlayerService.removePlayerFromLobby (lobby.id, player.id) { session with isAdmin = true }

            //Assert
            error |> shouldBeError 404 "Lobby not found."

            //TODO: Assert about game still containing player
        }