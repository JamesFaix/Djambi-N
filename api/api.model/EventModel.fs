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
    | GameCreated of BasicEvent
    | GameStarted of BasicEvent
    | TurnCommitted of EventWithContext<Turn>

type Event with
    member x.getEffects() =
        match x with            
        | GameParametersChanged e 
        | GameCanceled e 
        | PlayerJoined e
        | PlayerEjected e
        | PlayerQuit e ->
            e.effects
        | GameCreated e 
        | GameStarted e -> 
            e.effects
        | TurnCommitted e ->
            e.effects
            

module Event =
    let gameParametersChanged (effects) =
        Event.GameParametersChanged {
            kind = EventKind.GameParametersChanged
            timestamp = DateTime.UtcNow
            effects = effects
        }

    let gameCanceled () =
        Event.GameCanceled {
            kind = EventKind.GameCanceled
            timestamp = DateTime.UtcNow
            effects = [ EventEffect.gameStatusChanged(GameStatus.Pending, GameStatus.AbortedWhilePending) ]
        }

    let playerJoined (player) =
        Event.PlayerJoined {
            kind = EventKind.PlayerJoined
            timestamp = DateTime.UtcNow
            effects = [ EventEffect.playerAdded(player) ]
        }

    let playerEjected (effects) =
        Event.PlayerEjected {
            kind = EventKind.PlayerEjected
            timestamp = DateTime.UtcNow
            effects = effects
        }

    let playerQuit (effects) =
        Event.PlayerQuit {
            kind = EventKind.PlayerQuit
            timestamp = DateTime.UtcNow
            effects = effects
        }

    let gameCreated (gameRequest, playerRequest) =
        Event.GameCreated {
            kind = EventKind.GameCreated
            timestamp = DateTime.UtcNow
            effects = [
                EventEffect.gameCreated(gameRequest)
                EventEffect.playerAdded(playerRequest)
            ]
        }

    let gameStarted (effects) =
        Event.GameStarted {
            kind = EventKind.GameStarted
            timestamp = DateTime.UtcNow
            effects = effects
        }

type StateAndEventResponse =    
    {
        game : Game
        event : Event
    }