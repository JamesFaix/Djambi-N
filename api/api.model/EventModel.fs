[<AutoOpen>]
module Apex.Api.Model.EventModel

open Apex.ClientGenerator.Annotations

[<ClientType(ClientSection.Events)>]
type CurrentTurnChangedEffect =
    {
        oldValue : Turn option
        newValue : Turn option
    }

[<ClientType(ClientSection.Events)>]
type GameStatusChangedEffect =
    {
        oldValue : GameStatus
        newValue : GameStatus
    }

[<ClientType(ClientSection.Events)>]
type NeutralPlayerAddedEffect =
    {
        name : string
        placeholderPlayerId : int
    }

[<ClientType(ClientSection.Events)>]
type ParametersChangedEffect =
    {
        oldValue : GameParameters
        newValue : GameParameters
    }

[<ClientType(ClientSection.Events)>]
type PieceEnlistedEffect =
    {
        oldPiece : Piece
        newPlayerId : int
    }

[<ClientType(ClientSection.Events)>]
type PieceAbandonedEffect =
    {
        oldPiece : Piece
    }

[<ClientType(ClientSection.Events)>]
type PieceDroppedEffect =
    {
        oldPiece : Piece
        newPiece : Piece
    }

[<ClientType(ClientSection.Events)>]
type PieceKilledEffect =
    {
        oldPiece : Piece
    }

[<ClientType(ClientSection.Events)>]
type PieceMovedEffect =
    {
        oldPiece : Piece
        newCellId : int
    }

[<ClientType(ClientSection.Events)>]
type PieceVacatedEffect =
    {
        oldPiece : Piece
        newCellId : int
    }

[<ClientType(ClientSection.Events)>]
type PlayerAddedEffect =
    {
        name : string option
        userId : int
        kind : PlayerKind
    }

[<ClientType(ClientSection.Events)>]
type PlayerOutOfMovesEffect =
    {
        playerId : int
    }

[<ClientType(ClientSection.Events)>]
type PlayerRemovedEffect =
    {
        oldPlayer : Player
    }

[<ClientType(ClientSection.Events)>]
type PlayerStatusChangedEffect =
    {
        playerId : int
        oldStatus : PlayerStatus
        newStatus : PlayerStatus
    }

[<ClientType(ClientSection.Events)>]
type TurnCycleAdvancedEffect =
    {
        oldValue : int list
        newValue : int list
    }

[<ClientType(ClientSection.Events)>]
type TurnCyclePlayerFellFromPowerEffect =
    {
        oldValue : int list
        newValue : int list
        playerId : int
    }

[<ClientType(ClientSection.Events)>]
type TurnCyclePlayerRemovedEffect =
    {
        oldValue : int list
        newValue : int list
        playerId : int
    }

[<ClientType(ClientSection.Events)>]
type TurnCyclePlayerRoseToPowerEffect =
    {
        oldValue : int list
        newValue : int list
        playerId : int
    }

[<ClientType(ClientSection.Events)>]
type Effect =
    | CurrentTurnChanged of CurrentTurnChangedEffect
    | GameStatusChanged of GameStatusChangedEffect
    | NeutralPlayerAdded of NeutralPlayerAddedEffect
    | ParametersChanged of ParametersChangedEffect
    | PieceAbandoned of PieceAbandonedEffect
    | PieceDropped of PieceDroppedEffect
    | PieceEnlisted of PieceEnlistedEffect
    | PieceKilled of PieceKilledEffect
    | PieceMoved of PieceMovedEffect
    | PieceVacated of PieceVacatedEffect
    | PlayerAdded of PlayerAddedEffect
    | PlayerOutOfMoves of PlayerOutOfMovesEffect
    | PlayerRemoved of PlayerRemovedEffect
    | PlayerStatusChanged of PlayerStatusChangedEffect
    | TurnCycleAdvanced of TurnCycleAdvancedEffect
    | TurnCyclePlayerFellFromPower of TurnCyclePlayerFellFromPowerEffect
    | TurnCyclePlayerRemoved of TurnCyclePlayerRemovedEffect
    | TurnCyclePlayerRoseToPower of TurnCyclePlayerRoseToPowerEffect

[<ClientType(ClientSection.Events)>]
type EventKind =
    | GameParametersChanged
    | GameCanceled
    | PlayerJoined
    | PlayerRemoved
    | GameStarted
    | TurnCommitted
    | TurnReset
    | CellSelected
    | PlayerStatusChanged

[<ClientType(ClientSection.Events)>]
type Event =
    {
        id : int
        createdBy : CreationSource
        actingPlayerId : int option
        kind : EventKind
        effects : Effect list
    }

[<ClientType(ClientSection.Events)>]
type StateAndEventResponse =
    {
        game : Game
        event : Event
    }

//Internal request
type CreateEventRequest =
    {
        kind : EventKind
        effects : Effect list
        createdByUserId : int
        actingPlayerId : int option
    }

module PlayerAddedEffect =
    let fromRequest (request : CreatePlayerRequest) : Effect =
        let f : PlayerAddedEffect = {
            name = request.name
            userId = request.userId.Value
            kind = request.kind
        }
        Effect.PlayerAdded f