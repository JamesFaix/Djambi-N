namespace Apex.Api.Web.Controllers

open Apex.Api.Common
open Apex.Api.Common.Control
open Apex.Api.Common.Control.AsyncHttpResult
open Apex.Api.Model
open Apex.Api.Web.Interfaces
open Apex.Api.Web
open Apex.Api.Logic.Interfaces

type PlayerController(u : HttpUtility,
                      playerMan : IPlayerManager) =
    interface IPlayerController with
        member x.addPlayer gameId =
            let func ctx =
                u.getSessionAndModelFromContext<CreatePlayerRequest> ctx
                |> thenBindAsync (fun (request, session) -> playerMan.addPlayer gameId request session)
            u.handle func

        member x.removePlayer (gameId, playerId) =
            let func ctx =
                u.getSessionFromContext ctx
                |> thenBindAsync (playerMan.removePlayer(gameId, playerId))
            u.handle func

        member x.updatePlayerStatus (gameId, playerId, statusName) =
            let func ctx =
                Enum.parseUnion<PlayerStatus> statusName
                |> Result.bindAsync (fun status ->
                    u.getSessionFromContext ctx
                    |> thenBindAsync (playerMan.updatePlayerStatus (gameId, playerId, status))
                )
            u.handle func