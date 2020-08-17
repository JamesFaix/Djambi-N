namespace Djambi.Api.Web.Controllers

open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open FSharp.Control.Tasks
open Serilog
open Djambi.Api.Logic.Interfaces
open Djambi.Api.Web
open Djambi.Api.Web.Model
open Djambi.Api.Web.Mappings

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
    