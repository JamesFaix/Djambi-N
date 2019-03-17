namespace Djambi.Api.Db.Repositories

open Dapper
open Djambi.Api.Common.Collections
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Common.Json
open Djambi.Api.Db.Mapping
open Djambi.Api.Db.Model
open Djambi.Api.Db.Repositories
open Djambi.Api.Db.SqlUtility
open Djambi.Api.Model
open Djambi.Api.Db
open Djambi.Api.Db.Interfaces

type SnapshotRepository(gameRepo : GameRepository) =
    interface ISnapshotRepository with
        member x.getSnapshot snapshotId =
            let param = DynamicParameters()
                            .add("SnapshotId", snapshotId)
                            .add("GameId", null)
            let cmd = proc("Snapshots_Get", param)

            querySingle<SnapshotSqlModel>(cmd, "Snapshot")
            |> thenMap mapSnapshotFromSql

        member x.getSnapshotsForGame gameId =
            let param = DynamicParameters()
                            .add("SnapshotId", null)
                            .add("GameId", gameId)
            let cmd = proc("Snapshots_Get", param)

            queryMany<SnapshotSqlModel>(cmd, "Snapshot")
            |> thenMap (List.map mapSnapshotInfoFromSql)

        member x.deleteSnapshot snapshotId =
            let param = DynamicParameters()
                            .add("SnapshotId", snapshotId)
            let cmd = proc("Snapshots_Delete", param)

            queryUnit(cmd, "Snapshot")

        member x.createSnapshot request =
            let jsonModel = 
                {
                    game = request.game
                    history = request.history
                }

            let json = JsonUtility.serialize jsonModel

            let param = DynamicParameters()
                            .add("GameId", request.game.id)
                            .add("CreatedByUserId", request.createdByUserId)
                            .add("Description", request.description)
                            .add("SnapshotJson", json)
            let cmd = proc("Snapshots_Create", param)

            querySingle<int>(cmd, "Snapshot")

        member x.loadSnapshot (gameId, snapshotId) =
            let getReplaceEventHistoryCommand (gameId : int, history : Event list) : CommandDefinition =  
                let param = DynamicParameters()
                                .add("GameId", gameId)
                                .add("History", TableValuedParameter.eventList history)
                proc("Snapshots_ReplaceEventHistory", param)

            (x :> ISnapshotRepository).getSnapshot snapshotId
            |> thenBindAsync (fun snapshot ->
                let commands = 
                    getReplaceEventHistoryCommand(gameId, snapshot.history) ::
                    gameRepo.getUpdateGameCommand snapshot.game ::
                    (snapshot.game.players |> List.map gameRepo.getUpdatePlayerCommand)

                executeTransactionally commands "Snapshot"
            )