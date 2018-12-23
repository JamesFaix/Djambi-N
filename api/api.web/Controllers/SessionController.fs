module Djambi.Api.Web.Controllers.SessionController

open System
open Microsoft.AspNetCore.Http
open Djambi.Api.Common
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Logic.Managers
open Djambi.Api.Model.SessionModel
open Djambi.Api.Web
open Djambi.Api.Web.HttpUtility

let appendCookie (ctx : HttpContext) (sessionToken : string, expiration : DateTime) =
    let cookieOptions = new CookieOptions()
    cookieOptions.Domain <- "localhost" //TODO: Move this to a config file
    cookieOptions.Path <- "/"
    cookieOptions.Secure <- false
    cookieOptions.HttpOnly <- true
    cookieOptions.Expires <-  DateTimeOffset(expiration) |> Nullable.ofValue
    ctx.Response.Cookies.Append(cookieName, sessionToken, cookieOptions);

let openSession : HttpHandler =
    let func (ctx : HttpContext) =
        ensureNotSignedInAndGetModel<LoginRequest> ctx
        |> thenBindAsync (fun jsonModel -> SessionManager.openSession(jsonModel, appendCookie ctx))
    handle func

let closeSession : HttpHandler =
    let func (ctx : HttpContext) =
        //Always clear the cookie, even if the DB does not have a session matching it
        appendCookie ctx ("", DateTime.MinValue)

        getSessionFromContext ctx
        |> thenBindAsync SessionManager.closeSession

    handle func
