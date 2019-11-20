namespace Apex.Api.Web.Controllers

open Apex.Api.Common.Control.AsyncHttpResult
open Apex.Api.Model
open Apex.Api.Web.Interfaces
open Apex.Api.Web
open Apex.Api.Logic.Interfaces

type EventController(eventMan : IEventManager,
                     u : HttpUtility) =
    interface IEventController with
        member x.getEvents gameId =
            let func ctx =
                u.getSessionAndModelFromContext<EventsQuery> ctx
                |> thenBindAsync (fun (query, session) ->
                    eventMan.getEvents (gameId, query) session
                )
            u.handle func