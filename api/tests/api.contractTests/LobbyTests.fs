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
        response |> shouldBeError HttpStatusCode.NotFound "Not signed in."
    } :> Task

//Get games should work

//Get games should fail if no session

//Get open games should only return open games

//Get open games should fail if no session

//Get user games should only return games a user is a player in

//Get user games should fail if invalid userId

//Get user games should fail if no session

//Add player should work

//Add player should fail if user is already in game

//Add player should fail if invalid gameId

//Add player should fail if invalid userId

//Add player should fail if game is at player capacity

//Add player should fail if game is not open

//Add player should fail if no session

//Remove player should work

//Remove player should fail if invalid gameId

//Remove player should fail if invalid userId

//Remove player should fail if user not in game

//Remove player should fail if game is not open

//Remove player should fail if no session