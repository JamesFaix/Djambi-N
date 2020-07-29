namespace Apex.Api.Web

open System
open System.Security.Authentication
open System.Threading.Tasks
open FSharp.Control.Tasks
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Options
open Serilog
open Apex.Api.Common.Control
open Apex.Api.Logic.Interfaces
open Apex.Api.Model
open Apex.Api.Model.Configuration

type SessionContextProvider(options : IOptions<ApiSettings>,
                            service : ISessionService,
                            cookieProvider : CookieProvider,
                            logger : ILogger) =
    
    member x.GetSessionOptionFromContext (ctx : HttpContext) : Task<Option<Session>> =

        let cookieName = options.Value.cookieName
        let token = ctx.Request.Cookies.Item(cookieName)

        logger.Information("GetSessionOptionFromContext {cookieName} {token}", cookieName, token)

        if token |> String.IsNullOrEmpty
        then 
            logger.Information("No token")
            None |> Task.FromResult
        else
            task {
                try 
                    match! service.getAndRenewSession token with
                    | Some session -> 
                        logger.Information("Found session matching token")
                        return Some session
                    | None ->
                        logger.Information("No session matching token")
                        cookieProvider.AppendEmptyCookie ctx
                        return None
                with
                | _ as ex ->
                    logger.Error(ex, "Error finding session")
                    cookieProvider.AppendEmptyCookie ctx
                    return raise ex
            }

    member x.GetSessionFromContext (ctx : HttpContext) : Task<Session> =
        task{
            match! x.GetSessionOptionFromContext ctx with
            | None -> return raise <| AuthenticationException("Not signed in.")
            | Some session -> return session
        }