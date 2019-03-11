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
        |> thenBindAsync (fun session ->
            SnapshotManager.getSnapshotsForGame gameId session
        )
    handle func

let deleteSnapshot (gameId : int, snapshotId : int) : HttpHandler =
    let func ctx =
        getSessionFromContext ctx
        |> thenBindAsync (fun session ->
            SnapshotManager.deleteSnapshot gameId snapshotId session
        )
    handle func