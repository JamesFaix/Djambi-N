namespace Apex.Api.Logic.Services

open System.Linq
open Apex.Api.Common.Collections
open Apex.Api.Common.Control
open Apex.Api.Logic
open Apex.Api.Logic.ModelExtensions.GameModelExtensions
open Apex.Api.Logic.Services
open Apex.Api.Model

type TurnService(eventServ : EventService,
                 indirectEffectsServ : IndirectEffectsService,
                 selectionOptionsServ : SelectionOptionsService) =

    //--- Commit

    let getPrimaryEffects (game : Game) : Effect list =
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
                let subjectStrategy = Pieces.getStrategy subject

                //Kill target
                if subjectStrategy.killsTarget
                then
                    pieces.[target.id] <- target.kill
                    effects.Add(Effect.PieceKilled { oldPiece = target })

                //Drop target if drop cell exists
                match currentTurn.dropCellId with
                | Some dropCellId ->
                    pieces.[target.id] <- pieces.[target.id].moveTo dropCellId
                    //Changing the piece to a corpse here will ensure the returned effect list reads well to the client
                    //It should not affect how this event is processed on the server
                    let newPiece = 
                        if subjectStrategy.killsTarget 
                        then { target with cellId = dropCellId; kind = Corpse; playerId = None }
                        else { target with cellId = dropCellId }
                    effects.Add(Effect.PieceDropped { oldPiece = target; newPiece = newPiece })
                | None -> ()

                //Move target back to origin if subject is assassin
                if subjectStrategy.movesTargetToOrigin
                then
                    pieces.[target.id] <- pieces.[target.id].moveTo originCellId
                    //Changing the piece to a corpse here will ensure the returned effect list reads well to the client
                    //It should not affect how this event is processed on the server
                    let newPiece = 
                        if subjectStrategy.killsTarget 
                        then { target with cellId = originCellId; kind = Corpse; playerId = None }
                        else { target with cellId = originCellId }
                    effects.Add(Effect.PieceDropped { oldPiece = target; newPiece = newPiece })

            effects |> Seq.toList

    member x.getCommitTurnEvent (game : Game) (session : Session) : CreateEventRequest HttpResult =
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
        let updatedGame = eventServ.applyEffects primaryEffects game

        effects.AddRange (indirectEffectsServ.getIndirectEffectsForTurnCommit (game, updatedGame))

        Ok {
            kind = EventKind.TurnCommitted
            effects = effects |> Seq.toList
            createdByUserId = session.user.id
            actingPlayerId = Context.getActingPlayerId session game //Important to use un-updated game, because there is no turn cycle in the updated game
        }

    //--- Reset

    member x.getResetTurnEvent(game : Game) (session : Session) : CreateEventRequest HttpResult =
        Security.ensureCurrentPlayerOrOpenParticipation session game
        |> Result.bind (fun _ ->
            let updatedGame = { game with currentTurn = Some Turn.empty }
            selectionOptionsServ.getSelectableCellsFromState updatedGame
            |> Result.map (fun selectionOptions ->
                let turn = { Turn.empty with selectionOptions = selectionOptions }
                let effects = [
                    Effect.CurrentTurnChanged { oldValue = game.currentTurn; newValue = Some turn }
                ]
                {
                    kind = EventKind.TurnReset
                    effects = effects
                    createdByUserId = session.user.id
                    actingPlayerId = Context.getActingPlayerId session game
                }
            )
        )