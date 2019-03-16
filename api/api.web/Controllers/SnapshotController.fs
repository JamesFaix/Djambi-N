namespace Djambi.Api.Web.Controllers

open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Logic.Managers
open Djambi.Api.Model
open Djambi.Api.Web.Interfaces
open Djambi.Api.Web

type SnapshotController(u : HttpUtility) =
    interface ISnapshotController with
        member x.createSnapshot gameId =
            let func ctx =
                u.getSessionAndModelFromContext<CreateSnapshotRequest> ctx
                |> thenBindAsync (fun (request, session) ->
                    SnapshotManager.createSnapshot gameId request session
                )
            u.handle func

        member x.getSnapshotsForGame gameId =
            let func ctx =
                u.getSessionFromContext ctx
                |> thenBindAsync (SnapshotManager.getSnapshotsForGame gameId)
            u.handle func

        member x.deleteSnapshot (gameId, snapshotId) =
            let func ctx =
                u.getSessionFromContext ctx
                |> thenBindAsync (SnapshotManager.deleteSnapshot gameId snapshotId)
            u.handle func

        member x.loadSnapshot (gameId, snapshotId) =
            let func ctx =
                u.getSessionFromContext ctx 
                |> thenBindAsync (SnapshotManager.loadSnapshot gameId snapshotId)
            u.handle func