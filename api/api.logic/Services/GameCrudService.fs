module Djambi.Api.Logic.Services.GameCrudService

open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.Model

let createGame (parameters : GameParameters) (session : Session) : Game AsyncHttpResult =
    //Create game
    GameRepository.createGame (parameters, session.userId)
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

let getGames (query : GamesQuery) (session : Session) : Game list AsyncHttpResult =
    let isViewableByActiveUser (game : Game) : bool =
        game.parameters.isPublic
        || game.createdByUserId = session.userId

    GameRepository.getGames query
    |> thenMap (fun games ->
        if session.isAdmin
        then games
        else games |> List.filter isViewableByActiveUser
    )

let getGame (gameId : int) (session : Session) : Game AsyncHttpResult =
    GameRepository.getGame gameId
    |> thenBind (fun game ->
        if (game.parameters.isPublic
            || game.createdByUserId = session.userId
            || game.players |> List.exists(fun p -> p.userId.IsSome
                                                 && p.userId.Value = session.userId))
        then Ok <| game
        else Error <| HttpException(404, "Game not found.")        
    )

let updateGameParameters (gameId : int, parameters : GameParameters) (session : Session) : Game AsyncHttpResult =
    GameRepository.getGame gameId
    |> thenBindAsync (fun game -> 
        if not (session.isAdmin || session.userId = game.createdByUserId)
        then errorTask <| HttpException(403, "Cannot change game parameters of game created by another user.")
        else 
            GameRepository.updateGameParameters gameId parameters
            |> thenBindAsync (fun _ -> GameRepository.getGame gameId)    
    )