namespace Apex.Api.Web.Controllers

open Microsoft.AspNetCore.Http
open Apex.Api.Common.Control.AsyncHttpResult
open Apex.Api.Model
open Apex.Api.Web.Interfaces
open Apex.Api.Web
open Apex.Api.Logic.Interfaces

type SessionController(u : HttpUtility,
                       sessionMan : ISessionManager) =
    interface ISessionController with

        member x.openSession =
            let func (ctx : HttpContext) =
                u.getSessionOptionAndModelFromContext<LoginRequest> ctx
                |> thenBindAsync (fun (request, session) ->
                    match session with
                    | Some s -> sessionMan.logout s
                    | None -> okTask ()

                    |> thenBindAsync (fun _ -> sessionMan.login request)
                    |> thenDo (fun session -> u.appendCookie ctx (session.token, session.expiresOn))
                )
            u.handle func

        member x.closeSession =
            let func (ctx : HttpContext) =
                //Always clear the cookie, even if the DB does not have a session matching it
                u.appendEmptyCookie ctx

                u.getSessionFromContext ctx
                |> thenBindAsync sessionMan.logout

            u.handle func
