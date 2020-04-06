namespace Apex.Api.Web.Controllers

open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open FSharp.Control.Tasks
open Serilog
open Apex.Api.Logic.Interfaces
open Apex.Api.Web
open Apex.Api.Web.Mappings
open Apex.Api.Web.Model

[<ApiController>]
[<Route("api/games/{gameId}/current-turn")>]
type TurnController(manager : ITurnManager,
                       logger : ILogger,
                       scp : SessionContextProvider) =
    inherit ControllerBase()
    
    [<HttpPost("selection-request/{cellId}")>]
    [<ProducesResponseType(200, Type = typeof<StateAndEventResponseDto>)>]
    member __.SelectCell(gameId : int, cellId : int) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let! session = scp.GetSessionFromContext ctx
            let! response = manager.selectCell (gameId, cellId) session
            let dto = response |> toStateAndEventResponseDto
            return OkObjectResult(dto) :> IActionResult
        }
        
    [<HttpPost("reset-request")>]
    [<ProducesResponseType(200, Type = typeof<StateAndEventResponseDto>)>]
    member __.ResetTurn(gameId : int) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let! session = scp.GetSessionFromContext ctx
            let! response = manager.resetTurn gameId session
            let dto = response |> toStateAndEventResponseDto
            return OkObjectResult(dto) :> IActionResult
        }
    
    [<HttpPost("commit-request")>]
    [<ProducesResponseType(200, Type = typeof<StateAndEventResponseDto>)>]
    member __.CommitTurn(gameId : int) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let! session = scp.GetSessionFromContext ctx
            let! response = manager.commitTurn gameId session
            let dto = response |> toStateAndEventResponseDto
            return OkObjectResult(dto) :> IActionResult
        }
    