[<AutoOpen>]
module Apex.Api.Model.EventModel

type CurrentTurnChangedEffect =
    {
        oldValue : Turn option
        newValue : Turn option
    }

type GameStatusChangedEffect =
    {
        oldValue : GameStatus
        newValue : GameStatus
    }

type NeutralPlayerAddedEffect =
    {
        name : string
        placeholderPlayerId : int
    }

type ParametersChangedEffect =
    {
        oldValue : GameParameters
        newValue : GameParameters
    }

type PieceEnlistedEffect =
    {
        oldPiece : Piece
        newPlayerId : int
    }

type PieceAbandonedEffect =
    {
        oldPiece : Piece
    }

type PieceDroppedEffect =
    {
        oldPiece : Piece
        newPiece : Piece
    }

type PieceKilledEffect =
    {
        oldPiece : Piece
    }

type PieceMovedEffect =
    {
        oldPiece : Piece
        newCellId : int
    }

type PieceVacatedEffect =
    {
        oldPiece : Piece
        newCellId : int
    }

type PlayerAddedEffect =
    {
        name : string option
        userId : int
        kind : PlayerKind
    }

type PlayerOutOfMovesEffect =
    {
        playerId : int
    }

type PlayerRemovedEffect =
    {
        oldPlayer : Player
    }

type PlayerStatusChangedEffect =
    {
        playerId : int
        oldStatus : PlayerStatus
        newStatus : PlayerStatus
    }

type TurnCycleAdvancedEffect =
    {
        oldValue : int list
        newValue : int list
    }

type TurnCyclePlayerFellFromPowerEffect =
    {
        oldValue : int list
        newValue : int list
        playerId : int
    }

type TurnCyclePlayerRemovedEffect =
    {
        oldValue : int list
        newValue : int list
        playerId : int
    }

type TurnCyclePlayerRoseToPowerEffect =
    {
        oldValue : int list
        newValue : int list
        playerId : int
    }

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

type Event =
    {
        id : int
        createdBy : CreationSource
        actingPlayerId : int option
        kind : EventKind
        effects : Effect list
    }

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