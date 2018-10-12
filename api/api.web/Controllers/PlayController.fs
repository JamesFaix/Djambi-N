module Djambi.Api.Web.Controllers.PlayController

open System
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Logic.ModelExtensions
open Djambi.Api.Logic.ModelExtensions.BoardModelExtensions
open Djambi.Api.Logic.Services
open Djambi.Api.Web.HttpUtility
open Djambi.Api.Web.Mappings.PlayWebMapping

let startGame(gameId: int) =
    let func ctx = 
        GameStartService.startGame gameId
        |> thenMap mapGameStartResponseToJson
    handle func

//Board
let getBoard(regionCount : int) =
    let func ctx =
        BoardModelUtility.getBoard regionCount
        |> okTask
    handle func
            
let getCellPaths(regionCount : int, cellId : int) =
    let func ctx = 
        let board = BoardModelUtility.getBoardMetadata(regionCount)
        let cell = board.cells() |> Seq.find(fun c -> c.id = cellId)
        board.pathsFromCell(cell)
        |> List.map (List.map (fun c -> c.id))
        |> okTask
    handle func

//Gameplay
let getGameState(gameId : int) =
    let func ctx =
        PlayService.getGameState(gameId)
        |> thenMap mapGameStateToJsonModel
    handle func

let selectCell(gameId : int, cellId : int) =
    let func ctx = 
        PlayService.selectCell(gameId, cellId)
        |> thenMap mapTurnStateToJsonModel
    handle func

let resetTurn(gameId : int) =
    let func ctx =
        PlayService.resetTurn gameId
        |> thenMap mapTurnStateToJsonModel
    handle func

let commitTurn(gameId : int) =
    let func ctx =
        PlayService.commitTurn gameId
        |> thenMap mapCommitTurnResponseToJsonModel
    handle func

let sendMessage(gameId : int) =
    let func ctx = 
        raise (NotImplementedException "")
    handle func