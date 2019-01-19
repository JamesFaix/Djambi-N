module Djambi.Api.Logic.Services.EventService

open System
open Djambi.Api.Model
open Djambi.Api.Common.Collections

let private applyGameStatusChangedEffect (effect : DiffEffect<GameStatus>) (game : Game) : Game =
    match (effect.oldValue, effect.newValue) with
    | (Pending, Started) ->
        //This case is a lot more complicated
        GameStartService.applyStartGame game
    | _ ->
        { game with status = effect.newValue }

let private applyTurnCycleChangedEffect (effect : DiffEffect<int list>) (game : Game) : Game =
    { game with turnCycle = effect.newValue }

let private applyParameterChangedEffect (effect : DiffEffect<GameParameters>) (game : Game) : Game =
    { game with parameters = effect.newValue }

let private applyPlayerEliminatedEffect (effect : ScalarEffect<int>) (game : Game) : Game =
    { game with 
        players = game.players |> List.replaceIf 
            (fun p -> p.id = effect.value) 
            (fun p -> { p with isAlive = Some false })
    }

let private applyPieceKilledEffect (effect : ScalarEffect<int>) (game : Game) : Game =
    { game with 
        pieces = game.pieces |> List.replaceIf
            (fun p -> p.id = effect.value) 
            (fun p -> { p with kind = PieceKind.Corpse; playerId = None }) 
    }

let private applyPlayersRemovedEffect (effect : ScalarEffect<int list>) (game : Game) : Game =
    { game with 
        players = game.players 
            |> List.exceptWithKey (fun p -> p.id) effect.value 
    }

let private applyPlayerOutOfMovesEffect (effect : ScalarEffect<int>) (game : Game) : Game =
    //This effect is just to communicate what happened,
    //the same event should also create a PlayerEliminated and PiecesOwnershipChanged effect
    game

let private applyAddPlayerEffect (effect : ScalarEffect<CreatePlayerRequest>) (game : Game) : Game =
    let player : Player = 
        {
            id = 0
            gameId = game.id
            userId = effect.value.userId
            kind = effect.value.kind
            name = match effect.value.name with Some x -> x | None -> ""
            isAlive = None
            colorId = None
            startingRegion = None
            startingTurnNumber = None
        }

    { game with players = List.append game.players [player] }

let private applyPiecesOwnershipChangedEffect (effect : DiffWithContextEffect<int option, int list>) (game : Game) : Game = 
    { game with 
        pieces = game.pieces |> List.replaceIf
            (fun p -> effect.context |> List.contains p.id)
            (fun p -> { p with playerId = effect.newValue })
    }

let private applyPieceMovedEffect (effect : DiffWithContextEffect<int, int>) (game : Game) : Game =
    { game with 
        pieces = game.pieces |> List.replaceIf
            (fun p -> effect.context = p.id)
            (fun p -> { p with cellId = effect.newValue }) 
    }

let private applyCurrentTurnChangedEffect (effect : DiffEffect<Turn option>) (game : Game) : Game =
    { game with currentTurn = effect.newValue }

let private applyGameCreatedEffect (effect : ScalarEffect<CreateGameRequest>) : Game =
    {
        id = 0
        status = GameStatus.Pending
        createdOn = DateTime.UtcNow
        createdByUserId = effect.value.createdByUserId
        parameters = effect.value.parameters
        players = List.empty
        pieces = List.empty
        turnCycle = List.empty
        currentTurn = None
    }

let private applyEffect (effect : Effect) (game : Game) : Game =
    match effect with 
    | Effect.GameStatusChanged e -> applyGameStatusChangedEffect e game
    | Effect.TurnCycleChanged e -> applyTurnCycleChangedEffect e game
    | Effect.ParametersChanged e -> applyParameterChangedEffect e game
    | Effect.PlayerEliminated e -> applyPlayerEliminatedEffect e game
    | Effect.PieceKilled e -> applyPieceKilledEffect e game       
    | Effect.PlayersRemoved e -> applyPlayersRemovedEffect e game
    | Effect.PlayerOutOfMoves e -> applyPlayerOutOfMovesEffect e game
    | Effect.PlayerAdded e -> applyAddPlayerEffect e game
    | Effect.PiecesOwnershipChanged e -> applyPiecesOwnershipChangedEffect e game
    | Effect.PieceMoved e -> applyPieceMovedEffect e game
    | Effect.CurrentTurnChanged e -> applyCurrentTurnChangedEffect e game

let applyEvent (game : Game) (event : Event) : Game =
    let mutable game = game
    for ef in event.effects do
        game <- applyEffect ef game
    game