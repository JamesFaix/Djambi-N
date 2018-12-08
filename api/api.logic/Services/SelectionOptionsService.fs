module Djambi.Api.Logic.Services.SelectionOptionsService

open Djambi.Api.Logic.ModelExtensions
open Djambi.Api.Logic.ModelExtensions.BoardModelExtensions
open Djambi.Api.Logic.ModelExtensions.GameModelExtensions
open Djambi.Api.Model

let private getMoveSelectionOptions(game : GameState, piece : Piece, regionCount : int) : int list =
    let board = BoardModelUtility.getBoardMetadata regionCount
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

    let excludeCenterUnless(condition : Piece -> bool)(cell : Cell) : bool =
        if cell.isCenter
        then match pieceIndex.TryFind cell.id with
                | None -> false
                | Some p -> condition p
        else true

    match piece.pieceType with
    | Chief ->
        paths
        |> Seq.collect (takeCellsUntilAndIncluding (fun p -> p.isAlive && p.playerId <> piece.playerId))
        |> Seq.map (fun cell -> cell.id)
        |> Seq.toList
    | Thug ->
        paths
        |> Seq.map (fun path -> path |> List.take (min 2 path.Length))
        |> Seq.collect (takeCellsUntilAndIncluding (fun p -> p.isAlive && p.playerId <> piece.playerId))
        |> Seq.filter (excludeCenterUnless (fun _ -> false))
        |> Seq.map (fun cell -> cell.id)
        |> Seq.toList
    | Assassin ->
        paths
        |> Seq.collect (takeCellsUntilAndIncluding (fun p -> p.isAlive && p.playerId <> piece.playerId))
        |> Seq.filter (excludeCenterUnless (fun p -> p.pieceType = Chief))
        |> Seq.map (fun cell -> cell.id)
        |> Seq.toList
    | Reporter ->
        paths
        |> Seq.collect (takeCellsUntilAndIncluding (fun _ -> false))
        |> Seq.filter (excludeCenterUnless (fun _ -> false))
        |> Seq.map (fun cell -> cell.id)
        |> Seq.toList
    | Diplomat ->
        paths
        |> Seq.collect (takeCellsUntilAndIncluding (fun p -> p.isAlive && p.playerId <> piece.playerId))
        |> Seq.filter (excludeCenterUnless (fun p -> p.pieceType = Chief))
        |> Seq.map (fun cell -> cell.id)
        |> Seq.toList
    | Gravedigger ->
        paths
        |> Seq.collect (takeCellsUntilAndIncluding (fun p -> not p.isAlive))
        |> Seq.filter (excludeCenterUnless (fun p -> p.pieceType = Corpse))
        |> Seq.map (fun cell -> cell.id)
        |> Seq.toList
    | _ -> List.empty

let private getTargetSelectionOptions(game : GameState, turn : TurnState, regionCount : int) : int list =
    match turn.destinationCellId with
    | None -> List.empty
    | Some destinationCellId ->
        match turn.subjectPiece game with
        | None -> list.Empty
        | Some subject ->
            match subject.pieceType with
            | Reporter ->
                let board = BoardModelUtility.getBoardMetadata regionCount
                let neighbors = board.neighborsFromCellId destinationCellId
                let pieceIndex = game.piecesIndexedByCell
                neighbors |> Seq.filter (fun cell -> match pieceIndex.TryFind cell.id with
                                                        | None -> false
                                                        | Some piece -> piece.playerId <> subject.playerId)
                            |> Seq.map (fun cell -> cell.id)
                            |> Seq.toList
            | _ -> List.Empty

let private getDropSelectionOptions(game : GameState, turn : TurnState, regionCount : int) : int list =
    match turn.subjectPiece game with
    | None -> List.empty
    | Some subject ->
        match turn.destinationCellId with
        | None -> List.empty
        | Some _ ->
            let pieceIndex = game.piecesIndexedByCell
            let board = BoardModelUtility.getBoardMetadata regionCount
            board.cells()
            |> Seq.filter (fun c -> match pieceIndex.TryFind c.id with
                                    | None -> true
                                    | Some occupant -> occupant.id = subject.id)
            |> Seq.map (fun c -> c.id)
            |> Seq.toList

let private getVacateSelectionOptions(game : GameState, turn : TurnState, regionCount : int) : int list =
    match turn.subjectPiece game with
    | None -> List.empty
    | Some subject ->
        match turn.destinationCell regionCount with
        | None -> List.empty
        | Some destination ->
            if not destination.isCenter
            then List.empty
            else match subject.pieceType with
                    | Assassin
                    | Gravedigger
                    | Diplomat ->
                        let board = BoardModelUtility.getBoardMetadata regionCount
                        let pieceIndex = game.piecesIndexedByCell
                        board.pathsFromCell destination
                        |> Seq.collect (fun path -> path |> Seq.takeWhile (fun cell -> (pieceIndex.TryFind cell.id).IsNone))
                        |> Seq.filter (fun cell -> not cell.isCenter)
                        |> Seq.map (fun cell -> cell.id)
                        |> Seq.toList
                    | _ -> List.empty

let getSelectableCellsFromState(game : Game) : (int list * SelectionType option) =
    let currentPlayerId = game.gameState.currentPlayerId
    match game.turnState.selections.Length with
    | 0 -> let cellIds = game.gameState.piecesControlledBy currentPlayerId
                            |> Seq.map (fun piece -> (piece, getMoveSelectionOptions(game.gameState, piece, game.regionCount)))
                            |> Seq.filter (fun (_, cells) -> cells.IsEmpty |> not)
                            |> Seq.map (fun (piece, _) -> piece.cellId)
                            |> Seq.toList
           (cellIds, Some Subject)
    | 1 -> let subject = game.turnState.subjectPiece(game.gameState).Value
           let cellIds = getMoveSelectionOptions(game.gameState, subject, game.regionCount)
           (cellIds, Some Move)
    | 2 -> let subject = game.turnState.subjectPiece(game.gameState).Value
           match (subject.pieceType, game.turnState.selections.[1]) with
            | Reporter, _ ->
                let cellIds = getTargetSelectionOptions(game.gameState, game.turnState, game.regionCount)
                if cellIds.IsEmpty
                then (List.empty, None)
                else (cellIds, Some Target)
            | Thug, sel
            | Chief, sel
            | Diplomat, sel
            | Gravedigger, sel when sel.pieceId.IsSome ->
                let cellIds = getDropSelectionOptions(game.gameState, game.turnState, game.regionCount)
                (cellIds, Some Drop)
            | Assassin, sel when sel.pieceId.IsSome ->
                let cellIds = getVacateSelectionOptions(game.gameState, game.turnState, game.regionCount)
                if cellIds.IsEmpty
                then (List.empty, None)
                else (cellIds, Some Vacate)
            | _ -> (List.empty, None)
    | 3 -> let subject = game.turnState.subjectPiece(game.gameState).Value
           match (subject.pieceType, game.turnState.selections.[1]) with
            | Diplomat, sel
            | Gravedigger, sel when sel.pieceId.IsSome ->
                let cellIds = getVacateSelectionOptions(game.gameState, game.turnState, game.regionCount)
                if cellIds.IsEmpty
                then (List.empty, None)
                else (cellIds, Some Vacate)
            | _ -> (List.empty, None)
    | _ -> (List.empty, None)