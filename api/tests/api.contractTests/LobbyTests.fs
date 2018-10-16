module Djambi.Api.ContractTests.LobbyTests

open System
open System.Net
open System.Threading.Tasks
open FSharp.Control.Tasks
open NUnit.Framework
open Djambi.Api.WebClient

[<Test>]
let ``Create game should work``() =
    task {
        //Arrange
        let! (_, token) = SetupUtility.createUserAndSignIn()
        let request = RequestFactory.createLobbyRequest()

        //Act
        let! response = LobbyClient.createLobby(request, token)

        //Assert
        response |> shouldHaveStatus HttpStatusCode.OK

        let game = response.bodyValue
        game.id |> shouldNotBe 0
      //  game.status |> shouldBe (GameStatus.Open.ToString())
        game.description |> shouldBe request.description
        game.regionCount |> shouldBe request.regionCount
    } :> Task

[<Test>]
let ``Create game should fail if no session``() =
    task {
        //Arrange
        let request = RequestFactory.createLobbyRequest()

        //Act
        let! response = LobbyClient.createLobby(request, "")

        //Assert
        response |> shouldBeError HttpStatusCode.Unauthorized "Not signed in."
    } :> Task

[<Test>]
let ``Delete game should work``() =
    task {
        //Arrange
        let! (_, token) = SetupUtility.createUserAndSignIn()
        let createGameRequest = RequestFactory.createLobbyRequest()
        let! gameResponse = LobbyClient.createLobby(createGameRequest, token)
        let game = gameResponse.bodyValue

        //Act
        let! response = LobbyClient.deleteLobby(game.id, token)

        //Assert
        response |> shouldHaveStatus HttpStatusCode.OK

        let! games = LobbyClient.getLobbies(token) |> AsyncResponse.bodyValue
        games |> List.exists (fun g -> g.id = game.id) |> shouldBeFalse
    } :> Task
    
[<Test>]
let ``Delete game should fail if invalid gameId``() =
    task {
        //Arrange
        let! (_, token) = SetupUtility.createUserAndSignIn()

        //Act
        let! response = LobbyClient.deleteLobby(Int32.MinValue, token)

        //Assert
        response |> shouldBeError HttpStatusCode.NotFound "Game not found."
    } :> Task

[<Test>]
let ``Delete game should fail if no session``() =
    task {
        //Arrange
        let! (_, token) = SetupUtility.createUserAndSignIn()
        let createGameRequest = RequestFactory.createLobbyRequest()
        let! gameResponse = LobbyClient.createLobby(createGameRequest, token)
        let game = gameResponse.bodyValue
        let! _ = SessionClient.closeSession token

        //Act
        let! response = LobbyClient.deleteLobby(game.id, token)

        //Assert
        response |> shouldBeError HttpStatusCode.Unauthorized "Not signed in."
    } :> Task

[<Test>]
let ``Delete game should fail if game created by user not in session``() =
    task {
        //Arrange
        let! (_, token1) = SetupUtility.createUserAndSignIn()
        let createGameRequest1 = RequestFactory.createLobbyRequest()
        let! gameResponse1 = LobbyClient.createLobby(createGameRequest1, token1)
        let game1 = gameResponse1.bodyValue

        let! (_, token2) = SetupUtility.createUserAndSignIn()
        let createGameRequest2 = RequestFactory.createLobbyRequest()
        let! gameResponse2 = LobbyClient.createLobby(createGameRequest2, token2)
        let game2 = gameResponse2.bodyValue

        //Act
        let! response = LobbyClient.deleteLobby(game1.id, token2)

        //Assert
        response |> shouldBeError HttpStatusCode.Forbidden "Users can only delete games that they created."
    } :> Task

[<Test>]
let ``Get games should work``() =
    task {
        //Arrange
        let! (_, token1) = SetupUtility.createUserAndSignIn()
        let! (_, token2) = SetupUtility.createUserAndSignIn()
        let request1 = RequestFactory.createLobbyRequest()
        let request2 = RequestFactory.createLobbyRequest()
        let! game1 = LobbyClient.createLobby(request1, token1) |> AsyncResponse.bodyValue
        let! game2 = LobbyClient.createLobby(request2, token2) |> AsyncResponse.bodyValue

        //Act
        let! response = LobbyClient.getLobbies(token1)

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
        let! response = LobbyClient.getLobbies("someToken")

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
        let request = RequestFactory.createLobbyRequest()
        let! game = LobbyClient.createLobby(request, token) |> AsyncResponse.bodyValue
        let! _ = SessionClient.closeSession token

        //Act
        let! response = LobbyClient.addPlayer(game.id, user.id, token)

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
        let request = RequestFactory.createLobbyRequest()
        let! lobby = LobbyClient.createLobby(request, token) |> AsyncResponse.bodyValue
        let! _ = SessionClient.closeSession token

        //Act
        let! response = LobbyClient.removePlayer(lobby.id, user.id, token)

        //Assert
        response |> shouldBeError HttpStatusCode.Unauthorized "Not signed in."
    } :> Task