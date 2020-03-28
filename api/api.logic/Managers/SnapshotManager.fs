namespace Apex.Api.Logic.Managers

open Apex.Api.Common.Control
open Apex.Api.Common.Control.AsyncHttpResult
open Apex.Api.Logic
open Apex.Api.Model
open Apex.Api.Logic.Interfaces
open Apex.Api.Db.Interfaces
open Apex.Api.Enums

type SnapshotManager(eventRepo : IEventRepository,
                     gameRepo : IGameRepository,
                     snapshotRepo : ISnapshotRepository) =
    interface ISnapshotManager with

        member x.createSnapshot gameId request session =
            Security.ensureHas Privilege.Snapshots session
            |> Result.bind (fun _ -> 
                if not <| Validation.isValidSnapshotDescription request.description
                then Error <| HttpException(422, "Snapshot descriptions cannot exceed 100 characters.")
                else Ok ()
            )
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