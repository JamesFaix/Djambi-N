namespace Apex.Api.Web.Mappings

open Apex.Api.Web.Model
open Apex.Api.Model

[<AutoOpen>]
module TurnMappings = 
    
    let toSelectionKindDto (source : SelectionKind) : SelectionKindDto =
        match source with
        | SelectionKind.Drop -> SelectionKindDto.Drop
        | SelectionKind.Move -> SelectionKindDto.Move
        | SelectionKind.Subject -> SelectionKindDto.Subject
        | SelectionKind.Target -> SelectionKindDto.Target
        | SelectionKind.Vacate -> SelectionKindDto.Vacate

    let toSelectionDto (source : Selection) : SelectionDto =
        {
            cellId = source.cellId
            kind = source.kind |> toSelectionKindDto
            pieceId = source.pieceId |> Option.toNullable
        }

    let toTurnStatusDto (source : TurnStatus) : TurnStatusDto =
        match source with
        | TurnStatus.AwaitingSelection -> TurnStatusDto.AwaitingSelection
        | TurnStatus.AwaitingCommit -> TurnStatusDto.AwaitingCommit
        | TurnStatus.DeadEnd -> TurnStatusDto.DeadEnd

    let toTurnDto (source : Turn) : TurnDto =
        TurnDto(
            source.status |> toTurnStatusDto,
            source.selections |> List.map toSelectionDto,
            source.selectionOptions,
            source.requiredSelectionKind
                |> Option.map toSelectionKindDto
                |> Option.toNullable
        )