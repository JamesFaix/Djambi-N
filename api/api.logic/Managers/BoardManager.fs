module Djambi.Api.Logic.Managers.BoardManager

open Djambi.Api.Common.Control
open Djambi.Api.Logic.Services
open Djambi.Api.Model
open Djambi.ClientGenerator.Annotations

[<ClientFunction(HttpMethod.Get, "/boards/%i", ClientSection.Board)>]
let getBoard (regionCount : int) (session : Session) : Board AsyncHttpResult =
    BoardService.getBoard regionCount session
            
[<ClientFunction(HttpMethod.Get, "/boards/%i/cells/%i/paths", ClientSection.Board)>]
let getCellPaths(regionCount : int, cellId : int) (session : Session) : int list list AsyncHttpResult =
    BoardService.getCellPaths (regionCount, cellId) session