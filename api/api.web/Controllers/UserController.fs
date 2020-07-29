namespace Apex.Api.Web.Controllers

open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open FSharp.Control.Tasks
open Apex.Api.Logic.Interfaces
open Apex.Api.Web
open Apex.Api.Web.Mappings
open Apex.Api.Web.Model

[<ApiController>]
[<Route("api/users")>]
type UserController(manager : IUserManager,
                       scp : SessionContextProvider) =
    inherit ControllerBase()
    
    [<HttpPost>]
    [<ProducesResponseType(200, Type = typeof<UserDto>)>]
    member __.CreateUser([<FromBody>] request : CreateUserRequestDto) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let! sessionOption = scp.GetSessionOptionFromContext ctx
            let request = request |> toCreateUserRequest
            let! user = manager.createUser request sessionOption
            let dto = user |> toUserDto
            return OkObjectResult(dto) :> IActionResult
        }
        
    [<HttpDelete("{userId}")>]
    [<ProducesResponseType(200)>]
    member __.DeleteUser(userId : int) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let! session = scp.GetSessionFromContext ctx
            let! response = manager.deleteUser userId session
            //TODO: Log out if non-admin deleting self
            return OkObjectResult(response) :> IActionResult
        }
    
    [<HttpGet("{userId}")>]
    [<ProducesResponseType(200, Type = typeof<UserDto>)>]
    member __.GetUser(userId : int) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let! session = scp.GetSessionFromContext ctx
            let! user =  manager.getUser userId session
            let dto = user |> toUserDto
            return OkObjectResult(dto) :> IActionResult
        }
        
    [<HttpGet("current")>]
    [<ProducesResponseType(200, Type = typeof<UserDto>)>]
    member __.GetCurrentUser() : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let! session = scp.GetSessionFromContext ctx
            let! user = manager.getCurrentUser session
            let dto = user |> toUserDto
            return OkObjectResult(dto) :> IActionResult
        }