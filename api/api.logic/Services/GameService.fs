module Djambi.Api.Logic.Services.GameService

open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.Model.GameModel
open Djambi.Api.Model.SessionModel

let getGameState(gameId : int) (session : Session) : GameState AsyncHttpResult =
    GameRepository.getGame gameId
    //TODO: Must be either
        //Admin
        //User in game
    |> thenMap (fun g -> g.gameState)
