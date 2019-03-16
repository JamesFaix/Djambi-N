namespace Djambi.Api.Logic.Managers

open Djambi.Api.Common.Control  
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Logic
open Djambi.Api.Model
open Djambi.Ap.Db.Repositories
open Djambi.Api.Db.Repositories
open Djambi.Api.Logic.Interfaces

type SnapshotManager() =
    interface ISnapshotManager with

        member x.createSnapshot gameId request session =
            Security.ensureHas Privilege.Snapshots session
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
    
        member x.getSnapshotsForGame gameId session =
            Security.ensureHas Privilege.Snapshots session
            |> Result.bindAsync (fun _ ->
                SnapshotRepository.getSnapshotsForGame gameId
            )
    
        member x.deleteSnapshot gameId snapshotId session =
            Security.ensureHas Privilege.Snapshots session
            |> Result.bindAsync (fun _ ->
                SnapshotRepository.deleteSnapshot snapshotId
            )

        member x.loadSnapshot gameId snapshotId session =
            Security.ensureHas Privilege.Snapshots session
            |> Result.bindAsync (fun _ ->
                SnapshotRepository.loadSnapshot (gameId, snapshotId)
            )