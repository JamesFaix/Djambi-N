namespace Apex.Api.Web.Model

open System
open System.ComponentModel.DataAnnotations
open Apex.Api.Enums

type PlayerDto = {
    id : int
    gameId : int
    userId : Nullable<int>
    kind : PlayerKind

    [<Required>]
    name : string

    status : PlayerStatus
    colorId : Nullable<int>
    startingRegion : Nullable<int>
    startingTurnNumber : Nullable<int>
}

type PieceDto = {
    id : int
    kind : PieceKind
    playerId : Nullable<int>
    originalPlayerId : int
    cellId : int
}

type SelectionDto = {
    kind : SelectionKind
    cellId : int
    pieceId : Nullable<int>
}

// Turn needs to be a class so null can be used for instances
[<AllowNullLiteral>]
type TurnDto(status : TurnStatus,
            selections : List<SelectionDto>,
            selectionOptions : List<int>,
            requiredSelectionKind : Nullable<SelectionKind>) =
    member __.Status = status

    [<Required>]
    member __.Selections = selections

    [<Required>]
    member __.SelectionOptions = selectionOptions

    member __.RequiredSelectionKind = requiredSelectionKind

[<CLIMutable>]
type GameParametersDto = {
    // Nullable
    description : string
    
    regionCount : int
    
    isPublic : bool
    
    allowGuests : bool
}

type GameDto = {
    id : int

    [<Required>]
    createdBy : CreationSourceDto

    [<Required>]
    parameters : GameParametersDto

    status : GameStatus

    [<Required>]
    players : List<PlayerDto>

    // Nullable
    pieces : List<PieceDto>

    // Nullable
    turnCycle : List<int>

    // Nullable
    currentTurn : TurnDto
}