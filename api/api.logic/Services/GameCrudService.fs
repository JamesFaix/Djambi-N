module Djambi.Api.Logic.Services.GameCrudService

open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.Model

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