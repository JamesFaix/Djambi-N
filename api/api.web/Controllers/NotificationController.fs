namespace Apex.Api.Web.Controllers

open System
open System.Threading
open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Primitives

open FSharp.Control.Tasks
open Serilog

open Apex.Api.Common.Control
open Apex.Api.Logic.Interfaces
open Apex.Api.Model
open Apex.Api.Web
open Apex.Api.Web.Sse
open Apex.Api.Web.Websockets

[<ApiController>]
[<Route("api/notifications")>]
type NotificationController(service : INotificationService,
                       logger : ILogger,
                       scp : SessionContextProvider) =
    inherit ControllerBase()
    
    let contentType = "text/event-stream"
    let checkForCloseDelay = TimeSpan.FromSeconds(3.0)


    [<HttpGet("sse")>]
    [<ProducesResponseType(200)>]
    member __.ConnectSse() : Task =
        let ctx = base.HttpContext

        if (ctx.Request.Headers.["Accept"] <> StringValues(contentType))
        then raise <| InvalidWebRequestException(sprintf "Accept header must be '%s'." contentType)
        
        task {
            let! session = scp.GetSessionFromContext ctx

            ctx.Response.Headers.["Content-Type"] <- StringValues(contentType)
            ctx.Response.Body.Flush()
            
            let userId = session.user.id
            let subscriber = new SseSubscriber(userId, ctx.Response, logger)

            service.add subscriber

            while not ctx.RequestAborted.IsCancellationRequested do
                let! _ = Task.Delay checkForCloseDelay
                ()
            service.remove userId
            return ()
        } :> Task
    
    [<HttpGet("ws")>]
    [<ProducesResponseType(200)>]
    member __.ConnectWebSockets() : Task =
        let ctx = base.HttpContext

        if not ctx.WebSockets.IsWebSocketRequest
        then raise <| InvalidWebRequestException("This endpoint requires a websocket request.")

        task {
            let! session = scp.GetSessionFromContext ctx

            let! socket = ctx.WebSockets.AcceptWebSocketAsync()

            let userId = session.user.id
            let subscriber = new WebsocketSubscriber(userId, socket, logger)
            service.add subscriber
                            
            let buffer : byte[] = Array.zeroCreate 4096
            let! _ = socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None)
            
            return ()                   
        } :> Task