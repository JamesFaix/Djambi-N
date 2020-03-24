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
[<Route("api/users")>]
type UserController(manager : IUserManager,
                       logger : ILogger,
                       util : HttpUtility) =
    inherit ControllerBase()
    
    [<HttpPost>]
    [<ProducesResponseType(200, Type = typeof<User>)>]
    member __.CreateUser([<FromBody>] request : CreateUserRequest) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let! response =
                util.getSessionOptionFromContext ctx
                |> thenBindAsync (fun sessionOption ->
                    manager.createUser request sessionOption
                )
                |> thenExtract

            return OkObjectResult(response) :> IActionResult
        }
        
    [<HttpDelete("{userId}")>]
    [<ProducesResponseType(200)>]
    member __.ResetTurn(userId : int) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let! response =
                util.getSessionFromContext ctx
                |> thenBindAsync (fun session ->
                    manager.deleteUser userId session
                )
                //TODO: Log out if non-admin deleting self
                |> thenExtract

            return OkObjectResult(response) :> IActionResult
        }
    
    [<HttpGet("{userId}")>]
    [<ProducesResponseType(200, Type = typeof<User>)>]
    member __.GetUser(userId : int) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let! response =
                util.getSessionFromContext ctx
                |> thenBindAsync (fun session ->
                    manager.getUser userId session
                )
                |> thenExtract

            return OkObjectResult(response) :> IActionResult
        }
        
    [<HttpGet("current")>]
    [<ProducesResponseType(200, Type = typeof<User>)>]
    member __.GetCurrentUser() : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let! response =
                util.getSessionFromContext ctx
                |> thenBindAsync (fun session ->
                    manager.getCurrentUser session
                )
                |> thenExtract

            return OkObjectResult(response) :> IActionResult
        }