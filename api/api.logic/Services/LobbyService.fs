module Djambi.Api.Logic.Services.LobbyService

open System
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.Model.LobbyModel

let createGame (request : CreateGameRequest, session : Session) : LobbyGameMetadata AsyncHttpResult =
    LobbyRepository.createGame request

let deleteGame (gameId : int, session : Session) : Unit AsyncHttpResult =
    LobbyRepository.deleteGame gameId

let addPlayerToGame (gameId : int, userId : int, session : Session) : Unit AsyncHttpResult =
    LobbyRepository.addPlayerToGame(gameId, userId)

let removePlayerFromGame (gameId : int, userId : int, session : Session) : Unit AsyncHttpResult =
    LobbyRepository.removePlayerFromGame(gameId, userId)