namespace Apex.Api.Web.Mappings

open Apex.Api.Web.Model
open Apex.Api.Model

[<AutoOpen>]
module TurnMappings = 
    
    let toSelectionDto (source : Selection) : SelectionDto =
        {
            cellId = source.cellId
            kind = source.kind
            pieceId = source.pieceId |> Option.toNullable
        }

    let toTurnDto (source : Turn) : TurnDto =
        TurnDto(
            source.status,
            source.selections |> List.map toSelectionDto,
            source.selectionOptions,
            source.requiredSelectionKind |> Option.toNullable
        )