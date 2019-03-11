module Djambi.Api.Logic.Managers.SnapshotManager

open Djambi.Api.Common.Control  
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Logic
open Djambi.Api.Logic.Services
open Djambi.Api.Model
open Djambi.ClientGenerator.Annotations
open Djambi.Ap.Db.Repositories
open Djambi.Api.Db.Repositories

[<ClientFunction(HttpMethod.Post, Routes.snapshots, ClientSection.Snapshots)>]
let createSnapshot (gameId : int) (request : CreateSnapshotRequest) (session : Session) : SnapshotInfo AsyncHttpResult =
    SecurityService.ensureHas Privilege.Snapshots session
    |> Result.bindAsync (fun _ ->
        GameRepository.getGame gameId
        |> thenBindAsync (fun game -> 
            EventRepository.getEvents (gameId, EventsQuery.empty)
            |> thenBindAsync (fun history -> 
                let request = 
                    {
                        game = game
                        history = history
                        description = request.description
                        createdByUserId = session.user.id
                    }
                SnapshotRepository.createSnapshot request
            )
        )
        |> thenBindAsync (fun snapshotId ->
            SnapshotRepository.getSnapshot snapshotId
            |> thenMap Snapshot.hideDetails
        )
    )
    
[<ClientFunction(HttpMethod.Get, Routes.snapshots, ClientSection.Snapshots)>]
let getSnapshotsForGame (gameId : int) (session : Session) : SnapshotInfo list AsyncHttpResult =
    SecurityService.ensureHas Privilege.Snapshots session
    |> Result.bindAsync (fun _ ->
        SnapshotRepository.getSnapshotsForGame gameId
    )
    
[<ClientFunction(HttpMethod.Delete, Routes.snapshot, ClientSection.Snapshots)>]
let deleteSnapshot (gameId : int) (snapshotId : int) (session : Session) : Unit AsyncHttpResult =
    SecurityService.ensureHas Privilege.Snapshots session
    |> Result.bindAsync (fun _ ->
        SnapshotRepository.deleteSnapshot snapshotId
    )

[<ClientFunction(HttpMethod.Post, Routes.snapshotLoad, ClientSection.Snapshots)>]
let loadSnapshot (gameId : int) (snapshotId : int) (session : Session) : Unit AsyncHttpResult =
    SecurityService.ensureHas Privilege.Snapshots session
    |> Result.bindAsync (fun _ ->
        SnapshotRepository.loadSnapshot (gameId, snapshotId)
    )