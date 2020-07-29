namespace Apex.Api.Host

open System
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
            
            let! _ =  next.Invoke(ctx) 
            
            let status = ctx.Response.StatusCode
            Log.Logger.Information("Completed request {method} {path}, status {status}", method, path, status)
            
            return ()
        } :> Task