namespace Djambi.Api.Web.Mappings

open Djambi.Api.Web.Model
open Djambi.Api.Model

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