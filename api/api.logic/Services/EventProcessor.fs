//This module applies the effects of a given event to the game state
module Djambi.Api.Logic.Services.EventProcessor

open Djambi.Api.Model
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Repositories

let processGameStatusChangedEffect (effect : DiffEffect<GameStatus>) (game : Game) : Game AsyncHttpResult = 
    let newGame = EventService.applyGameStatusChangedEffect effect game
    EventRepository.persistEvent (game, newGame)

let processTurnCycleChangedEffect (effect : DiffEffect<int list>) (game : Game) : Game AsyncHttpResult =
    let newGame = EventService.applyTurnCycleChangedEffect effect game
    EventRepository.persistEvent (game, newGame)

let processParameterChangedEffect (effect : DiffEffect<GameParameters>) (game : Game) : Game AsyncHttpResult =
    let newGame = EventService.applyParameterChangedEffect effect game
    EventRepository.persistEvent (game, newGame)

let processPlayerEliminatedEffect (effect : ScalarEffect<int>) (game : Game) : Game AsyncHttpResult =
    let newGame = EventService.applyPlayerEliminatedEffect effect game
    EventRepository.persistEvent (game, newGame)

let processPieceKilledEffect (effect : ScalarEffect<int>) (game : Game) : Game AsyncHttpResult =
    let newGame = EventService.applyPieceKilledEffect effect game
    EventRepository.persistEvent (game, newGame)

let processPlayersRemovedEffect (effect : ScalarEffect<int list>) (game : Game) : Game AsyncHttpResult =
    let newGame = EventService.applyPlayersRemovedEffect effect game
    EventRepository.persistEvent (game, newGame)

let private processPlayerOutOfMovesEffect (effect : ScalarEffect<int>) (game : Game) : Game AsyncHttpResult =
    //This effect is just to communicate what happened,
    //the same event should also create a PlayerEliminated and PiecesOwnershipChanged effect
    okTask game

let processAddPlayerEffect (effect : ScalarEffect<CreatePlayerRequest>) (game : Game) : Game AsyncHttpResult =
    let newGame = EventService.applyAddPlayerEffect effect game
    EventRepository.persistEvent (game, newGame)
    
let processPiecesOwnershipChangedEffect (effect : DiffWithContextEffect<int option, int list>) (game : Game) : Game AsyncHttpResult = 
    let newGame = EventService.applyPiecesOwnershipChangedEffect effect game
    EventRepository.persistEvent (game, newGame)

let processPieceMovedEffect (effect : DiffWithContextEffect<int, int>) (game : Game) : Game AsyncHttpResult =
    let newGame = EventService.applyPieceMovedEffect effect game
    EventRepository.persistEvent (game, newGame)

let processCurrentTurnChangedEffect (effect : DiffEffect<Turn option>) (game : Game) : Game AsyncHttpResult =
    let newGame = EventService.applyCurrentTurnChangedEffect effect game
    EventRepository.persistEvent (game, newGame)
    
let processEvent (game : Game) (event : Event) : StateAndEventResponse AsyncHttpResult =    
    let newGame = EventService.applyEvent game event
    EventRepository.persistEvent (game, newGame)
    |> thenMap (fun persistedGame -> { game = persistedGame; event = event })