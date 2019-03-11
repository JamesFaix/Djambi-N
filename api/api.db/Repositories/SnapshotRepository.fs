module Djambi.Ap.Db.Repositories.SnapshotRepository

open Dapper
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Common.Json
open Djambi.Api.Db.Mapping
open Djambi.Api.Db.Model
open Djambi.Api.Db.SqlUtility
open Djambi.Api.Model

let getSnapshot (snapshotId : int) : Snapshot AsyncHttpResult =
    let param = DynamicParameters()
                    .add("SnapshotId", snapshotId)
                    .add("GameId", null)
    let cmd = proc("Snapshots_Get", param)

    querySingle<SnapshotSqlModel>(cmd, "Snapshot")
    |> thenMap mapSnapshotFromSql

let getSnapshotsForGame (gameId : int) : SnapshotInfo list AsyncHttpResult =
    let param = DynamicParameters()
                    .add("SnapshotId", null)
                    .add("GameId", gameId)
    let cmd = proc("Snapshots_Get", param)

    queryMany<SnapshotSqlModel>(cmd, "Snapshot")
    |> thenMap (List.map mapSnapshotInfoFromSql)

let deleteSnapshot (snapshotId : int) : Unit AsyncHttpResult =
    let param = DynamicParameters()
                    .add("SnapshotId", snapshotId)
    let cmd = proc("Snapshots_Delete", param)

    queryUnit(cmd, "Snapshot")

let createSnapshot (request : CreateSnapshotRequest) : int AsyncHttpResult =
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