namespace Apex.Api.Web.Controllers

open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open FSharp.Control.Tasks
open Serilog
open Apex.Api.Logic.Interfaces
open Apex.Api.Web
open Apex.Api.Web.Model
open Apex.Api.Web.Mappings

[<ApiController>]
[<Route("api/games/{gameId}/events")>]
type EventController(manager : IEventManager,
                    logger : ILogger,
                    scp : SessionContextProvider) =
    inherit ControllerBase()
    
    [<HttpPost("query")>]
    [<ProducesResponseType(200, Type = typeof<EventDto[]>)>]
    member __.GetEvents(gameId : int, [<FromBody>] query : EventsQueryDto) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let! session = scp.GetSessionFromContext ctx
            let query = query |> toEventsQuery
            let! events = manager.getEvents (gameId, query) session
            let dtos = events |> toEventDtos
            return OkObjectResult(dtos) :> IActionResult
        }
    