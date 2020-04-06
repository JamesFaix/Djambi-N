namespace Apex.Api.Logic.Managers

open Apex.Api.Common.Control
open Apex.Api.Logic
open Apex.Api.Model
open Apex.Api.Logic.Interfaces
open Apex.Api.Db.Interfaces
open Apex.Api.Enums
open FSharp.Control.Tasks

type SnapshotManager(eventRepo : IEventRepository,
                     gameRepo : IGameRepository,
                     snapshotRepo : ISnapshotRepository) =
    interface ISnapshotManager with

        member x.createSnapshot gameId request session =
            Security.ensureHas Privilege.Snapshots session            
            if not <| Validation.isValidSnapshotDescription request.description
            then raise <| HttpException(422, "Snapshot descriptions cannot exceed 100 characters.")

            task {
                let! game = gameRepo.getGame gameId
                let! history = eventRepo.getEvents (gameId, EventsQuery.empty)
                let request =
                    {
                        game = game
                        history = history
                        description = request.description
                        createdByUserId = session.user.id
                    }
                let! snapshotId = snapshotRepo.createSnapshot request
                let! snapshot = snapshotRepo.getSnapshot snapshotId
                return snapshot |> Snapshot.hideDetails
            }

        member x.getSnapshotsForGame gameId session =
            Security.ensureHas Privilege.Snapshots session
            snapshotRepo.getSnapshotsForGame gameId
            
        member x.deleteSnapshot gameId snapshotId session =
            Security.ensureHas Privilege.Snapshots session
            snapshotRepo.deleteSnapshot snapshotId

        member x.loadSnapshot gameId snapshotId session =
            Security.ensureHas Privilege.Snapshots session
            snapshotRepo.loadSnapshot (gameId, snapshotId)