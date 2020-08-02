namespace Apex.Api.Web.Model

open System
open System.ComponentModel.DataAnnotations
open Apex.Api.Enums

[<AbstractClass>]
type EffectDto(kind : EffectKind) =
    [<Required>]
    member __.Kind = kind

type ChangeEffectDto<'a>(kind : EffectKind,
                        oldValue : 'a,
                        newValue : 'a) =
    inherit EffectDto(kind)
    [<Required>]
    member __.OldValue = oldValue
    [<Required>]
    member __.NewValue = newValue

type PieceUpdateEffectDto(kind : EffectKind, oldPiece : PieceDto) =
    inherit EffectDto(kind)
    [<Required>]
    member __.OldPiece = oldPiece

type NeutralPlayerAddedEffectDto(name : string, placeholderPlayerId : int) =
    inherit EffectDto(EffectKind.NeutralPlayerAdded)
    [<Required>]
    member __.Name = name
    [<Required>]
    member __.PlaceholderPlayerId = placeholderPlayerId

type PieceEnlistedEffectDto(oldPiece : PieceDto, newPlayerId : int) =
    inherit PieceUpdateEffectDto(EffectKind.PieceEnlisted, oldPiece)
    [<Required>]
    member __.NewPlayerId = newPlayerId

type PieceMovedEffectDto(oldPiece : PieceDto, newCellId : int) =
    inherit PieceUpdateEffectDto(EffectKind.PieceMoved, oldPiece)
    [<Required>]
    member __.NewCellId = newCellId

type PieceVacatedEffectDto(oldPiece : PieceDto, newCellId : int) =
    inherit PieceUpdateEffectDto(EffectKind.PieceVacated, oldPiece)
    [<Required>]
    member __.NewCellId = newCellId

type PlayerAddedEffectDto(name : string, userId : int, playerKind : PlayerKind) =
    inherit EffectDto(EffectKind.PlayerAdded)
    [<Required>]
    member __.Name = name
    [<Required>]
    member __.UserId = userId
    [<Required>]
    member __.PlayerKind = playerKind

type PlayerOutOfMovesEffectDto(playerId : int) =
    inherit EffectDto(EffectKind.PlayerOutOfMoves)
    [<Required>]
    member __.PlayerId = playerId
    
type PlayerRemovedEffectDto(oldPlayer : PlayerDto) =
    inherit EffectDto(EffectKind.PlayerRemoved)
    [<Required>]
    member __.OldPlayer = oldPlayer

type PlayerStatusChangedEffectDto(oldValue : PlayerStatus, newValue : PlayerStatus, playerId : int) =
    inherit ChangeEffectDto<PlayerStatus>(EffectKind.PlayerStatusChanged, oldValue, newValue)
    [<Required>]
    member __.PlayerId = playerId

type TurnCyclePlayerFellFromPowerEffectDto(oldValue : List<int>, newValue : List<int>, playerId : int) =
    inherit ChangeEffectDto<List<int>>(EffectKind.TurnCyclePlayerFellFromPower, oldValue, newValue)
    [<Required>]
    member __.PlayerId = playerId

type TurnCyclePlayerRemovedEffectDto(oldValue : List<int>, newValue : List<int>, playerId : int) =
    inherit ChangeEffectDto<List<int>>(EffectKind.TurnCyclePlayerRemoved, oldValue, newValue)
    [<Required>]
    member __.PlayerId = playerId

type TurnCyclePlayerRoseToPowerEffectDto(oldValue : List<int>, newValue : List<int>, playerId : int) =
    inherit ChangeEffectDto<List<int>>(EffectKind.TurnCyclePlayerRoseToPower, oldValue, newValue)
    [<Required>]
    member __.PlayerId = playerId

type EventDto = {
    [<Required>]
    id : int

    [<Required>]
    createdBy : CreationSourceDto
    
    actingPlayerId : Nullable<int>
    
    [<Required>]
    kind : EventKind

    [<Required>]
    effects : List<EffectDto>
}

type StateAndEventResponseDto = {
    [<Required>]
    game : GameDto
    
    [<Required>]
    event : EventDto
}
