[<AutoOpen>]
module Djambi.Api.IntegrationTests.TestUtilities

open System
open FSharp.Control.Tasks
open Microsoft.Extensions.Configuration
open Djambi.Api.Model.UserModel
open Djambi.Api.Model.LobbyModel
open Djambi.Api.Model.SessionModel
open Djambi.Utilities
open Djambi.Api.Model.PlayerModel
open Djambi.Api.Common
open Djambi.Api.Logic.Services

let private config =
    ConfigurationBuilder()
        .AddJsonFile("appsettings.json", false)
        .AddJsonFile(Environment.environmentConfigPath(6), false)
        .Build()

let connectionString =
    config.GetConnectionString("Main")
            .Replace("{sqlAddress}", config.["sqlAddress"])

let getCreateUserRequest() : CreateUserRequest =
    {
        name = "Test_" + Guid.NewGuid().ToString()
        password = Guid.NewGuid().ToString()
    }

let getLoginRequest(userRequest : CreateUserRequest) : LoginRequest =
    {
        username = userRequest.name
        password = userRequest.password
    }

let getCreateLobbyRequest() : CreateLobbyRequest =
    {
        regionCount = 3
        description = Some "Test"
        isPublic = false
        allowGuests = false
    }

let getCreatePlayerRequest(lobbyId : int) : CreatePlayerRequest =
    {
        playerType = PlayerType.User
        userId = None
        lobbyId = lobbyId
        name = None
    }

let adminUserId = 1

let getSessionForUser (userId : int) : Session =
    {
        userId = userId
        isAdmin = false
        id = 0
        token = ""
        createdOn = DateTime.MinValue
        expiresOn = DateTime.MinValue
    }

let createUser() : UserDetails AsyncHttpResult =
    let userRequest = getCreateUserRequest()
    UserService.createUser userRequest None

let createUserSessionAndLobby(allowGuests : bool) : (UserDetails * Session * Lobby) AsyncHttpResult =
    task {
        let! user = createUser() |> AsyncHttpResult.thenValue

        let session = getSessionForUser user.id

        let lobbyRequest = { getCreateLobbyRequest() with allowGuests = allowGuests }
        let! lobby = LobbyService.createLobby lobbyRequest session
                     |> AsyncHttpResult.thenValue

        return Ok <| (user, session, lobby)
    }