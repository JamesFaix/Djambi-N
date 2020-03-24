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
[<Route("api/search")>]
type SearchController(manager : ISearchManager,
                       logger : ILogger,
                       scp : SessionContextProvider) =
    inherit ControllerBase()
    
    [<HttpPost("games")>]
    [<ProducesResponseType(200, Type = typeof<SearchGame[]>)>]
    member __.SearchGames([<FromBody>] query : GamesQuery) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let! session = scp.GetSessionFromContext ctx
            let! games = manager.searchGames query session |> thenExtract
            return OkObjectResult(games) :> IActionResult
        }