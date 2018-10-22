module Djambi.Api.Logic.Services.BoardService

open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Logic.ModelExtensions
open Djambi.Api.Logic.ModelExtensions.BoardModelExtensions
open Djambi.Api.Model.BoardModel
open Djambi.Api.Model.SessionModel

let getBoard (regionCount : int) (session : Session) : Board AsyncHttpResult =
    okTask <| (BoardModelUtility.getBoard regionCount)

let getCellPaths (regionCount :int, cellId : int) (session : Session) : int list list AsyncHttpResult =
    let board = BoardModelUtility.getBoardMetadata(regionCount)
    let cell = board.cells() |> Seq.find(fun c -> c.id = cellId)
    board.pathsFromCell(cell)
    |> List.map (List.map (fun c -> c.id))
    |> okTask