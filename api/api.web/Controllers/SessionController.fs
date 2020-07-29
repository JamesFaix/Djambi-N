namespace Apex.Api.Web.Controllers

open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open FSharp.Control.Tasks
open Serilog
open Apex.Api.Logic.Interfaces
open Apex.Api.Model
open Apex.Api.Web
open Apex.Api.Web.Model
open Apex.Api.Web.Mappings

[<ApiController>]
[<Route("api/sessions")>]
type SessionController(manager : ISessionManager,
                       logger : ILogger,
                       scp : SessionContextProvider,
                       cookieProvider : CookieProvider) =
    inherit ControllerBase()
    
    [<HttpPost>]
    [<ProducesResponseType(200, Type = typeof<SessionDto>)>]
    member __.OpenSession([<FromBody>] request : LoginRequestDto) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let! sessionOption = scp.GetSessionOptionFromContext ctx
            let! _ = match sessionOption with
                        | Some s -> manager.logout s
                        | None -> Task.FromResult ()

            let request = request |> toLoginRequest
            let! session = manager.login request
            let dto = session |> toSessionDto

            cookieProvider.AppendCookie ctx (session.token, session.expiresOn)                               
            
            return OkObjectResult(dto) :> IActionResult
        }
       
    [<HttpDelete>]
    [<ProducesResponseType(204)>]
    member __.CloseSession() : Task<IActionResult> =
        let ctx = base.HttpContext

        //Always clear the cookie, even if the DB does not have a session matching it
        cookieProvider.AppendEmptyCookie ctx

        task {
            let! sessionOption = scp.GetSessionOptionFromContext ctx
            let! _ = match sessionOption with
                        | Some s -> manager.logout s
                        | None -> Task.FromResult ()

            return NoContentResult() :> IActionResult
        }