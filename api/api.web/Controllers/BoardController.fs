namespace Apex.Api.Web.Controllers

open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open FSharp.Control.Tasks
open Serilog
open Apex.Api.Common.Control.AsyncHttpResult
open Apex.Api.Logic.Interfaces
open Apex.Api.Model
open Apex.Api.Web

[<ApiController>]
[<Route("boards")>]
type BoardController(manager : IBoardManager,
                       logger : ILogger,
                       util : HttpUtility) =
    inherit ControllerBase()
    
    [<HttpGet("{regionCount}")>]
    [<ProducesResponseType(200, Type = typeof<Board>)>]
    member __.GetBoard(regionCount : int) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let board =
                util.getSessionFromContext ctx
                |> thenBindAsync (fun session ->
                    manager.getBoard regionCount session
                )
                |> thenExtract

            return OkObjectResult(board) :> IActionResult
        }
    
    [<HttpGet("{regionCount}/cells/{cellId}")>]
    [<ProducesResponseType(200, Type = typeof<int[][]>)>]
    member __.GetCellPaths(regionCount : int, cellId : int) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let board =
                util.getSessionFromContext ctx
                |> thenBindAsync (fun session ->
                    manager.getCellPaths (regionCount, cellId) session
                )
                |> thenExtract

            return OkObjectResult(board) :> IActionResult
        }