namespace Apex.Api.Web

open System
open System.Threading.Tasks
open FSharp.Control.Tasks
open Giraffe
open Microsoft.AspNetCore.Http
open Apex.Api.Common
open Apex.Api.Common.Control
open Apex.Api.Common.Control.AsyncHttpResult
open Apex.Api.Logic.Interfaces
open Apex.Api.Model.SessionModel
open Microsoft.Extensions.Options
open Apex.Api.Model.Configuration

type HttpUtility(options : IOptions<ApiSettings>,
                 sessionServ : ISessionService) =

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