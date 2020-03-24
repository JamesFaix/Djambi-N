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
[<Route("api/games")>]
type GameController(manager : IGameManager,
                       logger : ILogger,
                       scp : SessionContextProvider) =
    inherit ControllerBase()
    
    [<HttpGet("{gameId}")>]
    [<ProducesResponseType(200, Type = typeof<Game>)>]
    member __.GetGame(gameId : int) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let! session = scp.GetSessionFromContext ctx
            let! game = manager.getGame gameId session |> thenExtract
            return OkObjectResult(game) :> IActionResult
        }

    [<HttpPost>]
    [<ProducesResponseType(200, Type = typeof<Game>)>]
    member __.CreateGame([<FromBody>] request : GameParameters) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let! session = scp.GetSessionFromContext ctx
            let! game = manager.createGame request session |> thenExtract
            return OkObjectResult(game) :> IActionResult
        }
    
    [<HttpPut("{gameId}/parameters")>]
    [<ProducesResponseType(200, Type = typeof<StateAndEventResponse>)>]
    member __.UpdateGameParameters(gameId : int, [<FromBody>] parameters : GameParameters) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let! session = scp.GetSessionFromContext ctx
            let! response = manager.updateGameParameters gameId parameters session |> thenExtract
            return OkObjectResult(response) :> IActionResult
        }

    [<HttpPost("{gameId}/start-request")>]
    [<ProducesResponseType(200, Type = typeof<StateAndEventResponse>)>]
    member __.StartGame(gameId : int) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let! session = scp.GetSessionFromContext ctx
            let! response = manager.startGame gameId session |> thenExtract
            return OkObjectResult(response) :> IActionResult
        }
