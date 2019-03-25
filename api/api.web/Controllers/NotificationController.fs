namespace Djambi.Api.Web.Controllers

open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Primitives
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Logic.Interfaces
open Djambi.Api.Model
open Djambi.Api.Web
open Djambi.Api.Web.Interfaces
open Djambi.Api.Web.Sse

type NotificationController(u : HttpUtility,
                             notificationService : INotificationService) =    

    let contentType = "text/event-stream"

    let checkAcceptHeader (ctx : HttpContext) =
         if not (ctx.Request.Headers.["Accept"] = StringValues(contentType))
         then Error <| HttpException(400, sprintf "Accept header must be '%s'." contentType)
         else Ok ()

    let subscribe (gameId : int option) =
        let func (ctx : HttpContext) =
            checkAcceptHeader ctx
            |> Result.bindAsync (fun _ -> u.getSessionFromContext ctx)
            |> thenBindAsync (fun session ->
                ctx.Response.ContentType <- contentType
                ctx.Response.Body.Flush()

                let subscriberId : SubscriberId = 
                    { 
                        userId = session.user.id
                        gameId = gameId 
                    }
                let subscriber = new SseSubscriber(subscriberId, ctx.Response)
                
                notificationService.add subscriber

                ctx.RequestAborted.WaitHandle.WaitOne() |> ignore

                notificationService.remove subscriberId

                okTask ()
            )
        u.handle func

    interface INotificationsController with

        member x.getNotificationsForCurrentUser =
            subscribe None

        member x.getNotificationsForCurrentUserForGame gameId =
            subscribe (Some gameId)