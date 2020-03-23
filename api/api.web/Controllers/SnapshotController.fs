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
[<Route("games/{gameId}/snapshots")>]
type SnapshotController(manager : ISnapshotManager,
                       logger : ILogger,
                       util : HttpUtility) =
    inherit ControllerBase()
    
    [<HttpPost>]
    [<ProducesResponseType(200, Type = typeof<SnapshotInfo>)>]
    member __.CreateSnapshot(gameId : int, [<FromBody>] request : CreateSnapshotRequest) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let games =
                util.getSessionFromContext ctx
                |> thenBindAsync (fun session ->
                    manager.createSnapshot gameId request session
                )
                |> thenExtract

            return OkObjectResult(games) :> IActionResult
        }
        
    [<HttpGet>]
    [<ProducesResponseType(200, Type = typeof<SnapshotInfo[]>)>]
    member __.GetSnapshots(gameId : int) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let games =
                util.getSessionFromContext ctx
                |> thenBindAsync (fun session ->
                    manager.getSnapshotsForGame gameId session
                )
                |> thenExtract

            return OkObjectResult(games) :> IActionResult
        }
    
    [<HttpDelete("{snapshotId}")>]
    [<ProducesResponseType(200)>]
    member __.DeleteSnapshot(gameId : int, snapshotId : int) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let games =
                util.getSessionFromContext ctx
                |> thenBindAsync (fun session ->
                    manager.deleteSnapshot gameId snapshotId session
                )
                |> thenExtract

            return OkObjectResult(games) :> IActionResult
        }
    
    [<HttpPost("{snapshotId}/load")>]
    [<ProducesResponseType(200)>]
    member __.LoadSnapshot(gameId : int, snapshotId : int) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let games =
                util.getSessionFromContext ctx
                |> thenBindAsync (fun session ->
                    manager.loadSnapshot gameId snapshotId session
                )
                |> thenExtract

            return OkObjectResult(games) :> IActionResult
        }