namespace Djambi.Api.Web.Mappings

open Djambi.Api.Model
open Djambi.Api.Web.Model

[<AutoOpen>]
module SnapshotMappings =

    let toSnapshotInfoDto (source : SnapshotInfo) : SnapshotInfoDto =
        {
            id = source.id
            createdBy = source.createdBy |> toCreationSourceDto
            description = source.description
        }

    let toCreateSnapshotRequest (source : CreateSnapshotRequestDto) : CreateSnapshotRequest =
        {
            description = source.description
        }