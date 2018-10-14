module Djambi.Api.ContractTests.LobbyTests

open System
open System.Net
open System.Threading.Tasks
open FSharp.Control.Tasks
open NUnit.Framework
open Djambi.Api.WebClient
open Djambi.Api.Common.Enums

[<Test>]
let ``Create game should work``() =
    task {
        //Arrange
        let! (_, token) = SetupUtility.createUserAndSignIn()
        let request = RequestFactory.createGameRequest()

        //Act
        let! response = LobbyRepository.createGame(request, token)

        //Assert
        response |> shouldHaveStatus HttpStatusCode.OK

        let game = response.bodyValue
        game.id |> shouldNotBe 0
        game.players.Length |> shouldBe 0
        game.status |> shouldBe (GameStatus.Open.ToString())
        game.description |> shouldBe request.description
        game.boardRegionCount |> shouldBe request.boardRegionCount
    } :> Task

[<Test>]
let ``Create game should fail if no session``() =
    task {
        //Arrange
        let request = RequestFactory.createGameRequest()

        //Act
        let! response = LobbyRepository.createGame(request, "")

        //Assert
        response |> shouldBeError HttpStatusCode.Unauthorized "Not signed in."
    } :> Task

[<Test>]
let ``Delete game should work``() =
    task {
        //Arrange
        let! (_, token) = SetupUtility.createUserAndSignIn()
        let createGameRequest = RequestFactory.createGameRequest()
        let! gameResponse = LobbyRepository.createGame(createGameRequest, token)
        let game = gameResponse.bodyValue

        //Act
        let! response = LobbyRepository.deleteGame(game.id, token)

        //Assert
        response |> shouldHaveStatus HttpStatusCode.OK

        let! games = LobbyRepository.getGames(token) |> AsyncResponse.bodyValue
        games |> List.exists (fun g -> g.id = game.id) |> shouldBeFalse
    } :> Task
    
[<Test>]
let ``Delete game should fail if invalid gameId``() =
    task {
        //Arrange
        let! (_, token) = SetupUtility.createUserAndSignIn()

        //Act
        let! response = LobbyRepository.deleteGame(Int32.MinValue, token)

        //Assert
        response |> shouldBeError HttpStatusCode.NotFound "Game not found."
    } :> Task

[<Test>]
let ``Delete game should fail if no session``() =
    task {
        //Arrange
        let! (_, token) = SetupUtility.createUserAndSignIn()
        let createGameRequest = RequestFactory.createGameRequest()
        let! gameResponse = LobbyRepository.createGame(createGameRequest, token)
        let game = gameResponse.bodyValue
        let! _ = SessionRepository.closeSession token

        //Act
        let! response = LobbyRepository.deleteGame(game.id, token)

        //Assert
        response |> shouldBeError HttpStatusCode.Unauthorized "Not signed in."
    } :> Task

[<Test>]
let ``Delete game should fail if game created by user not in session``() =
    task {
        //Arrange
        let! (_, token1) = SetupUtility.createUserAndSignIn()
        let createGameRequest1 = RequestFactory.createGameRequest()
        let! gameResponse1 = LobbyRepository.createGame(createGameRequest1, token1)
        let game1 = gameResponse1.bodyValue

        let! (_, token2) = SetupUtility.createUserAndSignIn()
        let createGameRequest2 = RequestFactory.createGameRequest()
        let! gameResponse2 = LobbyRepository.createGame(createGameRequest2, token2)
        let game2 = gameResponse2.bodyValue

        //Act
        let! response = LobbyRepository.deleteGame(game1.id, token2)

        //Assert
        response |> shouldBeError HttpStatusCode.Forbidden "Users can only delete games that they created."
    } :> Task

[<Test>]
let ``Get games should work``() =
    task {
        //Arrange
        let! (_, token1) = SetupUtility.createUserAndSignIn()
        let! (_, token2) = SetupUtility.createUserAndSignIn()
        let request1 = RequestFactory.createGameRequest()
        let request2 = RequestFactory.createGameRequest()
        let! game1 = LobbyRepository.createGame(request1, token1) |> AsyncResponse.bodyValue
        let! game2 = LobbyRepository.createGame(request2, token2) |> AsyncResponse.bodyValue

        //Act
        let! response = LobbyRepository.getGames(token1)

        //Assert
        response |> shouldHaveStatus HttpStatusCode.OK

        let games = response.bodyValue
        games.Length |> shouldBeAtLeast 2
        games |> shouldExist (fun g -> g.id = game1.id)
        games |> shouldExist (fun g -> g.id = game2.id)

        //TODO: Add additional assertions about different game statuses
    } :> Task

