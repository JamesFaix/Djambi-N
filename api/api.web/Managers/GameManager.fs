﻿module Djambi.Api.Web.Managers.GameManager

open Djambi.Api.Logic.Services
open Djambi.Api.Common
open Djambi.Api.Model

let getGames (query : GamesQuery) (session : Session) : Game list AsyncHttpResult =
    GameCrudService.getGames query session

let getGame (gameId : int) (session : Session) : Game AsyncHttpResult =
    GameCrudService.getGame gameId session

let createGame (parameters : GameParameters) (session : Session) : Game AsyncHttpResult =
    GameCrudService.createGame parameters session

let deleteGame (gameId : int) (session : Session) : Unit AsyncHttpResult =
    GameCrudService.deleteGame gameId session
 
let updateGameParameters (gameId : int) (parameters : GameParameters) (session : Session) : Game AsyncHttpResult =
    GameCrudService.updateGameParameters (gameId, parameters) session

let addPlayer (request : CreatePlayerRequest, gameId : int) (session : Session) : Player AsyncHttpResult =
    PlayerService.addPlayer (gameId, request) session

let removePlayer (gameId : int, playerId : int) (session : Session) : Unit AsyncHttpResult =
    PlayerService.removePlayer (gameId, playerId) session

let startGame (gameId: int) (session : Session) : Game AsyncHttpResult =
    GameStartService.startGame gameId session

let selectCell (gameId : int, cellId : int) (session : Session) : Turn AsyncHttpResult =
    TurnService.selectCell (gameId, cellId) session

let resetTurn (gameId : int) (session : Session) : Turn AsyncHttpResult =
    TurnService.resetTurn gameId session

let commitTurn (gameId : int) (session : Session) : Game AsyncHttpResult =
    TurnService.commitTurn gameId session