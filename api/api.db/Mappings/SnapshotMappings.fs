namespace Djambi.Api.Db.Mappings

open Djambi.Api.Db.Model
open Djambi.Api.Model
open Newtonsoft.Json

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
        let data = source.SnapshotJson |> JsonConvert.DeserializeObject<SnapshotJson>
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