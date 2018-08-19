namespace Djambi.Api.Domain

open Giraffe
open System.Threading.Tasks
open Djambi.Api.Persistence
open Djambi.Api.Domain.PlayModels
open Djambi.Api.Domain.PlayModelExtensions
open Djambi.Api.Domain.BoardsExtensions
open Djambi.Api.Common.Enums

type PlayService(repository : PlayRepository) =

    member this.getGameState(gameId : int) : GameState Task =
        task {
            let! game = repository.getGame gameId
            return game.currentGameState
        }

    member this.getSelectableCells(gameId : int, playerId : int) : int list Task =
        task {
            let! game = repository.getGame gameId
            let gameState = game.currentGameState
            let currentPlayerId = gameState.currentPlayerId

            return if currentPlayerId <> playerId
                   then List.empty
                   else 
                        let turn = game.currentTurnState
                        match turn.selections.Length with
                        | 0 -> gameState.piecesControlledBy currentPlayerId
                               |> Seq.map (fun piece -> (piece, this.getMoveSelectionOptions(game, piece)))
                               |> Seq.filter (fun (_, cells) -> cells.IsEmpty |> not)
                               |> Seq.map (fun (piece, _) -> piece.cellId)
                               |> Seq.toList
                        | 1 -> let subject = turn.getSubjectPiece(gameState).Value
                               this.getMoveSelectionOptions(game, subject)
                        | 2 -> let subject = turn.getSubjectPiece(gameState).Value
                               match (subject.pieceType, turn.selections.[1]) with 
                               | Reporter, Move _ -> this.getTargetSelectionOptions game
                               | Thug, MoveWithTarget (_,_) 
                               | Chief, MoveWithTarget (_,_) 
                               | Diplomat, MoveWithTarget (_,_)
                               | Gravedigger, MoveWithTarget (_,_) -> this.getDropSelectionOptions game
                               | Assassin, MoveWithTarget (_,_) -> this.getVacateSelectionOptions game
                               | _ -> List.empty
                        | 3 -> let subject = turn.getSubjectPiece(gameState).Value
                               match (subject.pieceType, turn.selections.[1]) with 
                               | Diplomat, MoveWithTarget (_,_)
                               | Gravedigger, MoveWithTarget (_,_) -> this.getVacateSelectionOptions game
                               | _ -> List.empty
                        | _ -> List.empty
        }

    member private this.getMoveSelectionOptions(game : Game, piece : Piece) : int list =
        let board = BoardUtility.getBoardMetadata game.boardRegionCount
        let paths = board.pathsFromCellId piece.cellId
        let pieceIndex = game.currentGameState.piecesIndexedByCell
        match piece.pieceType with
        | Chief -> paths 
                   |> Seq.collect 
                        (fun path -> path 
                                     |> Seq.takeWhile 
                                         (fun cell -> 
                                         match pieceIndex.TryFind cell.id with
                                         | None -> true
                                         | Some occupant -> occupant.isAlive && occupant.playerId <> piece.playerId))
                   |> Seq.map (fun cell -> cell.id)
                   |> Seq.toList
        | Thug ->  paths 
                   |> Seq.collect 
                        (fun path -> path 
                                     |> Seq.mapi (fun i cell -> (i, cell))
                                     |> Seq.takeWhile 
                                         (fun (i, cell) -> 
                                         match (i, pieceIndex.TryFind cell.id) with
                                         | _, None -> true
                                         | i, Some occupant -> i < 2 && occupant.isAlive && occupant.playerId <> piece.playerId))
                   |> Seq.filter (fun (i, cell) -> cell.isCenter() |> not)
                   |> Seq.map (fun (i, cell) -> cell.id)
                   |> Seq.toList
        | Assassin -> paths 
                       |> Seq.collect 
                            (fun path -> path 
                                         |> Seq.takeWhile 
                                             (fun cell -> 
                                             match pieceIndex.TryFind cell.id with
                                             | None -> true
                                             | Some occupant -> occupant.isAlive && occupant.playerId <> piece.playerId))
                       |> Seq.filter (fun cell -> if cell.isCenter() |> not
                                                  then true
                                                  else match pieceIndex.TryFind cell.id with
                                                       | None -> false
                                                       | Some occupant -> match occupant.pieceType with | Chief -> true | _ -> false)
                       |> Seq.map (fun cell -> cell.id)
                       |> Seq.toList
        | Reporter -> paths 
                       |> Seq.collect 
                            (fun path -> path 
                                         |> Seq.takeWhile 
                                             (fun cell -> 
                                             match pieceIndex.TryFind cell.id with
                                             | None -> true
                                             | Some occupant -> occupant.isAlive && occupant.playerId <> piece.playerId))
                       |> Seq.filter (fun cell -> cell.isCenter() |> not)
                       |> Seq.map (fun cell -> cell.id)
                       |> Seq.toList
        | Diplomat -> paths 
                       |> Seq.collect 
                            (fun path -> path 
                                         |> Seq.takeWhile 
                                             (fun cell -> 
                                             match pieceIndex.TryFind cell.id with
                                             | None -> true
                                             | Some occupant -> occupant.isAlive && occupant.playerId <> piece.playerId))
                       |> Seq.filter (fun cell -> if cell.isCenter() |> not
                                                  then true
                                                  else match pieceIndex.TryFind cell.id with
                                                       | None -> false
                                                       | Some occupant -> match occupant.pieceType with | Chief -> true | _ -> false)
                       |> Seq.map (fun cell -> cell.id)
                       |> Seq.toList
        | Gravedigger -> paths 
                       |> Seq.collect 
                            (fun path -> path 
                                         |> Seq.takeWhile 
                                             (fun cell -> 
                                             match pieceIndex.TryFind cell.id with
                                             | None -> true
                                             | Some occupant -> occupant.isAlive |> not))
                       |> Seq.filter (fun cell -> if cell.isCenter() |> not
                                                  then true
                                                  else match pieceIndex.TryFind cell.id with
                                                       | None -> false
                                                       | Some occupant -> occupant.isAlive |> not)
                       |> Seq.map (fun cell -> cell.id)
                       |> Seq.toList
        
    member private this.getTargetSelectionOptions(game : Game) : int list =
        let turn = game.currentTurnState
        match turn.getDestinationCellId with
        | None -> List.empty
        | Some destinationCellId -> 
            match turn.getSubjectPiece game.currentGameState with
            | None -> list.Empty
            | Some subject ->
                match subject.pieceType with
                | Reporter -> 
                    let board = BoardUtility.getBoardMetadata game.boardRegionCount
                    let neighbors = board.neighborsFromCellId destinationCellId
                    let pieceIndex = game.currentGameState.piecesIndexedByCell
                    neighbors |> Seq.filter (fun cell -> match pieceIndex.TryFind cell.id with
                                                            | None -> false
                                                            | Some piece -> piece.playerId <> subject.playerId)
                              |> Seq.map (fun cell -> cell.id)
                              |> Seq.toList
                | _ -> List.Empty

    member private this.getDropSelectionOptions(game : Game) : int list =
        let turn = game.currentTurnState
        match turn.getSubjectPiece game.currentGameState with
        | None -> List.empty
        | Some subject -> 
            match turn.getDestinationCellId with
            | None -> List.empty
            | Some _ -> 
                let pieceIndex = game.currentGameState.piecesIndexedByCell
                let board = BoardUtility.getBoardMetadata game.boardRegionCount
                board.cells()
                |> Seq.filter (fun c -> match pieceIndex.TryFind c.id with
                                        | None -> true
                                        | Some occupant -> occupant.id = subject.id)
                |> Seq.map (fun c -> c.id)
                |> Seq.toList

    member private this.getVacateSelectionOptions(game : Game) : int list =
        let turn = game.currentTurnState
        match turn.getSubjectPiece game.currentGameState with
        | None -> List.empty
        | Some subject -> 
            match turn.getDestinationCell(game.boardRegionCount) with
            | None -> List.empty
            | Some destination -> 
                if destination.isCenter() |> not
                then List.empty
                else match subject.pieceType with
                     | Assassin
                     | Gravedigger 
                     | Diplomat -> let board = BoardUtility.getBoardMetadata game.boardRegionCount
                                   let pieceIndex = game.currentGameState.piecesIndexedByCell       
                                   board.pathsFromCell destination
                                   |> Seq.collect (fun path -> path |> Seq.takeWhile (fun cell -> (pieceIndex.TryFind cell.id).IsNone))
                                   |> Seq.filter (fun cell -> cell.isCenter() |> not)
                                   |> Seq.map (fun cell -> cell.id)
                                   |> Seq.toList
                     | _ -> List.empty