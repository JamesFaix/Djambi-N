[<AutoOpen>]
module Djambi.Api.Model.EventModel

open System
open Djambi.ClientGenerator.Annotations

[<ClientType(ClientSection.Events)>]
type GameStatusChangedEffect = 
    {
        oldValue : GameStatus
        newValue : GameStatus
    }

[<ClientType(ClientSection.Events)>]
type TurnCycleChangedEffect = 
    {
        oldValue : int list
        newValue : int list
    }
    
[<ClientType(ClientSection.Events)>]
type ParametersChangedEffect = 
    {
        oldValue : GameParameters
        newValue : GameParameters
    }

[<ClientType(ClientSection.Events)>]
type CurrentTurnChangedEffect = 
    {
        oldValue : Turn option
        newValue : Turn option
    }

[<ClientType(ClientSection.Events)>]
type PlayerEliminatedEffect =
    {
        playerId : int
    }

[<ClientType(ClientSection.Events)>]
type PieceKilledEffect =
    {
        pieceId : int
    }

[<ClientType(ClientSection.Events)>]
type PlayersRemovedEffect =
    {
        playerIds : int list
    }

[<ClientType(ClientSection.Events)>]
type PlayerOutOfMovesEffect =
    {
        playerId : int
    }

[<ClientType(ClientSection.Events)>]
type PlayerAddedEffect =
    {
        name : string option
        userId : int option
        kind : PlayerKind
    }

[<ClientType(ClientSection.Events)>]
type PiecesOwnershipChangedEffect =
    {
        oldPlayerId : int option
        newPlayerId : int option
        pieceIds : int list
    }

[<ClientType(ClientSection.Events)>]
type PieceMovedEffect =
    {
        oldCellId : int
        newCellId : int
        pieceId : int
    }

[<ClientType(ClientSection.Events)>]
type Effect =
    | GameStatusChanged of GameStatusChangedEffect
    | TurnCycleChanged of TurnCycleChangedEffect
    | ParametersChanged of ParametersChangedEffect
    //TODO: PlayerEliminated may need to change to PlayerStatusChanged as a DiffWithCOntext when statuses are added to players
    | PlayerEliminated of PlayerEliminatedEffect
    | PieceKilled of PieceKilledEffect
    | PlayersRemoved of PlayersRemovedEffect
    | PlayerOutOfMoves of PlayerOutOfMovesEffect
    | PlayerAdded of PlayerAddedEffect
    | PiecesOwnershipChanged of PiecesOwnershipChangedEffect
    | PieceMoved of PieceMovedEffect
    | CurrentTurnChanged of CurrentTurnChangedEffect

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
    }
    
module PlayerAddedEffect =
    let fromRequest (request : CreatePlayerRequest) : Effect =
        let f : PlayerAddedEffect = {
            name = request.name
            userId = request.userId
            kind = request.kind
        } 
        Effect.PlayerAdded f