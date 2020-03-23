namespace Apex.Api.Web.Controllers2

open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open FSharp.Control.Tasks
open Serilog
open Apex.Api.Common.Control.AsyncHttpResult
open Apex.Api.Logic.Interfaces
open Apex.Api.Model
open Apex.Api.Web

[<ApiController>]
[<Route("search")>]
type SearchController(manager : ISearchManager,
                       logger : ILogger,
                       util : HttpUtility) =
    inherit ControllerBase()
    
    [<HttpPost("games")>]
    [<ProducesResponseType(200, Type = typeof<SearchGame[]>)>]
    member __.SearchGames([<FromBody>] query : GamesQuery) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let games =
                util.getSessionFromContext ctx
                |> thenBindAsync (fun session ->
                    manager.searchGames query session
                )
                |> thenExtract

            return OkObjectResult(games) :> IActionResult
        }