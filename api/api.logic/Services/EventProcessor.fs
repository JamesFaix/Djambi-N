//This module applies the effects of a given event to the game state
module Djambi.Api.Logic.Services.EventProcessor

open Djambi.Api.Model
open Djambi.Api.Common.Collections
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Repositories

let processGameStatusChangedEffect (effect : DiffEffect<GameStatus>) (game : Game) : Game AsyncHttpResult =
    match (effect.oldValue, effect.newValue) with
    | (Pending, Started) ->
        //This case is a lot more complicated
        GameStartService.startGame game
    | _ ->
        let request : UpdateGameStateRequest = {
            gameId = game.id
            status = effect.newValue
            pieces = game.pieces
            turnCycle = game.turnCycle
            currentTurn = game.currentTurn
        }
        GameRepository.updateGameState request
        |> thenMap (fun _ -> { game with status = effect.newValue })

let processTurnCycleChangedEffect (effect : DiffEffect<int list>) (game : Game) : Game AsyncHttpResult =
    let request : UpdateGameStateRequest = {
        gameId = game.id
        status = game.status
        pieces = game.pieces
        turnCycle = effect.newValue
        currentTurn = game.currentTurn
    }
    GameRepository.updateGameState request
    |> thenMap (fun _ -> { game with turnCycle = effect.newValue })

let processParameterChangedEffect (effect : DiffEffect<GameParameters>) (game : Game) : Game AsyncHttpResult =
    GameRepository.updateGameParameters game.id effect.newValue
    |> thenMap (fun _ -> { game with parameters = effect.newValue })

let processPlayerEliminatedEffect (effect : ScalarEffect<int>) (game : Game) : Game AsyncHttpResult =
    GameRepository.killPlayer effect.value
    |> thenMap (fun _ -> 
        { game with 
            players = game.players |> List.replaceIf 
                (fun p -> p.id = effect.value) 
                (fun p -> { p with isAlive = Some false })
        }
    )

let processPieceKilledEffect (effect : ScalarEffect<int>) (game : Game) : Game AsyncHttpResult =
    let updatedPieces = 
        game.pieces |> List.replaceIf
            (fun p -> p.id = effect.value) 
            (fun p -> { p with kind = PieceKind.Corpse; playerId = None })

    let request : UpdateGameStateRequest =  
        {
            gameId = game.id
            status = game.status
            pieces = updatedPieces
            turnCycle = game.turnCycle
            currentTurn = game.currentTurn
        }
    GameRepository.updateGameState request
    |> thenMap (fun _ -> { game with pieces = updatedPieces })

let processPlayersRemovedEffect (effect : ScalarEffect<int list>) (game : Game) : Game AsyncHttpResult =
    okTask (effect.value |> Seq.ofList)
    |> thenDoEachAsync (fun pId -> GameRepository.removePlayer pId)
    |> thenMap (fun _ -> 
        let updatedPlayers = 
            game.players |> List.exceptWithKey (fun p -> p.id) effect.value
        { game with players = updatedPlayers }
    )

let private processPlayerOutOfMovesEffect (effect : ScalarEffect<int>) (game : Game) : Game AsyncHttpResult =
    //This effect is just to communicate what happened,
    //the same event should also create a PlayerEliminated and PiecesOwnershipChanged effect
    okTask game

let processAddPlayerEffect (effect : ScalarEffect<CreatePlayerRequest>) (game : Game) : Game AsyncHttpResult =
    GameRepository.addPlayer (game.id, effect.value)
    |> thenMap (fun player -> 
        { game with players = List.append game.players [player] }
    )

let processPiecesOwnershipChangedEffect (effect : DiffWithContextEffect<int option, int list>) (game : Game) : Game AsyncHttpResult = 
    let updatedPieces = 
        game.pieces |> List.replaceIf
            (fun p -> effect.context |> List.contains p.id)
            (fun p -> { p with playerId = effect.newValue })
    let request : UpdateGameStateRequest =
        {
            gameId = game.id
            status = game.status
            pieces = updatedPieces
            turnCycle = game.turnCycle
            currentTurn = game.currentTurn
        }
    GameRepository.updateGameState request
    |> thenMap (fun _ -> { game with pieces = updatedPieces })

let processPieceMovedEffect (effect : DiffWithContextEffect<int, int>) (game : Game) : Game AsyncHttpResult =
    let updatedPieces =
        game.pieces |> List.replaceIf
            (fun p -> effect.context = p.id)
            (fun p -> { p with cellId = effect.newValue })
    let request : UpdateGameStateRequest =
        {
            gameId = game.id
            status = game.status
            pieces = updatedPieces
            turnCycle = game.turnCycle
            currentTurn = game.currentTurn
        }
    GameRepository.updateGameState request
    |> thenMap (fun _ -> { game with pieces = updatedPieces })

let processCurrentTurnChangedEffect (effect : DiffEffect<Turn option>) (game : Game) : Game AsyncHttpResult =
    let request : UpdateGameStateRequest = 
        {
            gameId = game.id
            status = game.status
            pieces = game.pieces
            turnCycle = game.turnCycle
            currentTurn = effect.newValue
        }
    GameRepository.updateGameState request
    |> thenMap (fun _ -> { game with currentTurn = effect.newValue })

let processGameCreatedEffect (effect : ScalarEffect<CreateGameRequest>) : Game AsyncHttpResult =
    GameRepository.createGame effect.value
    |> thenBindAsync GameRepository.getGame

let private processEffect (effect : Effect) (game : Game) : Game AsyncHttpResult =
        
    match effect with 
    | Effect.GameCreated e ->
        failwith "Must process game created separately because a game does not yet exist."

    | Effect.GameStatusChanged e -> processGameStatusChangedEffect e game
    | Effect.TurnCycleChanged e -> processTurnCycleChangedEffect e game
    | Effect.ParametersChanged e -> processParameterChangedEffect e game
    | Effect.PlayerEliminated e -> processPlayerEliminatedEffect e game
    | Effect.PieceKilled e -> processPieceKilledEffect e game        
    | Effect.PlayersRemoved e -> processPlayersRemovedEffect e game
    | Effect.PlayerOutOfMoves e -> processPlayerOutOfMovesEffect e game
    | Effect.PlayerAdded e -> processAddPlayerEffect e game
    | Effect.PiecesOwnershipChanged e -> processPiecesOwnershipChangedEffect e game
    | Effect.PieceMoved e -> processPieceMovedEffect e game
    | Effect.CurrentTurnChanged e -> processCurrentTurnChangedEffect e game

let processEvent (game : Game option) (event : Event) : StateAndEventResponse AsyncHttpResult =

    match (game, event) with
    | (None, e) when e.kind = EventKind.GameCreated ->
        match (e.effects.[0], e.effects.[1]) with
        | (Effect.GameCreated createGameEffect, Effect.PlayerAdded addPlayerEffect) ->
            processGameCreatedEffect createGameEffect
            |> thenBindAsync (processAddPlayerEffect addPlayerEffect)
        | _ -> 
            failwith "Invalid GameCreated event effects."
    
    | (None, _) ->
        failwith "Only GameCreated event can be processed without a game."

    | (Some _, e) when e.kind = EventKind.GameCreated ->
        failwith "GameCreated event must be processed without a game."

    | (Some g, _) ->
        let projections = event.effects |> Seq.map processEffect
        applyEachAsync projections g
        
    |> thenMap (fun g -> 
        {
            game = g
            event = event
        }
    )