namespace Apex.Api.Host

open System
open System.Threading.Tasks
open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks
open Apex.Api.Common.Control

type ErrorHandlingMiddleware(next : RequestDelegate) =
    
    member __.Invoke(ctx : HttpContext) : Task =
        task {
            try
                // It looks like we should be able to use "return" here, but if we don't 
                // force an "await", the exceptions don't get caught.
                let! _ =  next.Invoke(ctx) 
                ()
            with
            | _ as ex ->
                let (statusCode, message) = 
                    match ex with
                    | :? HttpException as e1 -> (e1.statusCode, e1.Message)
                    | :? AggregateException as e1 ->
                        match e1.InnerExceptions.[0] with
                        | :? HttpException as e2 -> (e2.statusCode, e2.Message)
                        | _ as e1 -> (500, e1.Message)
                    | _ as e1 -> (500, e1.Message)

                ctx.Response.ContentType <- "application/json"
                ctx.Response.StatusCode <- statusCode
                let! _ = ctx.Response.WriteAsync message
                ()
            return ()
        } :> Task