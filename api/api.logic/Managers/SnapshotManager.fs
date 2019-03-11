module Djambi.Api.Logic.Managers.SnapshotManager

open Djambi.Api.Common.Control  
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Logic
open Djambi.Api.Logic.Services
open Djambi.Api.Model
open Djambi.ClientGenerator.Annotations
open Djambi.Ap.Db.Repositories
open Djambi.Api.Db.Repositories

[<ClientFunction(HttpMethod.Post, Routes.snapshots, ClientSection.Misc)>]
let createSnapshot (gameId : int) (request : CreateSnapshotRequest) (session : Session) : SnapshotInfo AsyncHttpResult =
    SecurityService.ensureHas Privilege.Snapshots session
    |> Result.bindAsync (fun _ ->
        GameRepository.getGame request.gameId
        |> thenBindAsync (fun game -> 
            EventRepository.getEvents (request.gameId, EventsQuery.empty)
            |> thenBindAsync (fun history -> 
                SnapshotRepository.createSnapshot request (game, history)
            )
        )
        |> thenBindAsync (fun snapshotId ->
            SnapshotRepository.getSnapshot snapshotId
            |> thenMap Snapshot.hideDetails
        )
    )
    
[<ClientFunction(HttpMethod.Get, Routes.snapshots, ClientSection.Misc)>]
let getSnapshotsForGame (gameId : int) (session : Session) : SnapshotInfo list AsyncHttpResult =
    SecurityService.ensureHas Privilege.Snapshots session
    |> Result.bindAsync (fun _ ->
        SnapshotRepository.getSnapshotsForGame gameId
    )
    
[<ClientFunction(HttpMethod.Delete, Routes.snapshot, ClientSection.Misc)>]
let deleteSnapshot (gameId : int) (snapshotId : int) (session : Session) : Unit AsyncHttpResult =
    SecurityService.ensureHas Privilege.Snapshots session
    |> Result.bindAsync (fun _ ->
        SnapshotRepository.deleteSnapshot snapshotId
    )