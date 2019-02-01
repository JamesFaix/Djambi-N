[<AutoOpen>]
module Djambi.Api.Model.EventModel

open System
open Djambi.ClientGenerator.Annotations

[<ClientType(ClientSection.Events)>]
type DiffEffect<'a> =
    {
        oldValue : 'a
        newValue : 'a
    }

[<ClientType(ClientSection.Events)>]
type ScalarEffect<'a> =
    {
        value : 'a
    }

[<ClientType(ClientSection.Events)>]
type DiffWithContextEffect<'a, 'b> =
    {
        oldValue : 'a
        newValue : 'a
        context : 'b
    }

[<ClientType(ClientSection.Events)>]
type Effect =
    | GameStatusChanged of DiffEffect<GameStatus>
    | TurnCycleChanged of DiffEffect<int list>
    | ParametersChanged of DiffEffect<GameParameters>
    //TODO: PlayerEliminated may need to change to PlayerStatusChanged as a DiffWithCOntext when statuses are added to players
    | PlayerEliminated of ScalarEffect<int>
    | PieceKilled of ScalarEffect<int>
    | PlayersRemoved of ScalarEffect<int list>
    | PlayerOutOfMoves of ScalarEffect<int>
    | PlayerAdded of ScalarEffect<CreatePlayerRequest>
    | PiecesOwnershipChanged of DiffWithContextEffect<int option, int list>
    | PieceMoved of DiffWithContextEffect<int, int>
    | CurrentTurnChanged of DiffEffect<Turn option>

module Effect =
    let gameStatusChanged (oldValue, newValue) =
        Effect.GameStatusChanged {
            oldValue = oldValue
            newValue = newValue
        }

    let turnCycleChanged (oldValue, newValue) =
        Effect.TurnCycleChanged {
            oldValue = oldValue
            newValue = newValue
        }

    let parametersChanged (oldValue, newValue) =
        Effect.ParametersChanged {
            oldValue = oldValue
            newValue = newValue
        }

    let playerEliminated (playerId) =
        Effect.PlayerEliminated {
            value = playerId
        }

    let playerAdded (request) =
        Effect.PlayerAdded {
            value = request
        }

    let playerOutOfMoves (playerId) =
        Effect.PlayerOutOfMoves {
            value = playerId
        }

    let pieceKilled (pieceId) =
        Effect.PieceKilled {
            value = pieceId
        }

    let playersRemoved (playerIds) =
        Effect.PlayersRemoved {
            value = playerIds
        }

    let piecesOwnershipChanged (pieceIds, oldPlayerId, newPlayerId) =
        Effect.PiecesOwnershipChanged {
            context = pieceIds
            oldValue = oldPlayerId
            newValue = newPlayerId
        }

    let pieceMoved (pieceId, oldCellId, newCellId) =
        Effect.PieceMoved {
            context = pieceId
            oldValue = oldCellId
            newValue = newCellId
        }

    let currentTurnChanged (oldTurn, newTurn) =
        Effect.CurrentTurnChanged {
            oldValue = oldTurn
            newValue = newTurn
        }

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