namespace Apex.Api.Web

open System
open System.IO
open System.Threading.Tasks
open FSharp.Control.Tasks
open Giraffe
open Microsoft.AspNetCore.Http
open Newtonsoft.Json
open Serilog
open Apex.Api.Common
open Apex.Api.Common.Control
open Apex.Api.Common.Control.AsyncHttpResult
open Apex.Api.Common.Json
open Apex.Api.Logic.Interfaces
open Apex.Api.Model.SessionModel
open Microsoft.Extensions.Options
open Apex.Api.Model.Configuration

type HttpHandler = HttpFunc -> HttpContext -> HttpContext option Task

type HttpUtility(options : IOptions<ApiSettings>,
                 sessionServ : ISessionService,
                 log : ILogger) =

    member x.cookieName = "ApexSession"

    member x.appendCookie (ctx : HttpContext) (token : string, expiration : DateTime) =
        let cookieOptions = CookieOptions()
        cookieOptions.Domain <- options.Value.cookieDomain
        cookieOptions.Path <- "/"
        cookieOptions.Secure <- false
        cookieOptions.HttpOnly <- true
        cookieOptions.Expires <- DateTimeOffset(expiration) |> Nullable.ofValue
        ctx.Response.Cookies.Append(x.cookieName, token, cookieOptions);

    member x.appendEmptyCookie (ctx : HttpContext) =
        x.appendCookie ctx ("", DateTime.MinValue)

    member x.handle<'a> (func : HttpContext -> 'a AsyncHttpResult) : HttpHandler =

        fun (next : HttpFunc) (ctx : HttpContext) ->
            let route = sprintf "%s %s" ctx.Request.Method (ctx.Request.Path.ToString())
            log.Information(sprintf "API Req: %s" route)
            task {
                try
                    let! result = func ctx
                    match result with
                    | Ok value ->
                        match ctx.Response.ContentType with
                        | "text/event-stream" ->
                            return! next ctx
                        | _ ->
                            return! json value next ctx
                    | Error ex ->
                        ctx.SetStatusCode ex.statusCode
                        return! json ex.Message next ctx
                with
                | :? HttpException as ex ->
                    ctx.SetStatusCode ex.statusCode
                    return! json ex.Message next ctx
                | ex ->
                    ctx.SetStatusCode 500
                    return! json ex.Message next ctx
            }
            |> Task.map(fun x -> 
                match x with 
                | Some c ->
                    log.Information(sprintf "API Rsp: %s %i" route c.Response.StatusCode)
                | _ -> ()
                x
            )

    member x.getSessionOptionFromContext (ctx : HttpContext) : Session option AsyncHttpResult =
        let token = ctx.Request.Cookies.Item(x.cookieName)

        if token |> String.IsNullOrEmpty
        then okTask <| None
        else
            task {
                let! result = sessionServ.getSession token
                return match result with
                        | Ok session -> Ok <| Some(session)
                        | Error ex when ex.statusCode = 404 -> Ok(None)
                        | Error ex ->
                            x.appendEmptyCookie ctx
                            Error ex
            }

    member x.getSessionFromContext (ctx : HttpContext) : Session AsyncHttpResult =
        x.getSessionOptionFromContext ctx
        |> thenBind (fun opt ->
            match opt with
            | None ->
                x.appendEmptyCookie ctx
                Error <| HttpException(401, "Not signed in.")
            | Some session -> Ok session
        )