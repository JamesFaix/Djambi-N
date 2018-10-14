module Djambi.Api.ContractTests.LobbyTests

open System
open System.Net
open System.Threading.Tasks
open FSharp.Control.Tasks
open NUnit.Framework
open Djambi.Api.WebClient
open Djambi.Api.Common.Enums

//Create game should work
[<Test>]
let ``Create game should work``() =
    task {
        //Arrange
        let! (user, token) = SetupUtility.createUserAndSignIn()
        let request = RequestFactory.createGameRequest()

        //Act
        let! response = LobbyRepository.createGame(request, token)

        //Assert
        response |> shouldHaveStatus HttpStatusCode.OK

        let game = response.bodyValue
        game.id |> shouldNotBe 0
        //game.players.Length |> shouldBe 1
        //game.players |> shouldExist (fun p -> p.id = user.id)
        game.status |> shouldBe (GameStatus.Open.ToString())
        game.description |> shouldBe request.description
        game.boardRegionCount |> shouldBe request.boardRegionCount
    } :> Task

//Create game should fail if no session

//Delete game should work

//Delete game should fail if invalid gameId

//Delete game should fail if no session

//Get game should work

//Get game should fail if no session

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