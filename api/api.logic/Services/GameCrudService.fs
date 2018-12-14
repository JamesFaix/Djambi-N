module Djambi.Api.Logic.Services.GameCrudService

open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.Model
open System

[<Obsolete>]
let createGame (parameters : GameParameters) (session : Session) : Game AsyncHttpResult =
    let gameRequest =   
        {
            parameters = parameters
            createdByUserId = session.userId
        }

    //Create game
    GameRepository.createGame gameRequest
    |> thenBindAsync (fun gameId ->
        //Add self as first player
        let playerRequest = CreatePlayerRequest.user session.userId
        GameRepository.addPlayer (gameId, playerRequest)
        //Return game
        |> thenBindAsync (fun _ -> GameRepository.getGame gameId)
    )

let deleteGame (gameId : int) (session : Session) : Unit AsyncHttpResult =
    if session.isAdmin
    then okTask ()
    else
        GameRepository.getGame gameId
        |> thenBind (fun game ->
            if game.createdByUserId = session.userId
            then Ok ()
            else Error <| HttpException(403, "Cannot delete a game created by another user.")
        )
    |> thenBindAsync (fun _ -> GameRepository.deleteGame gameId)

let private isGameViewableByActiveUser (session : Session) (game : Game) : bool =
    game.parameters.isPublic
    || game.createdByUserId = session.userId
    || game.players |> List.exists(fun p -> p.userId = Some session.userId)

let getGames (query : GamesQuery) (session : Session) : Game list AsyncHttpResult =
    GameRepository.getGames query
    |> thenMap (fun games ->
        if session.isAdmin
        then games
        else games |> List.filter (isGameViewableByActiveUser session)
    )

let getGame (gameId : int) (session : Session) : Game AsyncHttpResult =
    GameRepository.getGame gameId
    |> thenBind (fun game ->
        if isGameViewableByActiveUser session game
        then Ok <| game
        else Error <| HttpException(404, "Game not found.")        
    )