namespace Djambi.Api.Logic.Services

open Djambi.Api.Common.Collections
open Djambi.Api.Common.Control
open Djambi.Api.Logic
open Djambi.Api.Logic.ModelExtensions
open Djambi.Api.Logic.ModelExtensions.BoardModelExtensions
open Djambi.Api.Logic.ModelExtensions.GameModelExtensions
open Djambi.Api.Logic.Services
open Djambi.Api.Model

type SelectionService(selectionOptionsServ : SelectionOptionsService) =

    let getSubjectSelectionEventDetails (game : Game, cellId : int) : (Selection * TurnStatus * SelectionKind option) HttpResult =
        let pieces = game.piecesIndexedByCell
        match pieces.TryFind(cellId) with
        | None -> Errors.noPieceInCell()
        | Some p ->
            let selection = Selection.subject(cellId, p.id)
            Ok (selection, AwaitingSelection, Some Move)

    let getMoveSelectionEventDetails (game : Game, cellId : int) : (Selection * TurnStatus * SelectionKind option) HttpResult =
        let pieces = game.piecesIndexedByCell
        let board = BoardModelUtility.getBoardMetadata game.parameters.regionCount
        let subject = game.currentTurn.Value.subjectPiece(game).Value
        let subjectStrategy = Pieces.getStrategy subject
        match pieces.TryFind(cellId) with
        | None ->
            let selection = Selection.move(cellId)
            if subjectStrategy.canTargetAfterMove
                && board.neighborsFromCellId cellId
                    |> Seq.map (fun c -> pieces.TryFind c.id)
                    |> Seq.values
                    |> Seq.exists (fun p ->
                        let str = Pieces.getStrategy p
                        str.isAlive && p.playerId <> subject.playerId
                    )
            then Ok (selection, AwaitingSelection, Some Target)
            else Ok (selection, AwaitingCommit, None)
        | Some target ->
            let selection = Selection.moveWithTarget(cellId, target.id)
            if subjectStrategy.movesTargetToOrigin then
                match board.cell cellId with
                | None -> Errors.cellNotFound()
                | Some c when c.isCenter ->
                    Ok (selection, AwaitingSelection, Some Vacate)
                | _ ->
                    Ok (selection, AwaitingCommit, None)
            else Ok (selection, AwaitingSelection, Some Drop)

    let getTargetSelectionEventDetails (game : Game, cellId : int) : (Selection * TurnStatus * SelectionKind option) HttpResult =
        let pieces = game.piecesIndexedByCell
        match pieces.TryFind(cellId) with
        | None -> Errors.noPieceInCell()
        | Some target ->
            let selection = Selection.target(cellId, target.id)
            Ok (selection, AwaitingCommit, None)

    let getDropSelectionEventDetails (game : Game, cellId : int) : (Selection * TurnStatus * SelectionKind option) HttpResult =
        let turn = game.currentTurn.Value
        let subject = turn.subjectPiece(game).Value
        let destination = turn.destinationCell(game.parameters.regionCount).Value
        let subjectStrategy = Pieces.getStrategy subject
        let selection = Selection.drop(cellId)
        if subjectStrategy.canEnterCenterToEvictPiece
            && (not subjectStrategy.canStayInCenter)
            && destination.isCenter
        then Ok (selection, AwaitingSelection, Some Vacate)
        else Ok (selection, AwaitingCommit, None)

    let getVacateSelectionEventDetails (game : Game, cellId : int) : (Selection * TurnStatus * SelectionKind option) HttpResult =
        let selection = Selection.vacate(cellId)
        Ok (selection, AwaitingCommit, None)

    member x.getCellSelectedEvent(game : Game, cellId : int) (session: Session) : CreateEventRequest HttpResult =
        Security.ensureCurrentPlayerOrOpenParticipation session game
        |> Result.bind (fun _ ->
            let board = BoardModelUtility.getBoardMetadata game.parameters.regionCount
            match board.cell cellId with
            | Some _ -> Ok ()
            | None -> Errors.cellNotFound()
        )
        |> Result.bind (fun _ ->
            let currentTurn = game.currentTurn.Value
            if currentTurn.selectionOptions |> List.contains cellId |> not
            then Error <| HttpException(400, (sprintf "Cell %i is not currently selectable." cellId))
            elif currentTurn.status <> AwaitingSelection || currentTurn.requiredSelectionKind.IsNone
            then Errors.turnStatusDoesNotAllowSelection()
            else
                match currentTurn.requiredSelectionKind with
                | None -> Errors.turnStatusDoesNotAllowSelection()
                | Some Subject -> getSubjectSelectionEventDetails (game, cellId)
                | Some Move -> getMoveSelectionEventDetails (game, cellId)
                | Some Target -> getTargetSelectionEventDetails (game, cellId)
                | Some Drop -> getDropSelectionEventDetails (game, cellId)
                | Some Vacate -> getVacateSelectionEventDetails (game, cellId)

                |> Result.bind (fun (selection, turnStatus, requiredSelectionKind) ->
                    let turn =
                        {
                            status = turnStatus
                            selections = List.append currentTurn.selections [selection]
                            selectionOptions = []
                            requiredSelectionKind = requiredSelectionKind
                        }
                    let updatedGame = { game with currentTurn = Some turn }
                    selectionOptionsServ.getSelectableCellsFromState updatedGame
                    |> Result.map (fun selectionOptions ->
                        let turn =
                            if selectionOptions.IsEmpty && turn.status = AwaitingSelection
                            then Turn.deadEnd turn.selections
                            else { turn with selectionOptions = selectionOptions }

                        {
                            kind = EventKind.CellSelected
                            effects = [Effect.CurrentTurnChanged { oldValue = game.currentTurn; newValue = Some turn }]
                            createdByUserId = session.user.id
                            actingPlayerId = Context.getActingPlayerId session game
                        }
                    )
                )
        )