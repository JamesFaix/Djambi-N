namespace Apex.Api.Web.Mappings

open Apex.Api.Model
open Apex.Api.Web.Model

[<AutoOpen>]
module SnapshotMappings =

    let toSnapshotInfoDto (source : SnapshotInfo) : SnapshotInfoDto =
        let result = SnapshotInfoDto()
        result.CreatedBy <- source.createdBy |> toCreationSourceDto
        result.Description <- source.description 
        result.Id <- source.id
        result

    let toSnapshotInfoDtos (source : List<SnapshotInfo>) : SnapshotInfoDto[] =
        source
        |> List.map toSnapshotInfoDto
        |> List.toArray

    let toCreateSnapshotRequest (source : CreateSnapshotRequestDto) : CreateSnapshotRequest =
        {
            description = source.Description
        }