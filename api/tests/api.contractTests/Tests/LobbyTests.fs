module Djambi.Api.ContractTests.LobbyTests

open System.Net
open System.Threading.Tasks
open FSharp.Control.Tasks
open NUnit.Framework
open Djambi.Api.WebClient
open Djambi.Api.Model

[<Test>]
let ``Create game should work``() =
    task {
        //Arrange
        let! (_, token) = SetupUtility.createUserAndSignIn()
        let request = RequestFactory.createGameRequest()

        //Act
        let! response = GameClient.createGame(request, token)

        //Assert
        response |> shouldHaveStatus HttpStatusCode.OK

        let game = response.bodyValue
        game.id |> shouldNotBe 0
        game.parameters.description |> shouldBe request.description
        game.parameters.regionCount |> shouldBe request.regionCount
    } :> Task

[<Test>]
let ``Delete game should work``() =
    task {
        //Arrange
        let! (_, token) = SetupUtility.createUserAndSignIn()
        let createGameRequest = RequestFactory.createGameRequest()
        let! gameResponse = GameClient.createGame(createGameRequest, token)
        let game = gameResponse.bodyValue

        //Act
        let! response = GameClient.deleteGame(game.id, token)

        //Assert
        response |> shouldHaveStatus HttpStatusCode.OK

        let! games = GameClient.getGames(GamesQuery.empty, token) |> AsyncResponse.bodyValue
        games |> List.exists (fun g -> g.id = game.id) |> shouldBeFalse
    } :> Task

[<Test>]
let ``Get games should work``() =
    task {
        //Arrange
        let! (_, token) = SetupUtility.createUserAndSignIn()
        let request = RequestFactory.createGameRequest()
        let! game1 = GameClient.createGame(request, token) |> AsyncResponse.bodyValue
        let! game2 = GameClient.createGame(request, token) |> AsyncResponse.bodyValue

        //Act
        let! response = GameClient.getGames(GamesQuery.empty, token)

        //Assert
        response |> shouldHaveStatus HttpStatusCode.OK

        let games = response.bodyValue
        games.Length |> shouldBeAtLeast 2
        games |> shouldExist (fun g -> g.id = game1.id)
        games |> shouldExist (fun g -> g.id = game2.id)
    } :> Task

//TODO: Start game should work

//TODO: Start game should fail if not logged in

//TODO: Start game should fail if not user who created lobby and not admin

//TODO: Start game shoudl work if not user who created lobby but is admin