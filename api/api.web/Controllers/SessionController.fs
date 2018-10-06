namespace Djambi.Api.Web.Controllers

open System
open Giraffe
open Microsoft.AspNetCore.Http
open Djambi.Api.Common
open Djambi.Api.Common.Utilities
open Djambi.Api.Logic.Services
open Djambi.Api.Web
open Djambi.Api.Web.HttpUtility
open Djambi.Api.Web.Mappings.LobbyWebMapping
open Djambi.Api.Web.Model.LobbyWebModel

module SessionController =

    let appendCookie (ctx : HttpContext) (sessionToken : string, expiration : DateTimeOffset) =
        let cookieOptions = new CookieOptions()
        cookieOptions.Domain <- "localhost" //TODO: Move this to a config file
        cookieOptions.Path <- "/"
        cookieOptions.Secure <- false
        cookieOptions.HttpOnly <- true
        cookieOptions.Expires <- expiration |> toNullable
        ctx.Response.Cookies.Append(cookieName, sessionToken, cookieOptions);
    
    let signIn : HttpHandler =
        let func (ctx : HttpContext) =
            task {
                //Can't login if you already have a session.
                let cookie = ctx.Request.Cookies.Item(HttpUtility.cookieName)
                if cookie |> String.IsNullOrEmpty |> not
                then  raise (HttpException(401, "You are already logged in. To change users, log out and log in again."))

                let! request = ctx.BindModelAsync<LoginRequestJsonModel>()
                              |> Task.map mapLoginRequestFromJson

                let! (sessionToken, expiration) = SessionService.signIn request

                appendCookie ctx (sessionToken, expiration)
            }
        handle func

    let signOut : HttpHandler =
        let func (ctx : HttpContext) =
            task {
                try
                    let! user = HttpUtility.getUserFromContext ctx
                    let! _ = SessionService.signOut user
                    ()
                finally
                    //Always clear the cookie, even if the DB does not have a session matching it
                    appendCookie ctx ("", DateTimeOffset.MinValue)
            }
        handle func
        