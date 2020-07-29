namespace Apex.Api.Host

open System
open System.Diagnostics
open System.Threading.Tasks
open FSharp.Control.Tasks
open Microsoft.AspNetCore.Http
open Serilog
open Serilog.Context

type LoggingMiddelware(next : RequestDelegate) =
    member __.Invoke(ctx : HttpContext) : Task =
        task {
            let method = ctx.Request.Method
            let path = ctx.Request.Path

            use _ = LogContext.PushProperty("RequestId", Guid.NewGuid())
            Log.Logger.Information("Starting request {method} {path}", method, path)
            
            let timer = Stopwatch()
            timer.Start()

            let! _ =  next.Invoke(ctx) 
            
            let status = ctx.Response.StatusCode
            timer.Stop()
            let seconds = timer.Elapsed.TotalSeconds;

            Log.Logger.Information("Completed request {method} {path} in {seconds:N3} seconds, status {status}",
                                   method, path, seconds, status)
            
            return ()
        } :> Task