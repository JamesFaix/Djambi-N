[<AutoOpen>]
module Djambi.Api.Model.EventModel

open System
open Djambi.ClientGenerator.Annotations

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
        newPlayerId : int option
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
        newCellId : int
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
type PlayerEliminatedEffect =
    {
        playerId : int
    }

[<ClientType(ClientSection.Events)>]
type PlayerOutOfMovesEffect =
    {
        playerId : int
    }

[<ClientType(ClientSection.Events)>]
type PlayerRemovedEffect =
    {
        playerId : int
    }

[<ClientType(ClientSection.Events)>]
type TurnCycleChangedEffect = 
    {
        oldValue : int list
        newValue : int list
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
    | PlayerEliminated of PlayerEliminatedEffect
    | PlayerOutOfMoves of PlayerOutOfMovesEffect
    | PlayerRemoved of PlayerRemovedEffect
    | TurnCycleChanged of TurnCycleChangedEffect

[<ClientType(ClientSection.Events)>]
type EventKind =
    | GameParametersChanged
    | GameCanceled
    | PlayerJoined
    | PlayerEjected
    | PlayerQuit
    | GameStarted
    | TurnCommitted
    | TurnReset
    | CellSelected

[<ClientType(ClientSection.Events)>]
type Event =
    {
        id : int
        createdByUserId : int
        createdOn : DateTime
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
        if request.kind = PlayerKind.Neutral then
            let f : NeutralPlayerAddedEffect = {
                name = request.name.Value
            }
            Effect.NeutralPlayerAdded f
        else
            let f : PlayerAddedEffect = {
                name = request.name
                userId = request.userId.Value
                kind = request.kind
            } 
            Effect.PlayerAdded f