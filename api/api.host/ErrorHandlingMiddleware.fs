namespace Apex.Api.Host

open System.Threading.Tasks
open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks
open Apex.Api.Common.Control

type ErrorHandlingMiddleware(next : RequestDelegate) =
    
    member __.Invoke(ctx : HttpContext) : Task =
        task {
            try
                return next.Invoke(ctx)
            with
            | :? HttpException as ex ->
                ctx.Response.ContentType <- "application/json"
                ctx.Response.StatusCode <- ex.statusCode
                return ctx.Response.WriteAsync(ex.Message)
            | _ as ex ->
                ctx.Response.ContentType <- "application/json"
                ctx.Response.StatusCode <- 500
                return ctx.Response.WriteAsync(ex.Message)
        } :> Task