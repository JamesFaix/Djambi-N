module Djambi.Api.ContractTests.LobbyTests

open System.Net
open System.Threading.Tasks
open FSharp.Control.Tasks
open NUnit.Framework
open Djambi.Api.WebClient
open Djambi.Api.Web.Model.LobbyWebModel

[<Test>]
let ``Create lobby should work``() =
    task {
        //Arrange
        let! (_, token) = SetupUtility.createUserAndSignIn()
        let request = RequestFactory.createLobbyRequest()

        //Act
        let! response = LobbyClient.createLobby(request, token)

        //Assert
        response |> shouldHaveStatus HttpStatusCode.OK

        let lobby = response.bodyValue
        lobby.id |> shouldNotBe 0
        lobby.description |> shouldBe request.description
        lobby.regionCount |> shouldBe request.regionCount
    } :> Task

[<Test>]
let ``Delete lobby should work``() =
    task {
        //Arrange
        let! (_, token) = SetupUtility.createUserAndSignIn()
        let createLobbyRequest = RequestFactory.createLobbyRequest()
        let! lobbyResponse = LobbyClient.createLobby(createLobbyRequest, token)
        let lobby = lobbyResponse.bodyValue

        //Act
        let! response = LobbyClient.deleteLobby(lobby.id, token)

        //Assert
        response |> shouldHaveStatus HttpStatusCode.OK

        let! lobbies = LobbyClient.getLobbies(LobbiesQueryJsonModel.empty, token) |> AsyncResponse.bodyValue
        lobbies |> List.exists (fun g -> g.id = lobby.id) |> shouldBeFalse
    } :> Task

[<Test>]
let ``Get lobbies should work``() =
    task {
        //Arrange
        let! (_, token) = SetupUtility.createUserAndSignIn()
        let request = RequestFactory.createLobbyRequest()
        let! lobby1 = LobbyClient.createLobby(request, token) |> AsyncResponse.bodyValue
        let! lobby2 = LobbyClient.createLobby(request, token) |> AsyncResponse.bodyValue

        //Act
        let! response = LobbyClient.getLobbies(LobbiesQueryJsonModel.empty, token)

        //Assert
        response |> shouldHaveStatus HttpStatusCode.OK

        let lobbys = response.bodyValue
        lobbys.Length |> shouldBeAtLeast 2
        lobbys |> shouldExist (fun g -> g.id = lobby1.id)
        lobbys |> shouldExist (fun g -> g.id = lobby2.id)
    } :> Task

//TODO: Start game should work

//TODO: Start game should fail if not logged in

//TODO: Start game should fail if not user who created lobby and not admin

//TODO: Start game shoudl work if not user who created lobby but is admin