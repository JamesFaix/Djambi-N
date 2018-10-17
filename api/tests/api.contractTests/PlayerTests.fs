module Djambi.Api.ContractTests.PlayerTests

open System
open System.Net
open System.Threading.Tasks
open FSharp.Control.Tasks
open NUnit.Framework
open Djambi.Api.WebClient
open Djambi.Api.Web.Model.PlayerWebModel

//TODO: Add user player should work

//TODO: Add user player should fail if adding a different user and not admin

//TODO: Add user player should work if adding different user, but admin

//TODO: Add user player should fail if not passing userid

//TODO: Add user player should fail if passing name

//TODO: Add user player should fail if already in lobby

//TODO: Add guest player should work

//TODO: Add guest player should fail if adding guest to different user and not admin

//TOOD: Add guest player should work if adding guest to different user, but admin

//TODO: Add guest player should fail if not passing userid

//TODO: Add guest player should fail if not passing name

//TODO: Add guest player should fail if duplicate name

//TODO: Add guest player should fail if lobby does not allow guests

//TODO: Add virtual player should fail

//TODO: Add player should fail if invalid lobbyId

//TODO: Add player should fail if lobby is at player capacity

//TODO: Add player should fail if game already started

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

//TODO: Remove player should work

//TODO: Removing user player should remove all guests

//TODO: Remove player should fail if invalid lobbyId

//TODO: Remove player should fail if invalid playerId

//TODO: Remove player should fail if player not in lobby

//TODO: Remove player should fail if different user and not admin

//TODO: Remove player should work if different user, but admin

//TODO: Remove player should fail if game already started

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