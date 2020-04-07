namespace Apex.Api.Web

open System
open System.Threading.Tasks
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Options

open FSharp.Control.Tasks

open Apex.Api.Common.Control
open Apex.Api.Logic.Interfaces
open Apex.Api.Model
open Apex.Api.Model.Configuration
open System.Security.Authentication

type SessionContextProvider(options : IOptions<ApiSettings>,
                            service : ISessionService,
                            cookieProvider : CookieProvider) =
    
    member x.GetSessionOptionFromContext (ctx : HttpContext) : Task<Option<Session>> =
        let cookieName = options.Value.cookieName
        let token = ctx.Request.Cookies.Item(cookieName)

        if token |> String.IsNullOrEmpty
        then None |> Task.FromResult
        else
            task {
                try 
                    match! service.getAndRenewSession token with
                    | Some session -> return Some session
                    | None ->
                        cookieProvider.AppendEmptyCookie ctx
                        return None
                with
                | _ as ex ->
                    cookieProvider.AppendEmptyCookie ctx
                    return raise ex
            }

    member x.GetSessionFromContext (ctx : HttpContext) : Task<Session> =
        task{
            match! x.GetSessionOptionFromContext ctx with
            | None -> return raise <| AuthenticationException("Not signed in.")
            | Some session -> return session
        }