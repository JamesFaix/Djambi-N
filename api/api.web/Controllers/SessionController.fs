namespace Apex.Api.Web.Controllers

open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open FSharp.Control.Tasks
open Serilog
open Apex.Api.Common.Control.AsyncHttpResult
open Apex.Api.Logic.Interfaces
open Apex.Api.Model
open Apex.Api.Web

[<ApiController>]
[<Route("api/sessions")>]
type SessionController(manager : ISessionManager,
                       logger : ILogger,
                       scp : SessionContextProvider,
                       cookieProvider : CookieProvider) =
    inherit ControllerBase()
    
    [<HttpPost>]
    [<ProducesResponseType(200, Type = typeof<Session>)>]
    member __.OpenSession([<FromBody>] request : LoginRequest) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let! sessionOption = scp.GetSessionOptionFromContext ctx
            let! _ = match sessionOption with
                        | Some s -> manager.logout s
                        | None -> okTask ()

            let! session = manager.login request |> thenExtract

            cookieProvider.AppendCookie ctx (session.token, session.expiresOn)                               
            
            return OkObjectResult(session) :> IActionResult
        }
       
    [<HttpDelete>]
    [<ProducesResponseType(200)>]
    member __.CloseSession() : Task<IActionResult> =
        let ctx = base.HttpContext

        //Always clear the cookie, even if the DB does not have a session matching it
        cookieProvider.AppendEmptyCookie ctx

        task {
            let! sessionOption = scp.GetSessionOptionFromContext ctx
            let! _ = match sessionOption with
                        | Some s -> manager.logout s
                        | None -> okTask ()

            return OkResult() :> IActionResult
        }