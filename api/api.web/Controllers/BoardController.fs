module Djambi.Api.Web.Controllers.BoardController

open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Logic.ModelExtensions
open Djambi.Api.Logic.ModelExtensions.BoardModelExtensions
open Djambi.Api.Web.HttpUtility

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
