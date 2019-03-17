namespace Djambi.Api.Web.Controllers

open Djambi.Api.Common
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Model
open Djambi.Api.Web.Interfaces
open Djambi.Api.Web
open Djambi.Api.Logic.Interfaces

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