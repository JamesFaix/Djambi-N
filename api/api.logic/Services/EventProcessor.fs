//This module applies the effects of a given event to the game state
module Djambi.Api.Logic.Services.EventProcessor

open Djambi.Api.Model
open Djambi.Api.Common
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Repositories

let private processAddPlayerEffect (effect : ScalarEffect<CreatePlayerRequest>, game : Game) : Game AsyncHttpResult =
    GameRepository.addPlayer (game.id, effect.value)
    |> thenMap (fun player -> 
        { game with players = player :: game.players }
    )

//TODO: Add integration tests
let private processEffect (effect : Effect) (game : Game) : Game AsyncHttpResult =
        
    match effect with 
    | Effect.GameCreated e ->
        failwith "Must process game created separately because a game does not yet exist."

    | Effect.GameStatusChanged e ->
        match (e.oldValue, e.newValue) with
        | (Pending, Started) ->
            //This case is a lot more complicated
            GameStartService.startGame game
        | _ ->
            let request : UpdateGameStateRequest = {
                gameId = game.id
                status = e.newValue
                pieces = game.pieces
                turnCycle = game.turnCycle
                currentTurn = game.currentTurn
            }
            GameRepository.updateGameState request
            |> thenMap (fun _ -> { game with status = e.newValue })

    | Effect.TurnCycleChanged e ->
        let request : UpdateGameStateRequest = {
            gameId = game.id
            status = game.status
            pieces = game.pieces
            turnCycle = e.newValue
            currentTurn = game.currentTurn
        }
        GameRepository.updateGameState request
        |> thenMap (fun _ -> { game with turnCycle = e.newValue })

    | Effect.ParametersChanged e ->
        GameRepository.updateGameParameters game.id e.newValue
        |> thenMap (fun _ -> { game with parameters = e.newValue })

    | Effect.PlayerEliminated e ->
        GameRepository.killPlayer e.value
        |> thenMap (fun _ -> 
            { game with 
                players = game.players |> Utilities.replaceIf 
                    (fun p -> p.id = e.value) 
                    (fun p -> { p with isAlive = Some false })
            }
        )

    | Effect.PieceKilled e ->
        let updatedPieces = 
            game.pieces |> Utilities.replaceIf
                (fun p -> p.id = e.value) 
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

    | Effect.PlayersRemoved e ->
        okTask (e.value |> Seq.ofList)
        |> thenDoEachAsync (fun pId -> GameRepository.removePlayer pId)
        |> thenMap (fun _ -> 
            let updatedPlayers = 
                game.players |> Utilities.exceptWithKey (fun p -> p.id) e.value
            { game with players = updatedPlayers }
        )

    | Effect.PlayerOutOfMoves _ ->
        //This effect is just to communicate what happened,
        //the same event should also create a PlayerEliminated and PiecesOwnershipChanged effect
        okTask game

    | Effect.PlayerAdded e -> 
        processAddPlayerEffect (e, game)

    | Effect.PiecesOwnershipChanged e ->
        let updatedPieces = 
            game.pieces |> Utilities.replaceIf
                (fun p -> e.context |> List.contains p.id)
                (fun p -> { p with playerId = e.newValue })
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

    | Effect.PieceMoved e ->
        let updatedPieces =
            game.pieces |> Utilities.replaceIf
                (fun p -> e.context = p.id)
                (fun p -> { p with cellId = e.newValue })
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

let processEvent (game : Game option) (event : Event) : StateAndEventResponse AsyncHttpResult =

    match (game, event) with
    | (None, e) when e.kind = EventKind.GameCreated ->
        match (e.effects.[0], e.effects.[1]) with
        | (Effect.GameCreated createGameEffect, Effect.PlayerAdded addPlayerEffect) ->
            GameRepository.createGame createGameEffect.value
            |> thenBindAsync GameRepository.getGame
            |> thenBindAsync (fun g -> processAddPlayerEffect (addPlayerEffect, g))
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