namespace Apex.Api.Host

open System
open System.ComponentModel
open System.ComponentModel.DataAnnotations
open System.Data
open System.Data.Entity.Core
open System.Security.Authentication
open System.Threading.Tasks
open FSharp.Control.Tasks
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc
open MySql.Data.MySqlClient
open Newtonsoft.Json
open Serilog
open Apex.Api.Common.Control

type ErrorHandlingMiddleware(next : RequestDelegate) =

    let getStatus (ex : Exception) : int =
        match ex with
        | :? GameConfigurationException -> 400
        | :? GameRuleViolationException -> 400
        | :? InvalidEnumArgumentException -> 400
        | :? InvalidWebRequestException -> 400
        | :? ValidationException -> 400
        | :? AuthenticationException -> 401
        | :? UnauthorizedAccessException -> 403
        | :? NotFoundException -> 404
        | :? ObjectNotFoundException -> 404
        | :? DuplicateNameException -> 409
        | :? ApexWebsocketException -> 500
        | _ -> 500

    let getMessage (ex : Exception) : string =
        match ex with
        | :? MySqlException as e when e.Message.StartsWith("Access denied") -> 
            "Error connecting to database."
        | _ -> ex.Message

    let unNest (ex : Exception) : Exception =
        match ex with
        | :? AggregateException as e -> e.InnerExceptions.[0]
        | _ -> ex
    
    let toProblem (ex : Exception) : ProblemDetails =
        let ex = unNest ex;
        let message = getMessage ex;
        let status = getStatus ex

        let p = ProblemDetails()
        p.Status <- Nullable(status)
        p.Title <- message
        p

    member __.Invoke(ctx : HttpContext) : Task =
        task {
            try
                // It looks like we should be able to use "return" here, but if we don't 
                // force an "await", the exceptions don't get caught.
                let! _ =  next.Invoke(ctx) 
                ()
            with
            | _ as ex ->
                Log.Logger.Warning(ex, "Error caught by middleware")
                let p = ex |> toProblem
                let json = p |> JsonConvert.SerializeObject

                ctx.Response.ContentType <- "application/json"
                ctx.Response.StatusCode <- p.Status.Value
                let! _ = ctx.Response.WriteAsync json
                ()
            return ()
        } :> Task