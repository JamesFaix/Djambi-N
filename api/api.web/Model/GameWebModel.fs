namespace Djambi.Api.Web.Model

open System
open System.ComponentModel.DataAnnotations
open Djambi.Api.Enums

type PlayerDto = {
    [<Required>]
    id : int
    
    [<Required>]
    gameId : int
    
    userId : Nullable<int>
    
    [<Required>]
    kind : PlayerKind

    [<Required>]
    name : string

    [<Required>]
    status : PlayerStatus
    
    colorId : Nullable<int>
    startingRegion : Nullable<int>
    startingTurnNumber : Nullable<int>
}

type PieceDto = {
    [<Required>]
    id : int
    
    [<Required>]
    kind : PieceKind
    
    playerId : Nullable<int>
    
    [<Required>]
    originalPlayerId : int
    
    [<Required>]
    cellId : int
}

type SelectionDto = {
    [<Required>]
    kind : SelectionKind
    
    [<Required>]
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
    [<StringLength(100)>]
    description : string
    
    [<Required>]
    regionCount : int
    
    [<Required>]
    isPublic : bool
    
    [<Required>]
    allowGuests : bool
}

type GameDto = {
    [<Required>]
    id : int

    [<Required>]
    createdBy : CreationSourceDto

    [<Required>]
    parameters : GameParametersDto

    [<Required>]
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