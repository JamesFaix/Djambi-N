module Djambi.Api.Logic.Services.EventService

open System
open Djambi.Api.Model
open Djambi.Api.Common.Collections
open Djambi.Api.Common.Control

let applyGameStatusChangedEffect (effect : DiffEffect<GameStatus>) (game : Game) : Game =
    match (effect.oldValue, effect.newValue) with
    | (Pending, Started) ->
        //This case is a lot more complicated
        GameStartService.applyStartGame game
    | _ ->
        { game with status = effect.newValue }

let applyTurnCycleChangedEffect (effect : DiffEffect<int list>) (game : Game) : Game =
    { game with turnCycle = effect.newValue }

let applyParameterChangedEffect (effect : DiffEffect<GameParameters>) (game : Game) : Game =
    { game with parameters = effect.newValue }

let applyPlayerEliminatedEffect (effect : ScalarEffect<int>) (game : Game) : Game =
    { game with 
        players = game.players |> List.replaceIf 
            (fun p -> p.id = effect.value) 
            (fun p -> { p with isAlive = Some false })
    }

let applyPieceKilledEffect (effect : ScalarEffect<int>) (game : Game) : Game =
    { game with 
        pieces = game.pieces |> List.replaceIf
            (fun p -> p.id = effect.value) 
            (fun p -> { p with kind = PieceKind.Corpse; playerId = None }) 
    }

let applyPlayersRemovedEffect (effect : ScalarEffect<int list>) (game : Game) : Game =
    { game with 
        players = game.players 
            |> List.exceptWithKey (fun p -> p.id) effect.value 
    }

let private applyPlayerOutOfMovesEffect (effect : ScalarEffect<int>) (game : Game) : Game =
    //This effect is just to communicate what happened,
    //the same event should also create a PlayerEliminated and PiecesOwnershipChanged effect
    game

let applyAddPlayerEffect (effect : ScalarEffect<CreatePlayerRequest>) (game : Game) : Game =
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

let applyPiecesOwnershipChangedEffect (effect : DiffWithContextEffect<int option, int list>) (game : Game) : Game = 
    { game with 
        pieces = game.pieces |> List.replaceIf
            (fun p -> effect.context |> List.contains p.id)
            (fun p -> { p with playerId = effect.newValue })
    }

let applyPieceMovedEffect (effect : DiffWithContextEffect<int, int>) (game : Game) : Game =
    { game with 
        pieces = game.pieces |> List.replaceIf
            (fun p -> effect.context = p.id)
            (fun p -> { p with cellId = effect.newValue }) 
    }

let applyCurrentTurnChangedEffect (effect : DiffEffect<Turn option>) (game : Game) : Game =
    { game with currentTurn = effect.newValue }

let applyGameCreatedEffect (effect : ScalarEffect<CreateGameRequest>) : Game =
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

let private applyEffect (effect : Effect) (game : Game option) : Game HttpResult =
    
    match game with 
    | Some g ->
        match effect with 
        | Effect.GameCreated _ -> Error <| HttpException(500, "Cannot process GameCreated effect with an existing Game.")
        | Effect.GameStatusChanged e -> Ok <| applyGameStatusChangedEffect e g
        | Effect.TurnCycleChanged e -> Ok <| applyTurnCycleChangedEffect e g
        | Effect.ParametersChanged e -> Ok <| applyParameterChangedEffect e g
        | Effect.PlayerEliminated e -> Ok <| applyPlayerEliminatedEffect e g
        | Effect.PieceKilled e -> Ok <| applyPieceKilledEffect e g        
        | Effect.PlayersRemoved e -> Ok <| applyPlayersRemovedEffect e g
        | Effect.PlayerOutOfMoves e -> Ok <| applyPlayerOutOfMovesEffect e g
        | Effect.PlayerAdded e -> Ok <| applyAddPlayerEffect e g
        | Effect.PiecesOwnershipChanged e -> Ok <| applyPiecesOwnershipChangedEffect e g
        | Effect.PieceMoved e -> Ok <| applyPieceMovedEffect e g
        | Effect.CurrentTurnChanged e -> Ok <| applyCurrentTurnChangedEffect e g
    | None -> 
        match effect with 
        | Effect.GameCreated e -> Ok <| applyGameCreatedEffect e
        | _ -> Error <| HttpException(500, "All effects except GameCreated require an existing Game.")

let applyEvent (game : Game option) (event : Event) : Game HttpResult =

    let mutable effects = event.effects
    let mutable game = game

    //The start game effect is not based on an existing game, so it requires special treatment
    match (game, event) with
    | (None, e) when e.kind = EventKind.GameCreated ->
        match (e.effects.[0]) with
        | Effect.GameCreated ef ->
            game <- Some <| applyGameCreatedEffect ef
            effects <- effects.Tail
            Ok ()
        | _ -> 
            Error <| HttpException(500, "Invalid GameCreated event effects.")
    
    | (None, _) -> 
        Error <| HttpException(500, "Only GameCreated event can be processed without a game.")

    | (Some _, e) when e.kind = EventKind.GameCreated ->
        Error <| HttpException(500, "GameCreated event must be processed without a game.")

    | _ -> 
        Ok ()

    |> Result.bind (fun _ ->
        let mutable result = Ok <| game.Value
        let mutable stop = false

        while effects.Length > 0 && not stop do
            result <- applyEffect effects.Head game
            effects <- effects.Tail
            
            if result |> Result.isError 
            then stop <- true 
            else ()

        result            
    )