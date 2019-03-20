namespace Djambi.Api.Db.Repositories

open Djambi.Api.Common.Collections
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Model
open Djambi.Api.Db.Repositories
open Djambi.Api.Db.SqlUtility
open Djambi.Api.Db
open Djambi.Api.Db.Interfaces

type SnapshotRepository(gameRepo : GameRepository) =
    interface ISnapshotRepository with
        member x.getSnapshot snapshotId =
            let cmd = Commands.getSnapshots (Some snapshotId, None)
            querySingle<SnapshotSqlModel>(cmd, "Snapshot")
            |> thenMap Mapping.mapSnapshotFromSql

        member x.getSnapshotsForGame gameId =
            let cmd = Commands.getSnapshots (None, Some gameId)
            queryMany<SnapshotSqlModel>(cmd, "Snapshot")
            |> thenMap (List.map Mapping.mapSnapshotInfoFromSql)

        member x.deleteSnapshot snapshotId =
            let cmd = Commands.deleteSnapshot snapshotId
            queryUnit(cmd, "Snapshot")

        member x.createSnapshot request =
            let cmd = Commands2.createSnapshot request
            querySingle<int>(cmd, "Snapshot")

        member x.loadSnapshot (gameId, snapshotId) =            
            (x :> ISnapshotRepository).getSnapshot snapshotId
            |> thenBindAsync (fun snapshot ->
                let commands = 
                    Commands2.replaceEventHistory (gameId, snapshot.history) ::
                    Commands2.updateGame snapshot.game ::
                    (snapshot.game.players |> List.map Commands2.updatePlayer)

                executeTransactionally commands "Snapshot"
            )