﻿[<AutoOpen>]
module Djambi.Api.IntegrationTests.TestUtilities

open System
open FSharp.Control.Tasks
open Microsoft.Extensions.Configuration
open Djambi.Api.Model
open Djambi.Utilities
open Djambi.Api.Common
open Djambi.Api.Logic.Services
open Djambi.Api.Logic.Managers

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

let getGameParameters() : GameParameters =
    {
        regionCount = 3
        description = Some "Test"
        isPublic = false
        allowGuests = false
    }

let getCreateGameRequest (userId : int) : CreateGameRequest =
    {
        parameters = getGameParameters()
        createdByUserId = userId
    }

let getCreatePlayerRequest : CreatePlayerRequest =
    {
        kind = PlayerKind.User
        userId = None
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

let createuserSessionAndGame(allowGuests : bool) : (UserDetails * Session * Game) AsyncHttpResult =
    task {
        let! user = createUser() |> AsyncHttpResult.thenValue

        let session = getSessionForUser user.id

        let parameters = { getGameParameters() with allowGuests = allowGuests }
        let! resp = GameManager.createGame parameters session
                     |> AsyncHttpResult.thenValue

        return Ok <| (user, session, resp.game)
    }