namespace Apex.Api.Web.Model

open System
open System.ComponentModel.DataAnnotations

type EventKindDto =
   | GameParametersChanged = 1
   | GameCanceled = 2
   | PlayerJoined = 3
   | PlayerRemoved = 4
   | GameStarted = 5
   | TurnCommitted = 6
   | TurnReset = 7
   | CellSelected = 8
   | PlayerStatusChanged = 9 

type EffectKindDto =
    | CurrentTurnChanged = 1
    | GameStatusChanged = 2
    | NeutralPlayerAdded = 3
    | ParametersChanged = 4
    | PieceAbandoned = 5
    | PieceDropped = 6
    | PieceEnlisted = 7
    | PieceKilled = 8
    | PieceMoved = 9
    | PieceVacated = 10
    | PlayerAdded = 11
    | PlayerOutOfMoves = 12
    | PlayerRemoved = 13
    | PlayerStatusChanged = 14
    | TurnCycleAdvanced = 15
    | TurnCyclePlayerFellFromPower = 16
    | TurnCyclePlayerRemoved = 17
    | TurnCyclePlayerRoseToPower = 18

[<AbstractClass>]
type EffectDto(kind : EffectKindDto) =
    member __.Kind = kind

type ChangeEffectDto<'a>(kind : EffectKindDto,
                        oldValue : 'a,
                        newValue : 'a) =
    inherit EffectDto(kind)
    member __.OldValue = oldValue
    member __.NewValue = newValue

type PieceUpdateEffectDto(kind : EffectKindDto, oldPiece : PieceDto) =
    inherit EffectDto(kind)
    member __.OldPiece = oldPiece

type NeutralPlayerAddedEffectDto(name : string, placeholderPlayerId : int) =
    inherit EffectDto(EffectKindDto.NeutralPlayerAdded)
    member __.Name = name
    member __.PlaceholderPlayerId = placeholderPlayerId

type PieceEnlistedEffectDto(oldPiece : PieceDto, newPlayerId : int) =
    inherit PieceUpdateEffectDto(EffectKindDto.PieceEnlisted, oldPiece)
    member __.NewPlayerId = newPlayerId

type PieceMovedEffectDto(oldPiece : PieceDto, newCellId : int) =
    inherit PieceUpdateEffectDto(EffectKindDto.PieceMoved, oldPiece)
    member __.NewCellId = newCellId

type PieceVacatedEffectDto(oldPiece : PieceDto, newCellId : int) =
    inherit PieceUpdateEffectDto(EffectKindDto.PieceVacated, oldPiece)
    member __.NewCellId = newCellId

type PlayerAddedEffectDto(name : string, userId : int, playerKind : PlayerKindDto) =
    inherit EffectDto(EffectKindDto.PlayerAdded)
    member __.Name = name
    member __.UserId = userId
    member __.PlayerKind = playerKind

type PlayerOutOfMovesEffectDto(playerId : int) =
    inherit EffectDto(EffectKindDto.PlayerOutOfMoves)
    member __.PlayerId = playerId
    
type PlayerRemovedEffectDto(oldPlayer : PlayerDto) =
    inherit EffectDto(EffectKindDto.PlayerRemoved)
    member __.OldPlayer = oldPlayer

type PlayerStatusChangedEffectDto(oldValue : PlayerStatusDto, newValue : PlayerStatusDto, playerId : int) =
    inherit ChangeEffectDto<PlayerStatusDto>(EffectKindDto.PlayerStatusChanged, oldValue, newValue)
    member __.PlayerId = playerId

type TurnCyclePlayerFellFromPowerEffectDto(oldValue : List<int>, newValue : List<int>, playerId : int) =
    inherit ChangeEffectDto<List<int>>(EffectKindDto.TurnCyclePlayerFellFromPower, oldValue, newValue)
    member __.PlayerId = playerId

type TurnCyclePlayerRemovedEffectDto(oldValue : List<int>, newValue : List<int>, playerId : int) =
    inherit ChangeEffectDto<List<int>>(EffectKindDto.TurnCyclePlayerRemoved, oldValue, newValue)
    member __.PlayerId = playerId

type TurnCyclePlayerRoseToPowerEffectDto(oldValue : List<int>, newValue : List<int>, playerId : int) =
    inherit ChangeEffectDto<List<int>>(EffectKindDto.TurnCyclePlayerRoseToPower, oldValue, newValue)
    member __.PlayerId = playerId

type EventDto = {
    id : int

    [<Required>]
    createdBy : CreationSourceDto
    actingPlayerId : Nullable<int>
    kind : EventKindDto

    [<Required>]
    effects : List<EffectDto>
}

type StateAndEventResponseDto = {
    [<Required>]
    game : GameDto
    
    [<Required>]
    event : EventDto
}
