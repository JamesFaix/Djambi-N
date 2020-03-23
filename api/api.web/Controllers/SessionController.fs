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
[<Route("sessions")>]
type SessionController(manager : ISessionManager,
                       logger : ILogger,
                       util : HttpUtility) =
    inherit ControllerBase()
    
    [<HttpPost>]
    [<ProducesResponseType(200, Type = typeof<Session>)>]
    member __.OpenSession([<FromBody>] request : LoginRequest) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let games =
                util.getSessionOptionFromContext ctx
                |> thenBindAsync (fun session ->
                    match session with
                    | Some s -> manager.logout s
                    | None -> okTask ()

                    |> thenBindAsync (fun _ -> manager.login request)
                    |> thenDo (fun session -> util.appendCookie ctx (session.token, session.expiresOn))                                  
                )
                |> thenExtract

            return OkObjectResult(games) :> IActionResult
        }
       
    [<HttpDelete>]
    [<ProducesResponseType(200)>]
    member __.CloseSession() : Task<IActionResult> =
        let ctx = base.HttpContext

        //Always clear the cookie, even if the DB does not have a session matching it
        util.appendEmptyCookie ctx

        task {
            let games =
                util.getSessionFromContext ctx
                |> thenBindAsync manager.logout 
                |> thenExtract

            return OkObjectResult(games) :> IActionResult
        }