module Djambi.Api.Logic.Services.GameService

open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.Model

let getGame(gameId : int) (session : Session) : Game AsyncHttpResult =
    GameRepository.getGame gameId
    //TODO: Must be either
        //Admin
        //User in game