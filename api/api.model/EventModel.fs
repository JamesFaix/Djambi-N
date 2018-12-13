[<AutoOpen>]
module Djambi.Api.Model.EventModel

open System

type EffectKind =
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
    | GameStatusChanged of DiffEffect<GameStatus>
    | TurnCycleChanged of DiffEffect<int list>
    | ParametersChanged of DiffEffect<GameParameters>
    //TODO: PlayerEliminated may need to change to PlayerStatusChanged as a DiffWithCOntext when statuses are added to players
    | PlayerEliminated of ScalarEffect<int>
    | PieceKilled of ScalarEffect<int>
    | PlayersRemoved of ScalarEffect<int list>
    | PlayerOutOfMoves of ScalarEffect<int>
    | PlayerAdded of ScalarEffect<Player>
    | PiecesOwnershipChanged of DiffWithContextEffect<int list, int>
    | PieceMoved of DiffWithContextEffect<int, int>

module EventEffect =
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

    let playerAdded (player) =
        EventEffect.PlayerAdded {
            kind = EffectKind.PlayerAdded
            value = player
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

type BasicEvent =
    {
        kind : EventKind
        timestamp : DateTime
        effects : EventEffect list
    }

type EventWithContext<'a> = 
    { 
        kind : EventKind
        timestamp : DateTime
        effects : EventEffect list
        context : 'a
    }

type Event = 
    | GameParametersChanged of BasicEvent
    | GameCanceled of BasicEvent
    | PlayerJoined of BasicEvent
    | PlayerEjected of BasicEvent
    | PlayerQuit of BasicEvent
    | GameCreated of EventWithContext<Game>
    | GameStarted of EventWithContext<Game>
    | TurnCommitted of EventWithContext<Turn>

module Event =
    let gameParametersChanged (timestamp, oldParameters, newParameters) =
        Event.GameParametersChanged {
            kind = EventKind.GameParametersChanged
            timestamp = timestamp
            effects = [ EventEffect.parametersChanged(oldParameters, newParameters) ]
        }

    let gameCanceled (timestamp) =
        Event.GameCanceled {
            kind = EventKind.GameCanceled
            timestamp = timestamp
            effects = [ EventEffect.gameStatusChanged(GameStatus.Pending, GameStatus.AbortedWhilePending) ]
        }

    let playerJoined (timestamp, player) =
        Event.PlayerJoined {
            kind = EventKind.PlayerJoined
            timestamp = timestamp
            effects = [ EventEffect.playerAdded(player) ]
        }

    let playerEjected (timestamp, playerAndGuestIds) =
        Event.PlayerEjected {
            kind = EventKind.PlayerEjected
            timestamp = timestamp
            effects = [ EventEffect.PlayersRemoved(playerAndGuestIds) ]
        }

    let playerQuit (timestamp, effects) =
        Event.PlayerQuit {
            kind = EventKind.PlayerQuit
            timestamp = timestamp
            effects = effects
        }

    let gameCreated (timestamp, game) =
        Event.GameCreated {
            kind = EventKind.GameCreated
            timestamp = timestamp
            effects = []
            context = game
        }

    let gameStarted (timestamp, game) =
        Event.GameStarted {
            kind = EventKind.GameStarted
            timestamp = timestamp
            effects = [ EventEffect.gameStatusChanged(GameStatus.Pending, GameStatus.Started) ]
            context = game
        }