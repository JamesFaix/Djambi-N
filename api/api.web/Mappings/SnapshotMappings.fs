namespace Apex.Api.Web.Mappings

open Apex.Api.Model
open Apex.Api.Web.Model

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