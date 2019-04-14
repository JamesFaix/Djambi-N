namespace Djambi.Api.Logic.Managers

open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Logic
open Djambi.Api.Model
open Djambi.Api.Logic.Interfaces
open Djambi.Api.Db.Interfaces

type SnapshotManager(eventRepo : IEventRepository,
                     gameRepo : IGameRepository,
                     snapshotRepo : ISnapshotRepository) =
    interface ISnapshotManager with

        member x.createSnapshot gameId request session =
            Security.ensureHas Privilege.Snapshots session
            |> Result.bindAsync (fun _ ->
                gameRepo.getGame gameId
                |> thenBindAsync (fun game ->
                    eventRepo.getEvents (gameId, EventsQuery.empty)
                    |> thenBindAsync (fun history ->
                        let request =
                            {
                                game = game
                                history = history
                                description = request.description
                                createdByUserId = session.user.id
                            }
                        snapshotRepo.createSnapshot request
                    )
                )
                |> thenBindAsync (fun snapshotId ->
                    snapshotRepo.getSnapshot snapshotId
                    |> thenMap Snapshot.hideDetails
                )
            )

        member x.getSnapshotsForGame gameId session =
            Security.ensureHas Privilege.Snapshots session
            |> Result.bindAsync (fun _ ->
                snapshotRepo.getSnapshotsForGame gameId
            )

        member x.deleteSnapshot gameId snapshotId session =
            Security.ensureHas Privilege.Snapshots session
            |> Result.bindAsync (fun _ ->
                snapshotRepo.deleteSnapshot snapshotId
            )

        member x.loadSnapshot gameId snapshotId session =
            Security.ensureHas Privilege.Snapshots session
            |> Result.bindAsync (fun _ ->
                snapshotRepo.loadSnapshot (gameId, snapshotId)
            )