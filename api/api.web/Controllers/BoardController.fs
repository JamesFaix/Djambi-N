namespace Apex.Api.Web.Controllers

open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open FSharp.Control.Tasks
open Serilog
open Apex.Api.Common.Control.AsyncHttpResult
open Apex.Api.Logic.Interfaces
open Apex.Api.Web
open Apex.Api.Web.Mappings
open Apex.Api.Web.Model

[<ApiController>]
[<Route("api/boards")>]
type BoardController(manager : IBoardManager,
                       logger : ILogger,
                       scp : SessionContextProvider) =
    inherit ControllerBase()
    
    /// <summary> Gets the board with the given region count. </summary>
    /// <param name="regionCount"> The number of regions in the board. </param>
    /// <response code="200"> The board. </response>
    [<HttpGet("{regionCount}")>]
    [<ProducesResponseType(200, Type = typeof<BoardDto>)>]
    member __.GetBoard(regionCount : int) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let! session = scp.GetSessionFromContext ctx
            let! board = manager.getBoard regionCount session |> thenExtract
            let dto = board |> toBoardDto
            return OkObjectResult(dto) :> IActionResult
        }
    
    [<HttpGet("{regionCount}/cells/{cellId}")>]
    [<ProducesResponseType(200, Type = typeof<List<List<int>>>)>]
    member __.GetCellPaths(regionCount : int, cellId : int) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let! session = scp.GetSessionFromContext ctx
            let! paths = manager.getCellPaths (regionCount, cellId) session |> thenExtract
            return OkObjectResult(paths) :> IActionResult
        }