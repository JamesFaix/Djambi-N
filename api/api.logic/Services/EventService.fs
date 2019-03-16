namespace Djambi.Api.Logic.Services

open Djambi.Api.Model
open Djambi.Api.Common.Collections

type EventService(gameStartServ : GameStartService) =

    let applyCurrentTurnChangedEffect (effect : CurrentTurnChangedEffect) (game : Game) : Game =
        { game with currentTurn = effect.newValue }

    let applyGameStatusChangedEffect (effect : GameStatusChangedEffect) (game : Game) : Game =
        match (effect.oldValue, effect.newValue) with
        | (Pending, InProgress) ->
            //This case is a lot more complicated
            gameStartServ.applyStartGame game
        | _ ->
            { game with status = effect.newValue }

    let applyNeutralPlayerAddedEffect (effect : NeutralPlayerAddedEffect) (game : Game) : Game =
        let player : Player = 
            {
                id = 0
                gameId = game.id
                userId = None
                kind = PlayerKind.Neutral
                name = effect.name
                status = PlayerStatus.Pending
                colorId = None
                startingRegion = None
                startingTurnNumber = None
            }

        { game with players = List.append game.players [player] }

    let applyParameterChangedEffect (effect : ParametersChangedEffect) (game : Game) : Game =
        { game with parameters = effect.newValue }

    let applyPieceAbandonedEffect (effect : PieceAbandonedEffect) (game : Game) : Game = 
        { game with 
            pieces = game.pieces |> List.replaceIf
                (fun p -> p.id = effect.oldPiece.id)
                (fun p -> { p with playerId = None })
        }
    
    let applyPieceDroppedEffect (effect : PieceDroppedEffect) (game : Game) : Game =
        { game with 
            pieces = game.pieces |> List.replaceIf
                (fun p -> effect.oldPiece.id = p.id)
                (fun p -> { p with cellId = effect.newCellId }) 
        }

    let applyPieceEnlistedEffect (effect : PieceEnlistedEffect) (game : Game) : Game = 
        { game with 
            pieces = game.pieces |> List.replaceIf
                (fun p -> p.id = effect.oldPiece.id)
                (fun p -> { p with playerId = Some effect.newPlayerId })
        }

    let applyPieceKilledEffect (effect : PieceKilledEffect) (game : Game) : Game =
        { game with 
            pieces = game.pieces |> List.replaceIf
                (fun p -> p.id = effect.oldPiece.id) 
                (fun p -> { p with kind = PieceKind.Corpse; playerId = None }) 
        }

    let applyPieceMovedEffect (effect : PieceMovedEffect) (game : Game) : Game =
        { game with 
            pieces = game.pieces |> List.replaceIf
                (fun p -> effect.oldPiece.id = p.id)
                (fun p -> { p with cellId = effect.newCellId }) 
        }
    
    let applyPieceVacatedEffect (effect : PieceVacatedEffect) (game : Game) : Game =
        { game with 
            pieces = game.pieces |> List.replaceIf
                (fun p -> effect.oldPiece.id = p.id)
                (fun p -> { p with cellId = effect.newCellId }) 
        }

    let applyPlayerAddedEffect (effect : PlayerAddedEffect) (game : Game) : Game =
        let player : Player = 
            {
                id = 0
                gameId = game.id
                userId = Some effect.userId
                kind = effect.kind
                name = match effect.name with Some x -> x | None -> ""
                status = PlayerStatus.Pending
                colorId = None
                startingRegion = None
                startingTurnNumber = None
            }

        { game with players = List.append game.players [player] }

    let applyPlayerOutOfMovesEffect (effect : PlayerOutOfMovesEffect) (game : Game) : Game =
        //This effect is just to communicate what happened,
        //the same event should also create a PlayerEliminated and PiecesOwnershipChanged effect
        game

    let applyPlayerRemovedEffect (effect : PlayerRemovedEffect) (game : Game) : Game =
        { game with 
            players = game.players |> List.filter (fun p -> p.id <> effect.playerId)
        }

    let applyPlayerStatusChangedEffect (effect : PlayerStatusChangedEffect) (game : Game) : Game =
        { game with 
            players = game.players |> List.replaceIf 
                (fun p -> p.id = effect.playerId) 
                (fun p -> { p with status = effect.newStatus })
        }

    let applyTurnCycleAdvancedEffect (effect : TurnCycleAdvancedEffect) (game : Game) : Game =
        { game with turnCycle = effect.newValue }

    let applyTurnCyclePlayerFellFromPowerEffect (effect : TurnCyclePlayerFellFromPowerEffect) (game : Game) : Game =
        { game with turnCycle = effect.newValue }

    let applyTurnCyclePlayerRemovedEffect (effect : TurnCyclePlayerRemovedEffect) (game : Game) : Game =
        { game with turnCycle = effect.newValue }

    let applyTurnCyclePlayerRoseToPowerEffect (effect : TurnCyclePlayerRoseToPowerEffect) (game : Game) : Game =
        { game with turnCycle = effect.newValue }

    member x.applyEffect (effect : Effect) (game : Game) : Game =
        match effect with 
        | Effect.CurrentTurnChanged e -> applyCurrentTurnChangedEffect e game
        | Effect.GameStatusChanged e -> applyGameStatusChangedEffect e game
        | Effect.NeutralPlayerAdded e -> applyNeutralPlayerAddedEffect e game
        | Effect.ParametersChanged e -> applyParameterChangedEffect e game
        | Effect.PieceAbandoned e -> applyPieceAbandonedEffect e game
        | Effect.PieceDropped e -> applyPieceDroppedEffect e game
        | Effect.PieceEnlisted e -> applyPieceEnlistedEffect e game
        | Effect.PieceKilled e -> applyPieceKilledEffect e game       
        | Effect.PieceMoved e -> applyPieceMovedEffect e game
        | Effect.PieceVacated e -> applyPieceVacatedEffect e game
        | Effect.PlayerAdded e -> applyPlayerAddedEffect e game
        | Effect.PlayerOutOfMoves e -> applyPlayerOutOfMovesEffect e game
        | Effect.PlayerRemoved e -> applyPlayerRemovedEffect e game
        | Effect.PlayerStatusChanged e -> applyPlayerStatusChangedEffect e game
        | Effect.TurnCycleAdvanced e -> applyTurnCycleAdvancedEffect e game
        | Effect.TurnCyclePlayerFellFromPower e -> applyTurnCyclePlayerFellFromPowerEffect e game
        | Effect.TurnCyclePlayerRemoved e -> applyTurnCyclePlayerRemovedEffect e game
        | Effect.TurnCyclePlayerRoseToPower e -> applyTurnCyclePlayerRoseToPowerEffect e game

    member x.applyEffects (effects : Effect seq) (game : Game) : Game =
        let mutable game = game
        for ef in effects do
            game <- applyEffect ef game
        game

    member x.applyEvent (game : Game) (eventRequest : CreateEventRequest) : Game =
        applyEffects eventRequest.effects game