namespace Apex.Api.Web.Controllers

//open System
//open System.Threading
//open System.Threading.Tasks
//open FSharp.Control.Tasks
//open Microsoft.AspNetCore.Http
//open Microsoft.Extensions.Primitives
//open Serilog
//open Apex.Api.Common.Control
//open Apex.Api.Common.Control.AsyncHttpResult
//open Apex.Api.Logic.Interfaces
//open Apex.Api.Web
//open Apex.Api.Web.Interfaces
//open Apex.Api.Web.Sse
//open Apex.Api.Web.Websockets

//type NotificationController(u : HttpUtility,
//                            notificationService : INotificationService,
//                            log : ILogger) =

//    let contentType = "text/event-stream"
//    let checkForCloseDelay = TimeSpan.FromSeconds(3.0)

//    let checkAcceptHeader (ctx : HttpContext) =
//         if (ctx.Request.Headers.["Accept"] <> StringValues(contentType))
//         then Error <| HttpException(400, sprintf "Accept header must be '%s'." contentType)
//         else Ok ()
         
//    interface INotificationsController with
//        member x.connectSse =
//            let f (ctx : HttpContext) : unit AsyncHttpResult =
//                checkAcceptHeader ctx
//                |> Result.bindAsync (fun _ -> u.getSessionFromContext ctx)
//                |> thenMap (fun session ->
//                    ctx.Response.Headers.["Content-Type"] <- StringValues(contentType)
//                    ctx.Response.Body.Flush()

//                    let userId = session.user.id
//                    let subscriber = new SseSubscriber(userId, ctx.Response, log)

//                    notificationService.add subscriber

//                    userId
//                )
//                |> thenBindAsync (fun userId ->
//                    task {
//                        while not ctx.RequestAborted.IsCancellationRequested do
//                            let! _ = Task.Delay checkForCloseDelay
//                            ()
//                        notificationService.remove userId
//                        return Ok ()
//                    }
//                )
//            u.handle f

//        member x.connectWebSockets =
//            let f (ctx : HttpContext) : unit AsyncHttpResult = 
//                if not ctx.WebSockets.IsWebSocketRequest
//                then
//                    errorTask <| HttpException(400, "This endpoint requires a websocket request.")
//                else
//                    u.getSessionFromContext ctx
//                    |> thenBindAsync (fun session -> 
//                        ctx.WebSockets.AcceptWebSocketAsync()
//                        |> Task.bind (fun socket -> 

//                            let userId = session.user.id
//                            let subscriber = new WebsocketSubscriber(userId, socket, log)
//                            notificationService.add subscriber
                
//                            let buffer : byte[] = Array.zeroCreate 4096
//                            socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None)
//                        )
//                        |> Task.map (fun _ -> Ok ())
//                    )
//            u.handle f