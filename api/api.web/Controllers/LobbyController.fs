﻿module Djambi.Api.Web.Controllers.LobbyController

open Giraffe
open Microsoft.AspNetCore.Http
open Djambi.Api.Common
open Djambi.Api.Db.Repositories
open Djambi.Api.Web.HttpUtility
open Djambi.Api.Web.Mappings.LobbyWebMapping
open Djambi.Api.Web.Model.LobbyWebModel
    
let getOpenGames : HttpHandler =
    let func ctx =
        LobbyRepository.getOpenGames()
        |> Task.thenMap (List.map mapLobbyGameResponse)
    handle func
            
let getGames : HttpHandler =
    let func ctx =
        LobbyRepository.getGames()
        |> Task.thenMap (List.map mapLobbyGameResponse)
    handle func

let getUserGames (userId : int) =
    let func ctx =
        LobbyRepository.getUserGames userId
        |> Task.thenMap (List.map mapLobbyGameResponse)
    handle func

let createGame : HttpHandler =
    let func (ctx : HttpContext) =
        ctx.BindModelAsync<CreateGameJsonModel>()
        |> Task.map mapCreateGameRequest
        |> Task.bind LobbyRepository.createGame
        |> Task.thenMap mapLobbyGameResponse
    handle func        
    
let deleteGame(gameId : int) =
    let func ctx = 
        LobbyRepository.deleteGame gameId
    handle func
 
let addPlayerToGame(gameId : int, userId : int) =
    let func ctx =
        LobbyRepository.addPlayerToGame(gameId, userId)
    handle func

let removePlayerFromGame(gameId : int, userId : int) =
    let func ctx =
        LobbyRepository.removePlayerFromGame(gameId, userId)
    handle func