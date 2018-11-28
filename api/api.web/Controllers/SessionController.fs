module Djambi.Api.Web.Controllers.SessionController

open System
open Microsoft.AspNetCore.Http
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Common.Utilities
open Djambi.Api.Logic.Services
open Djambi.Api.Web
open Djambi.Api.Web.HttpUtility
open Djambi.Api.Web.Mappings.SessionWebMapping
open Djambi.Api.Web.Model.SessionWebModel
open Djambi.Api.Web.Mappings

let appendCookie (ctx : HttpContext) (sessionToken : string, expiration : DateTime) =
    let cookieOptions = new CookieOptions()
    cookieOptions.Domain <- "localhost" //TODO: Move this to a config file
    cookieOptions.Path <- "/"
    cookieOptions.Secure <- false
    cookieOptions.HttpOnly <- true
    cookieOptions.Expires <-  DateTimeOffset(expiration) |> toNullable
    ctx.Response.Cookies.Append(cookieName, sessionToken, cookieOptions);

let openSession : HttpHandler =
    let func (ctx : HttpContext) =
        ensureNotSignedInAndGetModel<LoginRequestJsonModel> ctx
        |> thenMap mapLoginRequestFromJson
        |> thenBindAsync SessionService.openSession
        |> thenBindAsync (fun session ->
            appendCookie ctx (session.token, session.expiresOn)
            UserService.getUser session.userId session
        )
        |> thenMap UserWebMapping.mapUserResponse
        |> thenReplaceError 409 (HttpException(409, "Already signed in."))

    handle func

let closeSession : HttpHandler =
    let func (ctx : HttpContext) =
        //Always clear the cookie, even if the DB does not have a session matching it
        appendCookie ctx ("", DateTime.MinValue)

        getSessionFromContext ctx
        |> thenBindAsync SessionService.closeSession
        |> thenMap ignore

    handle func
