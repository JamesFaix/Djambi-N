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
open Apex.Api.Enums

[<ApiController>]
[<Route("api/games/{gameId}/players")>]
type PlayerController(manager : IPlayerManager,
                       logger : ILogger,
                       scp : SessionContextProvider) =
    inherit ControllerBase()
    
    [<HttpPost>]
    [<ProducesResponseType(200, Type = typeof<StateAndEventResponseDto>)>]
    member __.AddPlayer(gameId : int, [<FromBody>] request : CreatePlayerRequestDto) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let! session = scp.GetSessionFromContext ctx
            let request = request |> toCreatePlayerRequest
            let! response = manager.addPlayer gameId request session |> thenExtract
            let dto = response |> toStateAndEventResponseDto
            return OkObjectResult(dto) :> IActionResult
        }

    [<HttpDelete("{playerId}")>]
    [<ProducesResponseType(200, Type = typeof<StateAndEventResponseDto>)>]
    member __.RemovePlayer(gameId : int, playerId : int) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let! session = scp.GetSessionFromContext ctx
            let! response = manager.removePlayer (gameId, playerId) session |> thenExtract
            let dto = response |> toStateAndEventResponseDto
            return OkObjectResult(dto) :> IActionResult
        }
    
    [<HttpPut("{playerId}/status/{status}")>]
    [<ProducesResponseType(200, Type = typeof<StateAndEventResponseDto>)>]
    member __.SetPlayerStatus(gameId : int, playerId : int, status : PlayerStatus) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let! session = scp.GetSessionFromContext ctx
            let! response = manager.updatePlayerStatus (gameId, playerId, status) session |> thenExtract                
            let dto = response |> toStateAndEventResponseDto
            return OkObjectResult(dto) :> IActionResult
        }
