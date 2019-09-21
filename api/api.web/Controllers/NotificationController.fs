namespace Djambi.Api.Web.Controllers

open System
open System.Threading
open System.Threading.Tasks
open FSharp.Control.Tasks
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Primitives
open Serilog
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Logic.Interfaces
open Djambi.Api.Web
open Djambi.Api.Web.Interfaces
open Djambi.Api.Web.Sse
open Djambi.Api.Web.Websockets

type NotificationController(u : HttpUtility,
                            notificationService : INotificationService,
                            log : ILogger) =

    let contentType = "text/event-stream"
    let checkForCloseDelay = TimeSpan.FromSeconds(3.0)

    let checkAcceptHeader (ctx : HttpContext) =
         if (ctx.Request.Headers.["Accept"] <> StringValues(contentType))
         then Error <| HttpException(400, sprintf "Accept header must be '%s'." contentType)
         else Ok ()

    let serverSentEventsHandler (ctx : HttpContext) : unit AsyncHttpResult =
        checkAcceptHeader ctx
        |> Result.bindAsync (fun _ -> u.getSessionFromContext ctx)
        |> thenMap (fun session ->
            ctx.Response.Headers.["Content-Type"] <- StringValues(contentType)
            ctx.Response.Body.Flush()

            let userId = session.user.id
            let subscriber = SseSubscriber(userId, ctx.Response, log)

            notificationService.add subscriber

            userId
        )
        |> thenBindAsync (fun userId ->
            task {
                while not ctx.RequestAborted.IsCancellationRequested do
                    let! _ = Task.Delay checkForCloseDelay
                    ()
                notificationService.remove userId
                return Ok ()
            }
        )

    let websocketsHandler (ctx : HttpContext) : unit AsyncHttpResult = 
        u.getSessionFromContext ctx
        |> thenBindAsync (fun session -> 
            ctx.WebSockets.AcceptWebSocketAsync()
            |> Task.bind (fun socket -> 

                let userId = session.user.id
                let subscriber = WebsocketSubscriber(userId, socket, log)
                notificationService.add subscriber
                
                let buffer : byte[] = Array.zeroCreate 4096
                socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None)
            )
            |> Task.map (fun _ -> Ok ())
        )

    interface INotificationsController with
        member x.getNotificationsForCurrentUser =
            let func (ctx : HttpContext) =
                if ctx.WebSockets.IsWebSocketRequest
                then websocketsHandler ctx
                else serverSentEventsHandler ctx
            u.handle func