namespace Apex.Api.Db.Mappings

open Apex.Api.Db.Model
open Apex.Api.Model
open System
open Apex.Api.Common.Json

[<AutoOpen>]
module SnapshotMappings =

    let toSnapshotInfo (source : SnapshotSqlModel) : SnapshotInfo =
        {
            id = source.SnapshotId
            description = source.Description
            createdBy = {
                userId = source.CreatedByUser.UserId
                userName = source.CreatedByUser.Name
                time = source.CreatedOn
            }
        }

    let toSnapshot (source : SnapshotSqlModel) : Snapshot =
        let data = JsonUtility.deserialize<SnapshotJson> source.SnapshotJson
        {
            id = source.SnapshotId
            description = source.Description
            createdBy = {
                userId = source.CreatedByUser.UserId
                userName = source.CreatedByUser.Name
                time = source.CreatedOn
            }
            game = data.game
            history = data.history
        }