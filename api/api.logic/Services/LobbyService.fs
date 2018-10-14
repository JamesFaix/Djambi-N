module Djambi.Api.Logic.Services.LobbyService

open System
open System.Linq
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.Model.LobbyModel

let createGame (request : CreateGameRequest, session : Session) : LobbyGameMetadata AsyncHttpResult =
    LobbyRepository.createGame request

let deleteGame (gameId : int, session : Session) : Unit AsyncHttpResult =
    LobbyRepository.getGame gameId
    |> thenBind (fun game -> 
        if Enumerable.Contains(session.userIds, game.createdByUserId)
        then Ok game
        else Error <| HttpException(403, "Users can only delete games that they created.")
    )
    |> thenBindAsync (fun _ -> LobbyRepository.deleteGame gameId)

let addPlayerToGame (gameId : int, userId : int, session : Session) : Unit AsyncHttpResult =
    LobbyRepository.addPlayerToGame(gameId, userId)

let removePlayerFromGame (gameId : int, userId : int, session : Session) : Unit AsyncHttpResult =
    LobbyRepository.removePlayerFromGame(gameId, userId)

let getGames (session : Session) : LobbyGameMetadata list AsyncHttpResult =
    LobbyRepository.getGames()
    
let getOpenGames (session : Session) : LobbyGameMetadata list AsyncHttpResult =
    LobbyRepository.getOpenGames()
    
let getUserGames (userId : int, session : Session) : LobbyGameMetadata list AsyncHttpResult =
    UserRepository.getUser userId
    |> thenBindAsync (fun _ -> LobbyRepository.getUserGames userId)