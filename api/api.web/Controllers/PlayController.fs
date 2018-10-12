module Djambi.Api.Web.Controllers.PlayController

open System
open System.Threading.Tasks
open Djambi.Api.Common
open Djambi.Api.Logic.ModelExtensions
open Djambi.Api.Logic.ModelExtensions.BoardModelExtensions
open Djambi.Api.Logic.Services
open Djambi.Api.Web.HttpUtility
open Djambi.Api.Web.Mappings.PlayWebMapping

let startGame(gameId: int) =
    let func ctx = 
        GameStartService.startGame gameId
        |> Task.thenMap mapGameStartResponseToJson
    handle func

//Board
let getBoard(regionCount : int) =
    let func ctx =
        BoardModelUtility.getBoard regionCount
        |> Ok 
        |> Task.FromResult
    handle func
            
let getCellPaths(regionCount : int, cellId : int) =
    let func ctx = 
        let board = BoardModelUtility.getBoardMetadata(regionCount)
        let cell = board.cells() |> Seq.find(fun c -> c.id = cellId)
        board.pathsFromCell(cell)
        |> List.map (List.map (fun c -> c.id))
        |> Ok 
        |> Task.FromResult
    handle func

//Gameplay
let getGameState(gameId : int) =
    let func ctx =
        PlayService.getGameState(gameId)
        |> Task.thenMap mapGameStateToJsonModel
    handle func

let selectCell(gameId : int, cellId : int) =
    let func ctx = 
        PlayService.selectCell(gameId, cellId)
        |> Task.thenMap mapTurnStateToJsonModel
    handle func

let resetTurn(gameId : int) =
    let func ctx =
        PlayService.resetTurn gameId
        |> Task.thenMap mapTurnStateToJsonModel
    handle func

let commitTurn(gameId : int) =
    let func ctx =
        PlayService.commitTurn gameId
        |> Task.thenMap mapCommitTurnResponseToJsonModel
    handle func

let sendMessage(gameId : int) =
    let func ctx = 
        raise (NotImplementedException "")
    handle func