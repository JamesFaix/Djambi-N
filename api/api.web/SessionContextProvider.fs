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
                    let! session = service.getSession token
                    return Some(session)
                with
                | :? HttpException as ex when ex.statusCode = 404 ->
                    return None
                | :? HttpException as ex ->
                    cookieProvider.AppendEmptyCookie ctx
                    return raise ex
            }

    member x.GetSessionFromContext (ctx : HttpContext) : Task<Session> =
        task{
            let! sessionOption = x.GetSessionOptionFromContext ctx
            if sessionOption.IsNone
            then raise <| HttpException(401, "Not signed in.")
            return sessionOption.Value
        }