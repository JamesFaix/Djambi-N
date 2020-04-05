namespace Apex.Api.Web.Controllers

open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open FSharp.Control.Tasks
open Serilog
open Apex.Api.Logic.Interfaces
open Apex.Api.Web
open Apex.Api.Web.Model
open Apex.Api.Web.Mappings

[<ApiController>]
[<Route("api/search")>]
type SearchController(manager : ISearchManager,
                       logger : ILogger,
                       scp : SessionContextProvider) =
    inherit ControllerBase()
    
    [<HttpPost("games")>]
    [<ProducesResponseType(200, Type = typeof<SearchGameDto[]>)>]
    member __.SearchGames([<FromBody>] query : GamesQueryDto) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let! session = scp.GetSessionFromContext ctx
            let query = query |> toGamesQuery
            let! games = manager.searchGames query session
            let dtos = games |> List.map toSearchGameDto
            return OkObjectResult(dtos) :> IActionResult
        }