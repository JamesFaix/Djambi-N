namespace Apex.Api.Logic.Services

open Apex.Api.Common.Control
open Apex.Api.Common.Control.AsyncHttpResult
open Apex.Api.Logic.ModelExtensions
open Apex.Api.Logic.ModelExtensions.BoardModelExtensions
open Apex.Api.Model

type BoardService() =
    member x.getBoard (regionCount : int) (session : Session) : Board AsyncHttpResult =
        okTask <| (BoardModelUtility.getBoard regionCount)

    member x.getCellPaths (regionCount :int, cellId : int) (session : Session) : int list list AsyncHttpResult =
        let board = BoardModelUtility.getBoardMetadata(regionCount)
        let cell = board.cells() |> Seq.find(fun c -> c.id = cellId)
        board.pathsFromCell(cell)
        |> List.map (List.map (fun c -> c.id))
        |> okTask