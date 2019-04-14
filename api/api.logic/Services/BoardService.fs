namespace Djambi.Api.Logic.Services

open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Logic.ModelExtensions
open Djambi.Api.Logic.ModelExtensions.BoardModelExtensions
open Djambi.Api.Model

type BoardService() =
    member x.getBoard (regionCount : int) (session : Session) : Board AsyncHttpResult =
        okTask <| (BoardModelUtility.getBoard regionCount)

    member x.getCellPaths (regionCount :int, cellId : int) (session : Session) : int list list AsyncHttpResult =
        let board = BoardModelUtility.getBoardMetadata(regionCount)
        let cell = board.cells() |> Seq.find(fun c -> c.id = cellId)
        board.pathsFromCell(cell)
        |> List.map (List.map (fun c -> c.id))
        |> okTask