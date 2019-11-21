namespace Apex.Api.Logic.Services

open Apex.Api.Logic.ModelExtensions
open Apex.Api.Logic.ModelExtensions.BoardModelExtensions
open Apex.Api.Logic.ModelExtensions.GameModelExtensions
open Apex.Api.Model
open Apex.Api.Logic
open Apex.Api.Common.Control

type SelectionOptionsService() =

    let getMoveSelectionOptions(game : Game, piece : Piece) : int list =
        let board = BoardModelUtility.getBoardMetadata game.parameters.regionCount
        let paths = board.pathsFromCellId piece.cellId
        let pieceIndex = game.piecesIndexedByCell
        let takeCellsUntilAndIncluding(condition : Piece -> bool)(path : Cell list) : Cell seq =
            seq {
                let mutable stop = false
                let mutable i = 0
                while (not stop && i < path.Length) do
                    let c = path.[i]
                    i <- i + 1

                    match pieceIndex.TryFind c.id with
                    | None -> yield c
                    | Some occupant ->
                        stop <- true
                        if condition occupant
                        then yield c
            }

        let strategy = Pieces.getStrategy piece

        paths
        |> Seq.map (fun path -> path |> List.take (min strategy.moveMaxDistance path.Length))
        |> Seq.collect (takeCellsUntilAndIncluding (fun p -> strategy.canTargetWithMove
                                                            && strategy.canTargetPiece piece p))
        |> Seq.filter (fun cell ->
            if not cell.isCenter then true
            else
                match pieceIndex.TryFind cell.id with
                | None -> strategy.canStayInCenter
                | Some p -> strategy.canTargetPiece piece p
                            && strategy.canEnterCenterToEvictPiece
        )
        |> Seq.map (fun cell -> cell.id)
        |> Seq.toList

    let getSubjectSelectionOptions (game : Game, turn : Turn) : int list =
        game.piecesControlledBy game.currentPlayerId
        |> Seq.map (fun piece -> (piece, getMoveSelectionOptions(game, piece)))
        |> Seq.filter (fun (_, cells) -> cells.IsEmpty |> not)
        |> Seq.map (fun (piece, _) -> piece.cellId)
        |> Seq.toList

    let getTargetSelectionOptions(game : Game, turn : Turn) : int list =
        match turn.destinationCellId with
        | None -> []
        | Some destinationCellId ->
            match turn.subjectPiece game with
            | None -> []
            | Some subject ->
                let strategy = Pieces.getStrategy(subject)
                if not strategy.canTargetAfterMove
                then []
                else
                    let board = BoardModelUtility.getBoardMetadata game.parameters.regionCount
                    let neighbors = board.neighborsFromCellId destinationCellId
                    let pieceIndex = game.piecesIndexedByCell
                    neighbors
                    |> Seq.filter (fun cell ->
                        match pieceIndex.TryFind cell.id with
                        | None -> false
                        | Some piece -> strategy.canTargetPiece subject piece)
                    |> Seq.map (fun cell -> cell.id)
                    |> Seq.toList

    let getDropSelectionOptions(game : Game, turn : Turn) : int list =
        //Requires a subject, destination, and target
        match turn.subjectPiece game with
        | None -> []
        | Some subject ->
            match turn.destinationCellId with
            | None -> []
            | Some _ ->
                match turn.targetPiece game with
                | None -> []
                | Some target ->
                    let pieceIndex = game.piecesIndexedByCell
                    let board = BoardModelUtility.getBoardMetadata game.parameters.regionCount
                    board.cells()
                    |> Seq.filter (fun c ->
                        match pieceIndex.TryFind c.id with
                        //You can drop a piece in the cell you can from, because you left it
                        //Cannot drop a piece in any other occupied cell
                        | Some occupant -> occupant.id = subject.id
                        | None ->
                            //You can drop in any vacant cell except the center
                            if not c.isCenter then true
                            else 
                                //If the center is vacant, you must check if the target piece could stay there
                                let subjectStrategy = Pieces.getStrategy subject
                                let targetKind = if subjectStrategy.killsTarget then Corpse else target.kind
                                let targetStrategy = Pieces.getStrategyForKind targetKind
                                targetStrategy.canStayInCenter
                    ) 
                    |> Seq.map (fun c -> c.id)
                    |> Seq.toList

    let getVacateSelectionOptions(game : Game, turn : Turn) : int list =
        match turn.subjectPiece game with
        | None -> []
        | Some subject ->
            match turn.destinationCell game.parameters.regionCount with
            | None -> []
            | Some destination ->
                let strategy = Pieces.getStrategy subject
                if not destination.isCenter || strategy.canStayInCenter
                then []
                else
                    let board = BoardModelUtility.getBoardMetadata game.parameters.regionCount
                    let pieceIndex = game.piecesIndexedByCell
                    board.pathsFromCell destination
                    |> Seq.collect (fun path -> path |> Seq.takeWhile (fun cell -> (pieceIndex.TryFind cell.id).IsNone))
                    |> Seq.filter (fun cell -> not cell.isCenter)
                    |> Seq.map (fun cell -> cell.id)
                    |> Seq.toList

    member x.getSelectableCellsFromState(game : Game) : int list HttpResult =
        let turn = game.currentTurn.Value
        match turn.requiredSelectionKind with
        | None ->
            Ok []
        | Some Subject ->
            Ok <| getSubjectSelectionOptions (game, turn)
        | Some Move ->
            let subject = turn.subjectPiece(game).Value
            Ok <| getMoveSelectionOptions (game, subject)
        | Some Target ->
            Ok <| getTargetSelectionOptions (game, turn)
        | Some Drop ->
            Ok <| getDropSelectionOptions (game, turn)
        | Some Vacate ->
            Ok <| getVacateSelectionOptions (game, turn)
         