module Djambi.Api.ContractTests.PlayerTests

open System
open System.Net
open System.Threading.Tasks
open FSharp.Control.Tasks
open NUnit.Framework
open Djambi.Api.WebClient
open Djambi.Api.Web.Model.PlayerWebModel

//Add player should work

//Add player should fail if user is already in lobby

//Add player should fail if invalid lobbyId

//Add player should fail if invalid userId

//Add player should fail if lobby is at player capacity

//Add player should fail if lobby is not open

[<Test>]
let ``Add player to lobby should fail if no session``() =
    task {
        //Arrange
        let! (user, token) = SetupUtility.createUserAndSignIn()
        let request = RequestFactory.createLobbyRequest()
        let! lobby = LobbyClient.createLobby(request, token) |> AsyncResponse.bodyValue
        let! _ = SessionClient.closeSession token
        let request : CreatePlayerJsonModel = 
            {
                userId = Nullable<int>(user.id)
                ``type`` = "User"
                name = Unchecked.defaultof<string>
            }

        //Act
        let! response = PlayerClient.addPlayer(lobby.id, request, token)

        //Assert
        response |> shouldBeError HttpStatusCode.Unauthorized "Not signed in."
    } :> Task

//Remove player should work

//Remove player should fail if invalid lobbyId

//Remove player should fail if invalid userId

//Remove player should fail if user not in lobby

//Remove player should fail if lobby is not open

[<Test>]
let ``Remove player from lobby should fail if no session``() =
    task {
        //Arrange
        let! (user, token) = SetupUtility.createUserAndSignIn()
        let request = RequestFactory.createLobbyRequest()
        let! lobby = LobbyClient.createLobby(request, token) |> AsyncResponse.bodyValue
        let! _ = SessionClient.closeSession token

        //Act
        let! response = PlayerClient.removePlayer(lobby.id, user.id, token)

        //Assert
        response |> shouldBeError HttpStatusCode.Unauthorized "Not signed in."
    } :> Task