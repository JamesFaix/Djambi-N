namespace Apex.Api.Host

open System.Diagnostics
open System.Threading.Tasks
open FSharp.Control.Tasks
open Microsoft.AspNetCore.Http
open Serilog

type LoggingMiddelware(next : RequestDelegate) =
    member __.Invoke(ctx : HttpContext) : Task =
        task {
            let method = ctx.Request.Method
            let path = ctx.Request.Path

            // The property RequestPath is populated by microsoft. 
            // Explicitly adding it here to make templated message look right.
            Log.Logger.Information("Starting request {method} {RequestPath}", method, path)
            
            let timer = Stopwatch()
            timer.Start()

            let! _ =  next.Invoke(ctx) 
            
            let status = ctx.Response.StatusCode
            timer.Stop()
            let seconds = timer.Elapsed.TotalSeconds;

            Log.Logger.Information("Completed request {method} {RequestPath} in {seconds:N3} seconds, status {status}",
                                   method, path, seconds, status)
            
            return ()
        } :> Task