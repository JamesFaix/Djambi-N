[<AutoOpen>]
module Djambi.Api.Model.EventModel

open System

type EffectKind =
    | GameCreated
    | GameStatusChanged
    | PlayerEliminated
    | PiecesOwnershipChanged
    | PieceMoved
    | PieceKilled
    | TurnCycleChanged
    | PlayerOutOfMoves
    | PlayerAdded
    | PlayersRemoved
    | ParametersChanged

type DiffEffect<'a> =
    {
        kind : EffectKind
        oldValue : 'a
        newValue : 'a
    }

type ScalarEffect<'a> =
    {
        kind : EffectKind
        value : 'a
    }

type DiffWithContextEffect<'a, 'b> =
    {
        kind : EffectKind
        oldValue : 'a
        newValue : 'a
        context : 'b
    }

type EventEffect =
    | GameCreated of ScalarEffect<CreateGameRequest>
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

module EventEffect =
    let gameCreated (request) =
        EventEffect.GameCreated {
            kind = EffectKind.GameCreated
            value = request
        }

    let gameStatusChanged (oldValue, newValue) =
        EventEffect.GameStatusChanged {
            kind = EffectKind.GameStatusChanged
            oldValue = oldValue
            newValue = newValue
        }

    let turnCycleChanged (oldValue, newValue) =
        EventEffect.TurnCycleChanged {
            kind = EffectKind.TurnCycleChanged
            oldValue = oldValue
            newValue = newValue
        }

    let parametersChanged (oldValue, newValue) =
        EventEffect.ParametersChanged {
            kind = EffectKind.ParametersChanged
            oldValue = oldValue
            newValue = newValue
        }

    let playerEliminated (playerId) =
        EventEffect.PlayerEliminated {
            kind = EffectKind.PlayerEliminated
            value = playerId
        }

    let playerAdded (request) =
        EventEffect.PlayerAdded {
            kind = EffectKind.PlayerAdded
            value = request
        }

    let playerOutOfMoves (playerId) =
        EventEffect.PlayerOutOfMoves {
            kind = EffectKind.PlayerOutOfMoves
            value = playerId
        }

    let pieceKilled (pieceId) = 
        EventEffect.PieceKilled {
            kind = EffectKind.PieceKilled
            value = pieceId
        }

    let playersRemoved (playerIds) =
        EventEffect.PlayersRemoved {
            kind = EffectKind.PlayersRemoved
            value = playerIds
        }

    let piecesOwnershipChanged (pieceIds, oldPlayerId, newPlayerId) =
        EventEffect.PiecesOwnershipChanged {
            kind = EffectKind.PiecesOwnershipChanged
            context = pieceIds
            oldValue = oldPlayerId
            newValue = newPlayerId
        }

    let pieceMoved (pieceId, oldCellId, newCellId) =
        EventEffect.PieceMoved {
            kind = EffectKind.PieceMoved
            context = pieceId
            oldValue = oldCellId
            newValue = newCellId
        }

type EventKind =
    | GameCreated
    | GameParametersChanged
    | GameCanceled 
    | PlayerJoined
    | PlayerEjected
    | PlayerQuit
    | GameStarted
    | TurnCommitted

type Event =
    {
        kind : EventKind
        timestamp : DateTime
        effects : EventEffect list
    }           

module Event =
    let create (kind, effects) : Event =
        {
            kind = kind
            timestamp = DateTime.UtcNow
            effects = effects
        }

type StateAndEventResponse =    
    {
        game : Game
        event : Event
    }