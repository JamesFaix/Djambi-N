namespace Djambi.Api.Web.Controllers

open Microsoft.AspNetCore.Http
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Model
open Djambi.Api.Web.Interfaces
open Djambi.Api.Web
open Djambi.Api.Logic.Interfaces

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
