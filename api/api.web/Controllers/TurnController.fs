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
[<Route("api/games/{gameId}/current-turn")>]
type TurnController(manager : ITurnManager,
                       logger : ILogger,
                       scp : SessionContextProvider) =
    inherit ControllerBase()
    
    [<HttpPost("selection-request/{cellId}")>]
    [<ProducesResponseType(200, Type = typeof<StateAndEventResponse>)>]
    member __.SelectCell(gameId : int, cellId : int) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let! session = scp.GetSessionFromContext ctx
            let! response = manager.selectCell (gameId, cellId) session |> thenExtract
            return OkObjectResult(response) :> IActionResult
        }
        
    [<HttpPost("reset-request")>]
    [<ProducesResponseType(200, Type = typeof<StateAndEventResponse>)>]
    member __.ResetTurn(gameId : int) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let! session = scp.GetSessionFromContext ctx
            let! response = manager.resetTurn gameId session |> thenExtract
            return OkObjectResult(response) :> IActionResult
        }
    
    [<HttpPost("commit-request")>]
    [<ProducesResponseType(200, Type = typeof<StateAndEventResponse>)>]
    member __.CommitTurn(gameId : int) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let! session = scp.GetSessionFromContext ctx
            let! response = manager.commitTurn gameId session |> thenExtract
            return OkObjectResult(response) :> IActionResult
        }
    