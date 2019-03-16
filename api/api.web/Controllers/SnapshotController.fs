namespace Djambi.Api.Web.Controllers

open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Model
open Djambi.Api.Web.Interfaces
open Djambi.Api.Web
open Djambi.Api.Logic.Interfaces

type SnapshotController(u : HttpUtility,
                        snapshotMan : ISnapshotManager) =
    interface ISnapshotController with
        member x.createSnapshot gameId =
            let func ctx =
                u.getSessionAndModelFromContext<CreateSnapshotRequest> ctx
                |> thenBindAsync (fun (request, session) ->
                    snapshotMan.createSnapshot gameId request session
                )
            u.handle func

        member x.getSnapshotsForGame gameId =
            let func ctx =
                u.getSessionFromContext ctx
                |> thenBindAsync (snapshotMan.getSnapshotsForGame gameId)
            u.handle func

        member x.deleteSnapshot (gameId, snapshotId) =
            let func ctx =
                u.getSessionFromContext ctx
                |> thenBindAsync (snapshotMan.deleteSnapshot gameId snapshotId)
            u.handle func

        member x.loadSnapshot (gameId, snapshotId) =
            let func ctx =
                u.getSessionFromContext ctx 
                |> thenBindAsync (snapshotMan.loadSnapshot gameId snapshotId)
            u.handle func