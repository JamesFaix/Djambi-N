module Djambi.Api.Logic.Services.TurnService

open System.Linq
open Djambi.Api.Common.Collections
open Djambi.Api.Common.Control
open Djambi.Api.Logic
open Djambi.Api.Logic.ModelExtensions.GameModelExtensions
open Djambi.Api.Logic.PieceStrategies
open Djambi.Api.Logic.Services
open Djambi.Api.Model

//--- Commit

let private getPrimaryEffects (game : Game) : Effect list =
    let pieces = game.pieces.ToDictionary(fun p -> p.id)
    let currentTurn = game.currentTurn.Value

    let effects = new ArrayList<Effect>()

    match (currentTurn.subjectPiece game, currentTurn.destinationCell game.parameters.regionCount) with
    | (None, _)
    | (_, None) -> raise (HttpException(500, "Cannot commit turn without subject and destination selected."))
    | (Some subject, Some destination) ->
        let originCellId = subject.cellId

        //Move subject to destination or vacate cell
        match currentTurn.vacateCellId with
        | None ->        
            pieces.[subject.id] <- subject.moveTo destination.id
            effects.Add(Effect.PieceMoved { oldPiece = subject; newCellId = destination.id })
        | Some vacateCellId -> 
            pieces.[subject.id] <- subject.moveTo vacateCellId
            effects.Add(Effect.PieceVacated { oldPiece = { subject with cellId = destination.id}; newCellId = vacateCellId })

        match currentTurn.targetPiece game with
        | None -> ()
        | Some target ->
            let subjectStrategy = PieceService.getStrategy subject

            //Kill target
            if subjectStrategy.killsTarget
            then 
                pieces.[target.id] <- target.kill
                effects.Add(Effect.PieceKilled { oldPiece = target })
                
            //Drop target if drop cell exists
            match currentTurn.dropCellId with
            | Some dropCellId ->  
                pieces.[target.id] <- pieces.[target.id].moveTo dropCellId
                effects.Add(Effect.PieceDropped { oldPiece = target; newCellId = dropCellId })
            | None -> ()

            //Move target back to origin if subject is assassin
            if subjectStrategy.movesTargetToOrigin
            then 
                pieces.[target.id] <- pieces.[target.id].moveTo originCellId
                effects.Add(Effect.PieceDropped { oldPiece = target; newCellId = originCellId })

        effects |> Seq.toList

let getCommitTurnEvent (game : Game) (session : Session) : CreateEventRequest HttpResult =
    (*
        The order of effects is important, both for the implementation and clarity of the event log to users.

        Move subject to destination
        Kill target (option)
        Move target to drop (option)
        Move subject to vacate (option)
    *)
    
    let effects = new ArrayList<Effect>()

    let primaryEffects = getPrimaryEffects game
    effects.AddRange primaryEffects 
    let updatedGame = EventService.applyEffects primaryEffects game

    effects.AddRange (IndirectEffectsService.getIndirectEffectsForTurnCommit (game, updatedGame))
    
    Ok {
        kind = EventKind.TurnCommitted
        effects = effects |> Seq.toList
        createdByUserId = session.user.id
        actingPlayerId = ContextService.getActingPlayerId session game //Important to use un-updated game, because there is no turn cycle in the updated game
    }
    
//--- Reset

let getResetTurnEvent(game : Game) (session : Session) : CreateEventRequest HttpResult =
    SecurityService.ensureCurrentPlayerOrOpenParticipation session game
    |> Result.bind (fun _ ->
        let updatedGame = { game with currentTurn = Some Turn.empty }
        SelectionOptionsService.getSelectableCellsFromState updatedGame
        |> Result.map (fun selectionOptions -> 
            let turn = { Turn.empty with selectionOptions = selectionOptions }
            let effects = [ 
                Effect.CurrentTurnChanged { oldValue = game.currentTurn; newValue = Some turn } 
            ]
            {
                kind = EventKind.TurnReset
                effects = effects
                createdByUserId = session.user.id
                actingPlayerId = ContextService.getActingPlayerId session game
            }
        )
    )