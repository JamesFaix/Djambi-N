module Djambi.Api.Web.Managers.GameManager

open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Logic.Services
open Djambi.Api.Web.Mappings.GameWebMapping
open Djambi.Api.Common
open Djambi.Api.Model.SessionModel
open Djambi.Api.Web.Model.GameWebModel

let getGameState (gameId : int) (session : Session) : GameStateJsonModel AsyncHttpResult =
    GameService.getGameState gameId session
    |> thenMap mapGameStateToJsonModel

let selectCell (gameId : int, cellId : int) (session : Session) : TurnStateJsonModel AsyncHttpResult =
    TurnService.selectCell (gameId, cellId) session
    |> thenMap mapTurnStateToJsonModel

let resetTurn (gameId : int) (session : Session) : TurnStateJsonModel AsyncHttpResult =
    TurnService.resetTurn gameId session
    |> thenMap mapTurnStateToJsonModel

let commitTurn (gameId : int) (session : Session) : CommitTurnResponseJsonModel AsyncHttpResult =
    TurnService.commitTurn gameId session
    |> thenMap mapCommitTurnResponseToJsonModel