//Get games should fail if not admin

[<Test>]
let ``Get games should fail if no session``() =
    task {
        //Arrange

        //Act
        let! response = LobbyRepository.getGames("someToken")

        //Assert
        response |> shouldBeError HttpStatusCode.Unauthorized "Not signed in."
    } :> Task

//Get open games should only return open games

[<Test>]
let ``Get open games should fail if no session``() =
    task {
        //Arrange

        //Act
        let! response = LobbyRepository.getOpenGames("someToken")

        //Assert
        response |> shouldBeError HttpStatusCode.Unauthorized "Not signed in."
    } :> Task

[<Test>]
let ``Get user games should only return games a user is a player in``() =
    task {
        //Arrange
        let! (user1, token1) = SetupUtility.createUserAndSignIn()
        let! (_, token2) = SetupUtility.createUserAndSignIn()
        let request1 = RequestFactory.createGameRequest()
        let request2 = RequestFactory.createGameRequest()
        let! game1 = LobbyRepository.createGame(request1, token1) |> AsyncResponse.bodyValue
        let! _ = LobbyRepository.createGame(request2, token2) |> AsyncResponse.bodyValue

        //Act
        let! response = LobbyRepository.getUserGames(user1.id, token1)

        //Assert
        response |> shouldHaveStatus HttpStatusCode.OK
        let games = response.bodyValue
        games.Length |> shouldBe 1
        games |> shouldExist (fun g -> g.id = game1.id)
    } :> Task

[<Test>]
let ``Get user games should fail if invalid userId``() =
    task {
        //Arrange
        let! (_, token) = SetupUtility.createUserAndSignIn()
        let request = RequestFactory.createGameRequest()
        let! _ = LobbyRepository.createGame(request, token) |> AsyncResponse.bodyValue

        //Act
        let! response = LobbyRepository.getUserGames(Int32.MinValue, token)

        //Assert
        response |> shouldBeError HttpStatusCode.NotFound "User not found."
    } :> Task

[<Test>]
let ``Get user games should fail if no session``() =
    task {
        //Arrange
        let createUserRequest = RequestFactory.createUserRequest()
        let! user = UserRepository.createUser createUserRequest |> AsyncResponse.bodyValue

        //Act
        let! response = LobbyRepository.getUserGames(user.id, "someToken")

        //Assert
        response |> shouldBeError HttpStatusCode.Unauthorized "Not signed in."
    } :> Task

//Add player should work

//Add player should fail if user is already in game

//Add player should fail if invalid gameId

//Add player should fail if invalid userId

//Add player should fail if game is at player capacity

//Add player should fail if game is not open

[<Test>]
let ``Add player to game should fail if no session``() =
    task {
        //Arrange
        let! (user, token) = SetupUtility.createUserAndSignIn()
        let request = RequestFactory.createGameRequest()
        let! game = LobbyRepository.createGame(request, token) |> AsyncResponse.bodyValue
        let! _ = SessionRepository.closeSession token

        //Act
        let! response = LobbyRepository.addPlayerToGame(game.id, user.id, token)

        //Assert
        response |> shouldBeError HttpStatusCode.Unauthorized "Not signed in."
    } :> Task

//Remove player should work

//Remove player should fail if invalid gameId

//Remove player should fail if invalid userId

//Remove player should fail if user not in game

//Remove player should fail if game is not open

[<Test>]
let ``Remove player from game should fail if no session``() =
    task {
        //Arrange
        let! (user, token) = SetupUtility.createUserAndSignIn()
        let request = RequestFactory.createGameRequest()
        let! game = LobbyRepository.createGame(request, token) |> AsyncResponse.bodyValue
        let! _ = SessionRepository.closeSession token

        //Act
        let! response = LobbyRepository.removePlayerFromGame(game.id, user.id, token)

        //Assert
        response |> shouldBeError HttpStatusCode.Unauthorized "Not signed in."
    } :> Task