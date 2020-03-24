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
        let result = SelectionDto()
        result.CellId <- source.cellId
        result.Kind <- source.kind |> toSelectionKindDto
        result.PieceId <- source.pieceId |> Option.toNullable
        result

    let toTurnStatusDto (source : TurnStatus) : TurnStatusDto =
        match source with
        | TurnStatus.AwaitingSelection -> TurnStatusDto.AwaitingSelection
        | TurnStatus.AwaitingCommit -> TurnStatusDto.AwaitingCommit
        | TurnStatus.DeadEnd -> TurnStatusDto.DeadEnd

    let toTurnDto (source : Turn) : TurnDto =
        let result = TurnDto()
        result.RequiresSelectionKind <- 
            source.requiredSelectionKind
            |> Option.map toSelectionKindDto
            |> Option.toNullable
        result.SelectionOptions <- 
            source.selectionOptions
            |> List.toArray
        result.Selections <- 
            source.selections
            |> List.map toSelectionDto
            |> List.toArray
        result.Status <- source.status |> toTurnStatusDto
        result
