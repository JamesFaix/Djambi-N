﻿module Djambi.Api.Web.Managers.PlayerManager

open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Logic.Services
open Djambi.Api.Web.Mappings
open Djambi.Api.Web.Model
open Djambi.Api.Model
open Djambi.Api.Common

let addPlayerToLobby(jsonModel : CreatePlayerJsonModel, lobbyId : int) (session : Session) : PlayerResponseJsonModel AsyncHttpResult =
    let request = mapCreatePlayerRequest (jsonModel, lobbyId)
    PlayerService.addPlayerToLobby request session
    |> thenMap mapPlayerResponse

let removePlayerFromLobby (lobbyId : int, playerId : int) (session : Session) : Unit AsyncHttpResult =
    PlayerService.removePlayerFromLobby (lobbyId, playerId) session