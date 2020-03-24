namespace Apex.Api.Web.Controllers

open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open FSharp.Control.Tasks
open Serilog
open Apex.Api.Common.Control.AsyncHttpResult
open Apex.Api.Logic.Interfaces
open Apex.Api.Web
open Apex.Api.Web.Mappings
open Apex.Api.Web.Model

[<ApiController>]
[<Route("api/games/{gameId}/snapshots")>]
type SnapshotController(manager : ISnapshotManager,
                       logger : ILogger,
                       scp : SessionContextProvider) =
    inherit ControllerBase()
    
    [<HttpPost>]
    [<ProducesResponseType(200, Type = typeof<SnapshotInfoDto>)>]
    member __.CreateSnapshot(gameId : int, [<FromBody>] request : CreateSnapshotRequestDto) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let! session = scp.GetSessionFromContext ctx
            let request = request |> toCreateSnapshotRequest
            let! snapshot = manager.createSnapshot gameId request session |> thenExtract
            let dto = snapshot |> toSnapshotInfoDto
            return OkObjectResult(dto) :> IActionResult
        }
        
    [<HttpGet>]
    [<ProducesResponseType(200, Type = typeof<SnapshotInfoDto[]>)>]
    member __.GetSnapshots(gameId : int) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let! session = scp.GetSessionFromContext ctx
            let! snapshots = manager.getSnapshotsForGame gameId session |> thenExtract
            let dtos = snapshots |> toSnapshotInfoDtos
            return OkObjectResult(dtos) :> IActionResult
        }
    
    [<HttpDelete("{snapshotId}")>]
    [<ProducesResponseType(200)>]
    member __.DeleteSnapshot(gameId : int, snapshotId : int) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let! session = scp.GetSessionFromContext ctx
            let! _ = manager.deleteSnapshot gameId snapshotId session |> thenExtract
            return OkResult() :> IActionResult
        }
    
    [<HttpPost("{snapshotId}/load")>]
    [<ProducesResponseType(200)>]
    member __.LoadSnapshot(gameId : int, snapshotId : int) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let! session = scp.GetSessionFromContext ctx
            let! _ = manager.loadSnapshot gameId snapshotId session |> thenExtract
            return OkResult() :> IActionResult
        }