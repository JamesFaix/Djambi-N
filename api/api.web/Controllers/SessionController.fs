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

    let appendCookie (ctx : HttpContext) (sessionToken : string, expiration : DateTime) =
        let cookieOptions = new CookieOptions()
        cookieOptions.Domain <- "localhost" //TODO: Move this to a config file
        cookieOptions.Path <- "/"
        cookieOptions.Secure <- false
        cookieOptions.HttpOnly <- true
        cookieOptions.Expires <-  DateTimeOffset(expiration) |> toNullable
        ctx.Response.Cookies.Append(cookieName, sessionToken, cookieOptions);
    
    let createSessionWithUser : HttpHandler =
        let func (ctx : HttpContext) =
            task {
                let token = ctx.Request.Cookies.Item(HttpUtility.cookieName)

                if token |> String.IsNullOrEmpty |> not
                then raise <| HttpException(409, "Already signed in")

                let! request = ctx.BindModelAsync<LoginRequestJsonModel>()
                               |> Task.map mapLoginRequestFromJson

                try 
                    let! session = SessionService.signIn(request.userName, request.password, None)
                
                    appendCookie ctx (session.token, session.expiresOn)

                    return session |> mapSessionResponse
                with
                | :? HttpException as ex when ex.statusCode = 409 ->
                    raise <| HttpException(409, "Already signed in")
                    return Unchecked.defaultof<SessionResponseJsonModel>
            }
        handle func

    let addUserToSession : HttpHandler =
        let func (ctx : HttpContext) =
            task {
                let token = ctx.Request.Cookies.Item(HttpUtility.cookieName)

                if token |> String.IsNullOrEmpty
                then raise <| HttpException(401, "Not signed in")

                let! request = ctx.BindModelAsync<LoginRequestJsonModel>()
                               |> Task.map mapLoginRequestFromJson

                let! session = SessionService.signIn(request.userName, request.password, Some token)
                
                appendCookie ctx (session.token, session.expiresOn)
                                
                return session |> mapSessionResponse
            }
        handle func

    let removeUserFromSession (userId : int) : HttpHandler =
        let func (ctx : HttpContext) =
            task {
                let token = ctx.Request.Cookies.Item(HttpUtility.cookieName)

                if token |> String.IsNullOrEmpty
                then raise <| HttpException(401, "Not signed in")
                
                let! session = SessionService.removeUserFromSession(userId, token)
                
                match session with
                | Some s -> 
                    appendCookie ctx (s.token, s.expiresOn)                
                    return s |> mapSessionResponse
                | None -> 
                    appendCookie ctx ("", DateTime.MinValue)
                    return Unchecked.defaultof<SessionResponseJsonModel>            
            }
        handle func

    let closeSession : HttpHandler =
        let func (ctx : HttpContext) =
            task {
                try
                    let token = ctx.Request.Cookies.Item(HttpUtility.cookieName)
    
                    if token |> String.IsNullOrEmpty
                    then raise <| HttpException(401, "Not signed in")
                    
                    let! _ = SessionService.closeSession token
                    ()
                finally
                    //Always clear the cookie, even if the DB does not have a session matching it
                    appendCookie ctx ("", DateTime.MinValue)
            }
        handle func
        