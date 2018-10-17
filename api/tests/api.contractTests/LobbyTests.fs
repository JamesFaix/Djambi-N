module Djambi.Api.ContractTests.LobbyTests

open System
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
let ``Create lobby should fail if no session``() =
    task {
        //Arrange
        let request = RequestFactory.createLobbyRequest()

        //Act
        let! response = LobbyClient.createLobby(request, "")

        //Assert
        response |> shouldBeError HttpStatusCode.Unauthorized "Not signed in."
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
let ``Delete lobby should fail if invalid lobbyId``() =
    task {
        //Arrange
        let! (_, token) = SetupUtility.createUserAndSignIn()

        //Act
        let! response = LobbyClient.deleteLobby(Int32.MinValue, token)

        //Assert
        response |> shouldBeError HttpStatusCode.NotFound "Lobby not found."
    } :> Task

[<Test>]
let ``Delete lobby should fail if no session``() =
    task {
        //Arrange
        let! (_, token) = SetupUtility.createUserAndSignIn()
        let createLobbyRequest = RequestFactory.createLobbyRequest()
        let! lobbyResponse = LobbyClient.createLobby(createLobbyRequest, token)
        let lobby = lobbyResponse.bodyValue
        let! _ = SessionClient.closeSession token

        //Act
        let! response = LobbyClient.deleteLobby(lobby.id, token)

        //Assert
        response |> shouldBeError HttpStatusCode.Unauthorized "Not signed in."
    } :> Task

[<Test>]
let ``Delete lobby should fail if lobby created by another user and not admin``() =
    task {
        //Arrange
        let! (_, token1) = SetupUtility.createUserAndSignIn()
        let createLobbyRequest1 = RequestFactory.createLobbyRequest()
        let! lobbyResponse1 = LobbyClient.createLobby(createLobbyRequest1, token1)
        let lobby1 = lobbyResponse1.bodyValue

        let! (_, token2) = SetupUtility.createUserAndSignIn()
        let createLobbyRequest2 = RequestFactory.createLobbyRequest()
        let! lobbyResponse2 = LobbyClient.createLobby(createLobbyRequest2, token2)
        let lobby2 = lobbyResponse2.bodyValue

        //Act
        let! response = LobbyClient.deleteLobby(lobby1.id, token2)

        //Assert
        response |> shouldBeError HttpStatusCode.Forbidden "Users can only delete lobbies that they created."
    } :> Task

//TODO: Delete lobby should work if lobby created by another user and session is admin

[<Test>]
let ``Get lobbies should work``() =
    task {
        //Arrange
        let! (_, token1) = SetupUtility.createUserAndSignIn()
        let! (_, token2) = SetupUtility.createUserAndSignIn()
        let request1 = RequestFactory.createLobbyRequest()
        let request2 = RequestFactory.createLobbyRequest()
        let! lobby1 = LobbyClient.createLobby(request1, token1) |> AsyncResponse.bodyValue
        let! lobby2 = LobbyClient.createLobby(request2, token2) |> AsyncResponse.bodyValue

        //Act
        let! response = LobbyClient.getLobbies(LobbiesQueryJsonModel.empty, token1)

        //Assert
        response |> shouldHaveStatus HttpStatusCode.OK

        let lobbys = response.bodyValue
        lobbys.Length |> shouldBeAtLeast 2
        lobbys |> shouldExist (fun g -> g.id = lobby1.id)
        lobbys |> shouldExist (fun g -> g.id = lobby2.id)
    } :> Task

[<Test>]
let ``Get lobbies should fail if no session``() =
    task {
        //Arrange

        //Act
        let! response = LobbyClient.getLobbies(LobbiesQueryJsonModel.empty, "someToken")

        //Assert
        response |> shouldBeError HttpStatusCode.Unauthorized "Not signed in."
    } :> Task

//TODO: Get lobbies should filter on created by

//TODO: Get lobbies should filter on allow guests

//TODO: Get lobbies should filter on is public

//TODO: Get lobbies should filter on player userId

//TODO: Get lobbies should filter non-public lobbies current user is not in, if not admin

//TODO: Start game should work

//TODO: Start game should fail if not logged in

//TODO: Start game should fail if not user who created lobby and not admin

//TODO: Start game shoudl work if not user who created lobby but is admin