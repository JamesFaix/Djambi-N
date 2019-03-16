namespace Djambi.Api.Web.Controllers

open Giraffe
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Web.HttpUtility
open Djambi.Api.Logic.Managers
open Djambi.Api.Model
open Djambi.Api.Web.Interfaces

type SnapshotController() =
    interface ISnapshotController with
        member x.createSnapshot (gameId : int) : HttpHandler =
            let func ctx =
                getSessionAndModelFromContext<CreateSnapshotRequest> ctx
                |> thenBindAsync (fun (request, session) ->
                    SnapshotManager.createSnapshot gameId request session
                )
            handle func

        member x.getSnapshotsForGame gameId =
            let func ctx =
                getSessionFromContext ctx
                |> thenBindAsync (SnapshotManager.getSnapshotsForGame gameId)
            handle func

        member x.deleteSnapshot (gameId, snapshotId) =
            let func ctx =
                getSessionFromContext ctx
                |> thenBindAsync (SnapshotManager.deleteSnapshot gameId snapshotId)
            handle func

        member x.loadSnapshot (gameId, snapshotId) =
            let func ctx =
                getSessionFromContext ctx 
                |> thenBindAsync (SnapshotManager.loadSnapshot gameId snapshotId)
            handle func