module Djambi.Api.Web.Managers.GameManager

open Djambi.Api.Logic.Services
open Djambi.Api.Common
open Djambi.Api.Model

let getGameState (gameId : int) (session : Session) : GameState AsyncHttpResult =
    GameService.getGameState gameId session

let selectCell (gameId : int, cellId : int) (session : Session) : TurnState AsyncHttpResult =
    TurnService.selectCell (gameId, cellId) session

let resetTurn (gameId : int) (session : Session) : TurnState AsyncHttpResult =
    TurnService.resetTurn gameId session

let commitTurn (gameId : int) (session : Session) : CommitTurnResponse AsyncHttpResult =
    TurnService.commitTurn gameId session