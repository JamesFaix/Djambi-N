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
                let! result = service.getSession token

                let sessionOption = 
                    match result with
                    | Ok session -> Some(session)
                    | Error ex when ex.statusCode = 404 -> None
                    | Error ex ->
                        cookieProvider.AppendEmptyCookie ctx
                        raise ex

                return sessionOption
            }

    member x.GetSessionFromContext (ctx : HttpContext) : Task<Session> =
        task{
            let! sessionOption = x.GetSessionOptionFromContext ctx
            if sessionOption.IsNone
            then raise <| HttpException(401, "Not signed in.")
            return sessionOption.Value
        }