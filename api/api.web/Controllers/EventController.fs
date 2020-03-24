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
[<Route("api/events")>]
type EventController(manager : IEventManager,
                    logger : ILogger,
                    scp : SessionContextProvider) =
    inherit ControllerBase()
    
    [<HttpGet("{gameId}")>]
    [<ProducesResponseType(200, Type = typeof<Event[]>)>]
    member __.GetEvents(gameId : int, [<FromBody>] query : EventsQuery) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let! session = scp.GetSessionFromContext ctx
            let! events = manager.getEvents (gameId, query) session |> thenExtract
            return OkObjectResult(events) :> IActionResult
        }
    