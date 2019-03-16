namespace Djambi.Api.Web.Controllers

open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Logic.Managers
open Djambi.Api.Model
open Djambi.Api.Web.Interfaces
open Djambi.Api.Web

type EventController(u : HttpUtility) =
    interface IEventController with
        member x.getEvents gameId =
            let func ctx =
                u.getSessionAndModelFromContext<EventsQuery> ctx
                |> thenBindAsync (fun (query, session) -> 
                    GameManager.getEvents (gameId, query) session
                )
            u.handle func