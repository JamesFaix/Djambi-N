module Djambi.Api.Web.Managers.BoardManager

open Djambi.Api.Logic.Services
open Djambi.Api.Model.SessionModel
open Djambi.Api.Common
open Djambi.Api.Model.BoardModel

let getBoard (regionCount : int) (session : Session) : Board AsyncHttpResult =
    BoardService.getBoard regionCount session
            
let getCellPaths(regionCount : int, cellId : int) (session : Session) : int list list AsyncHttpResult =
    BoardService.getCellPaths (regionCount, cellId) session