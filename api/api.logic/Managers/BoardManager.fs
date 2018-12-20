module Djambi.Api.Logic.Managers.BoardManager

open Djambi.Api.Logic.Services
open Djambi.Api.Model
open Djambi.Api.Common

let getBoard (regionCount : int) (session : Session) : Board AsyncHttpResult =
    BoardService.getBoard regionCount session
            
let getCellPaths(regionCount : int, cellId : int) (session : Session) : int list list AsyncHttpResult =
    BoardService.getCellPaths (regionCount, cellId) session