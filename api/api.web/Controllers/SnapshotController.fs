module Djambi.Api.Web.Controllers.SnapshotController

open Giraffe
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Web.HttpUtility
open Djambi.Api.Logic.Managers
open Djambi.Api.Model

let createSnapshot (gameId : int) : HttpHandler =
    let func ctx =
        getSessionAndModelFromContext<CreateSnapshotRequest> ctx
        |> thenBindAsync (fun (request, session) ->
            SnapshotManager.createSnapshot gameId request session
        )
    handle func

let getSnapshotsForGame (gameId : int) : HttpHandler =
    let func ctx =
        getSessionFromContext ctx
        |> thenBindAsync (SnapshotManager.getSnapshotsForGame gameId)
    handle func

let deleteSnapshot (gameId : int, snapshotId : int) : HttpHandler =
    let func ctx =
        getSessionFromContext ctx
        |> thenBindAsync (SnapshotManager.deleteSnapshot gameId snapshotId)
    handle func

let loadSnapshot (gameId : int, snapshotId : int) : HttpHandler =
    let func ctx =
        getSessionFromContext ctx 
        |> thenBindAsync (SnapshotManager.loadSnapshot gameId snapshotId)
    handle func