namespace Apex.Api.Host

open System
open System.Threading.Tasks
open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks
open Apex.Api.Common.Control
open System.ComponentModel
open System.Security.Authentication

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
                    | :? AuthenticationException as e -> (401, e.Message)
                    | :? InvalidEnumArgumentException as e -> (400, e.Message)
                    | :? HttpException as e1 -> (e1.statusCode, e1.Message)
                    | :? AggregateException as e1 ->
                        match e1.InnerExceptions.[0] with
                        | :? AuthenticationException as e -> (401, e.Message)
                        | :? InvalidEnumArgumentException as e -> (400, e.Message)
                        | :? HttpException as e2 -> (e2.statusCode, e2.Message)
                        | _ as e1 -> (500, e1.Message)
                    | _ as e1 -> (500, e1.Message)

                ctx.Response.ContentType <- "application/json"
                ctx.Response.StatusCode <- statusCode
                let! _ = ctx.Response.WriteAsync message
                ()
            return ()
        } :> Task