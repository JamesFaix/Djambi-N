namespace Apex.Api.Web.Controllers2

open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open FSharp.Control.Tasks
open Serilog
open Apex.Api.Common.Control.AsyncHttpResult
open Apex.Api.Logic.Interfaces
open Apex.Api.Model
open Apex.Api.Web
open Microsoft.Extensions.Primitives
open Microsoft.AspNetCore.Http
open Apex.Api.Common.Control
open System
open Apex.Api.Web.Sse

[<ApiController>]
[<Route("notifications")>]
type NotificationController(service : INotificationService,
                       logger : ILogger,
                       util : HttpUtility) =
    inherit ControllerBase()
    
    let contentType = "text/event-stream"
    let checkForCloseDelay = TimeSpan.FromSeconds(3.0)

    let checkAcceptHeader (ctx : HttpContext) =
         if (ctx.Request.Headers.["Accept"] <> StringValues(contentType))
         then Error <| HttpException(400, sprintf "Accept header must be '%s'." contentType)
         else Ok ()
    

    [<HttpGet("sse")>]
    [<ProducesResponseType(200)>]
    member __.ConnectSse() : Task<IActionResult> =
        raise <| NotImplementedException()
        //let ctx = base.HttpContext
        //checkAcceptHeader ctx
        //|> Apex.Api.Common.Control.Result.bindAsync (fun _ -> util.getSessionFromContext ctx)
        //|> thenMap (fun session ->
        //    ctx.Response.Headers.["Content-Type"] <- StringValues(contentType)
        //    ctx.Response.Body.Flush()

        //    let userId = session.user.id
        //    let subscriber = new SseSubscriber(userId, ctx.Response, log)

        //    service.add subscriber

        //    userId
        //)
        //|> thenBindAsync (fun userId ->
        //    task {
        //        while not ctx.RequestAborted.IsCancellationRequested do
        //            let! _ = Task.Delay checkForCloseDelay
        //            ()
        //        service.remove userId
        //        return Ok ()
        //    }
        //)
    
    [<HttpGet("ws")>]
    [<ProducesResponseType(200)>]
    member __.ConnectWebSockets() : Task<IActionResult> =
        raise <| NotImplementedException()
        //let ctx = base.HttpContext
        //task {
        //    let board =
        //        util.getSessionFromContext ctx
        //        |> thenBindAsync (fun session ->
        //            manager.getCellPaths (regionCount, cellId) session
        //        )
        //        |> thenExtract

        //    return OkObjectResult(board) :> IActionResult
        //}