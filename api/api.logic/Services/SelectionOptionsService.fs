module Djambi.Api.Logic.Services.SelectionOptionsService

open Djambi.Api.Logic.ModelExtensions
open Djambi.Api.Logic.ModelExtensions.BoardModelExtensions
open Djambi.Api.Logic.ModelExtensions.GameModelExtensions
open Djambi.Api.Model
open Djambi.Api.Logic.PieceStrategies

let private getMoveSelectionOptions(game : Game, piece : Piece) : int list =
    let board = BoardModelUtility.getBoardMetadata game.parameters.regionCount
    let paths = board.pathsFromCellId piece.cellId
    let pieceIndex = game.piecesIndexedByCell
    let takeCellsUntilAndIncluding(condition : Piece -> bool)(path : Cell list) : Cell seq =
        seq {
            let mutable stop = false
            let mutable i = 0
            while (stop = false && i < path.Length) do
                let c = path.[i]
                i <- i + 1

                match pieceIndex.TryFind c.id with
                | None -> yield c
                | Some occupant ->
                    stop <- true
                    if condition occupant
                    then yield c
        }

    let strategy = PieceService.getStrategy piece

    paths
    |> Seq.map (fun path -> path |> List.take (min strategy.moveMaxDistance path.Length))        
    |> Seq.collect (takeCellsUntilAndIncluding (fun p -> strategy.moveCanTargetPiece piece p))
    |> Seq.filter (fun cell -> 
        if not cell.isCenter then true
        else 
            match pieceIndex.TryFind cell.id with
            | None -> strategy.canStayInSeat
            | Some p -> strategy.moveCanEnterSeatToEvictPiece piece p
    )
    |> Seq.map (fun cell -> cell.id)
    |> Seq.toList

let private getTargetSelectionOptions(game : Game, turn : Turn) : int list =
    match turn.destinationCellId with
    | None -> List.empty
    | Some destinationCellId ->
        match turn.subjectPiece game with
        | None -> list.Empty
        | Some subject ->
            match subject.kind with
            | Reporter ->
                let board = BoardModelUtility.getBoardMetadata game.parameters.regionCount
                let neighbors = board.neighborsFromCellId destinationCellId
                let pieceIndex = game.piecesIndexedByCell
                neighbors |> Seq.filter (fun cell -> match pieceIndex.TryFind cell.id with
                                                        | None -> false
                                                        | Some piece -> piece.playerId <> subject.playerId)
                            |> Seq.map (fun cell -> cell.id)
                            |> Seq.toList
            | _ -> List.Empty

let private getDropSelectionOptions(game : Game, turn : Turn) : int list =
    match turn.subjectPiece game with
    | None -> List.empty
    | Some subject ->
        match turn.destinationCellId with
        | None -> List.empty
        | Some _ ->
            let pieceIndex = game.piecesIndexedByCell
            let board = BoardModelUtility.getBoardMetadata game.parameters.regionCount
            board.cells()
            |> Seq.filter (fun c -> match pieceIndex.TryFind c.id with
                                    | None -> true
                                    | Some occupant -> occupant.id = subject.id)
            |> Seq.map (fun c -> c.id)
            |> Seq.toList

let private getVacateSelectionOptions(game : Game, turn : Turn) : int list =
    match turn.subjectPiece game with
    | None -> List.empty
    | Some subject ->
        match turn.destinationCell game.parameters.regionCount with
        | None -> List.empty
        | Some destination ->
            if not destination.isCenter
            then List.empty
            else match subject.kind with
                    | Assassin
                    | Gravedigger
                    | Diplomat ->
                        let board = BoardModelUtility.getBoardMetadata game.parameters.regionCount
                        let pieceIndex = game.piecesIndexedByCell
                        board.pathsFromCell destination
                        |> Seq.collect (fun path -> path |> Seq.takeWhile (fun cell -> (pieceIndex.TryFind cell.id).IsNone))
                        |> Seq.filter (fun cell -> not cell.isCenter)
                        |> Seq.map (fun cell -> cell.id)
                        |> Seq.toList
                    | _ -> List.empty

let getSelectableCellsFromState(game : Game) : (int list * SelectionKind option) =
    let currentTurn = game.currentTurn.Value
    let currentPlayerId = game.currentPlayerId
    match currentTurn.selections.Length with
    | 0 -> let cellIds = game.piecesControlledBy currentPlayerId
                            |> Seq.map (fun piece -> (piece, getMoveSelectionOptions(game, piece)))
                            |> Seq.filter (fun (_, cells) -> cells.IsEmpty |> not)
                            |> Seq.map (fun (piece, _) -> piece.cellId)
                            |> Seq.toList
           (cellIds, Some Subject)
    | 1 -> let subject = currentTurn.subjectPiece(game).Value
           let cellIds = getMoveSelectionOptions(game, subject)
           (cellIds, Some Move)
    | 2 -> let subject = currentTurn.subjectPiece(game).Value
           match (subject.kind, currentTurn.selections.[1]) with
            | Reporter, _ ->
                let cellIds = getTargetSelectionOptions(game, currentTurn)
                if cellIds.IsEmpty
                then (List.empty, None)
                else (cellIds, Some Target)
            | Thug, sel
            | Chief, sel
            | Diplomat, sel
            | Gravedigger, sel when sel.pieceId.IsSome ->
                let cellIds = getDropSelectionOptions(game, currentTurn)
                (cellIds, Some Drop)
            | Assassin, sel when sel.pieceId.IsSome ->
                let cellIds = getVacateSelectionOptions(game, currentTurn)
                if cellIds.IsEmpty
                then (List.empty, None)
                else (cellIds, Some Vacate)
            | _ -> (List.empty, None)
    | 3 -> let subject = currentTurn.subjectPiece(game).Value
           match (subject.kind, currentTurn.selections.[1]) with
            | Diplomat, sel
            | Gravedigger, sel when sel.pieceId.IsSome ->
                let cellIds = getVacateSelectionOptions(game, currentTurn)
                if cellIds.IsEmpty
                then (List.empty, None)
                else (cellIds, Some Vacate)
            | _ -> (List.empty, None)
    | _ -> (List.empty, None)