namespace Djambi.Api.Domain

open Giraffe
open System.Threading.Tasks
open Djambi.Api.Persistence
open Djambi.Api.Domain.PlayModels
open Djambi.Api.Domain.PlayModelExtensions
open Djambi.Api.Domain.BoardsExtensions
open Djambi.Api.Common.Enums
open Djambi.Api.Common
open Djambi.Api.Domain.BoardModels

type PlayService(repository : PlayRepository) =

    member this.getGameState(gameId : int) : GameState Task =
        task {
            let! game = repository.getGame gameId
            return game.currentGameState
        }

    member this.getSelectableCells(gameId : int) : int list Task =
        task {
            let! game = repository.getGame gameId
            return this.getSelectableCellsFromState(game.currentGameState, game.currentTurnState, game.boardRegionCount)
        }

    member private this.getSelectableCellsFromState(gameState : GameState, turnState : TurnState, regionCount : int) : int list =
        let currentPlayerId = gameState.currentPlayerId
        match turnState.selections.Length with
        | 0 -> gameState.piecesControlledBy currentPlayerId
                |> Seq.map (fun piece -> (piece, this.getMoveSelectionOptions(gameState, piece, regionCount)))
                |> Seq.filter (fun (_, cells) -> cells.IsEmpty |> not)
                |> Seq.map (fun (piece, _) -> piece.cellId)
                |> Seq.toList
        | 1 -> let subject = turnState.getSubjectPiece(gameState).Value
               this.getMoveSelectionOptions(gameState, subject, regionCount)
        | 2 -> let subject = turnState.getSubjectPiece(gameState).Value
               match (subject.pieceType, turnState.selections.[1]) with 
                | Reporter, Move _ -> this.getTargetSelectionOptions(gameState, turnState, regionCount)
                | Thug, MoveWithTarget (_,_) 
                | Chief, MoveWithTarget (_,_) 
                | Diplomat, MoveWithTarget (_,_)
                | Gravedigger, MoveWithTarget (_,_) -> this.getDropSelectionOptions(gameState, turnState, regionCount)
                | Assassin, MoveWithTarget (_,_) -> this.getVacateSelectionOptions(gameState, turnState, regionCount)
                | _ -> List.empty
        | 3 -> let subject = turnState.getSubjectPiece(gameState).Value
               match (subject.pieceType, turnState.selections.[1]) with 
                | Diplomat, MoveWithTarget (_,_)
                | Gravedigger, MoveWithTarget (_,_) -> this.getVacateSelectionOptions(gameState, turnState, regionCount)
                | _ -> List.empty
        | _ -> List.empty

    member private this.getMoveSelectionOptions(game : GameState, piece : Piece, regionCount : int) : int list =
        let board = BoardUtility.getBoardMetadata regionCount
        let paths = board.pathsFromCellId piece.cellId
        let pieceIndex = game.piecesIndexedByCell

        let cellIsEmptyOrContainsEnemy(cell : Cell) =
            match pieceIndex.TryFind cell.id with
            | None -> true
            | Some occupant -> occupant.isAlive && occupant.playerId <> piece.playerId

        let cellIsNotCenterOrIsCenterWithChief(cell : Cell) =
             if cell.isCenter() |> not
             then true
             else match pieceIndex.TryFind cell.id with
                  | None -> false
                  | Some occupant -> occupant.pieceType = Chief

        match piece.pieceType with
        | Chief -> paths 
                   |> Seq.collect (fun path -> path |> Seq.takeWhile cellIsEmptyOrContainsEnemy)
                   |> Seq.map (fun cell -> cell.id)
                   |> Seq.toList
        | Thug ->  paths 
                   |> Seq.collect 
                        (fun path -> path 
                                     |> Seq.take (min 2 path.Length)
                                     |> Seq.takeWhile cellIsEmptyOrContainsEnemy)
                   |> Seq.filter (fun cell -> cell.isCenter() |> not)
                   |> Seq.map (fun cell -> cell.id)
                   |> Seq.toList
        | Assassin -> paths 
                       |> Seq.collect (fun path -> path |> Seq.takeWhile cellIsEmptyOrContainsEnemy)
                       |> Seq.filter cellIsNotCenterOrIsCenterWithChief
                       |> Seq.map (fun cell -> cell.id)
                       |> Seq.toList
        | Reporter -> paths 
                       |> Seq.collect (fun path -> path |> Seq.takeWhile cellIsEmptyOrContainsEnemy)
                       |> Seq.filter (fun cell -> cell.isCenter() |> not)
                       |> Seq.map (fun cell -> cell.id)
                       |> Seq.toList
        | Diplomat -> paths 
                       |> Seq.collect (fun path -> path |> Seq.takeWhile cellIsEmptyOrContainsEnemy)
                       |> Seq.filter cellIsNotCenterOrIsCenterWithChief
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
        
    member private this.getTargetSelectionOptions(game : GameState, turn : TurnState, regionCount : int) : int list =
        match turn.getDestinationCellId with
        | None -> List.empty
        | Some destinationCellId -> 
            match turn.getSubjectPiece game with
            | None -> list.Empty
            | Some subject ->
                match subject.pieceType with
                | Reporter -> 
                    let board = BoardUtility.getBoardMetadata regionCount
                    let neighbors = board.neighborsFromCellId destinationCellId
                    let pieceIndex = game.piecesIndexedByCell
                    neighbors |> Seq.filter (fun cell -> match pieceIndex.TryFind cell.id with
                                                            | None -> false
                                                            | Some piece -> piece.playerId <> subject.playerId)
                              |> Seq.map (fun cell -> cell.id)
                              |> Seq.toList
                | _ -> List.Empty

    member private this.getDropSelectionOptions(game : GameState, turn : TurnState, regionCount : int) : int list =
        match turn.getSubjectPiece game with
        | None -> List.empty
        | Some subject -> 
            match turn.getDestinationCellId with
            | None -> List.empty
            | Some _ -> 
                let pieceIndex = game.piecesIndexedByCell
                let board = BoardUtility.getBoardMetadata regionCount
                board.cells()
                |> Seq.filter (fun c -> match pieceIndex.TryFind c.id with
                                        | None -> true
                                        | Some occupant -> occupant.id = subject.id)
                |> Seq.map (fun c -> c.id)
                |> Seq.toList

    member private this.getVacateSelectionOptions(game : GameState, turn : TurnState, regionCount : int) : int list =
        match turn.getSubjectPiece game with
        | None -> List.empty
        | Some subject -> 
            match turn.getDestinationCell regionCount with
            | None -> List.empty
            | Some destination -> 
                if destination.isCenter() |> not
                then List.empty
                else match subject.pieceType with
                     | Assassin
                     | Gravedigger 
                     | Diplomat -> let board = BoardUtility.getBoardMetadata regionCount
                                   let pieceIndex = game.piecesIndexedByCell       
                                   board.pathsFromCell destination
                                   |> Seq.collect (fun path -> path |> Seq.takeWhile (fun cell -> (pieceIndex.TryFind cell.id).IsNone))
                                   |> Seq.filter (fun cell -> cell.isCenter() |> not)
                                   |> Seq.map (fun cell -> cell.id)
                                   |> Seq.toList
                     | _ -> List.empty

    member this.selectCell(gameId : int, cellId : int) : Result<TurnState, HttpError> Task = 
        task {
            let! selectableCells = this.getSelectableCells gameId
            if selectableCells |> List.contains cellId |> not
            then return Error({ statusCode = 400; message = sprintf "Cell %i is not currently selectable" cellId })
            else let! game = repository.getGame gameId 
                 let turn = game.currentTurnState
                 if turn.status = AwaitingConfirmation
                 then return Error({statusCode = 400; message = "Cannot make seletion when awaiting turn confirmation"})
                 else let pieceIndex = game.currentGameState.piecesIndexedByCell
                      let board = BoardUtility.getBoardMetadata game.boardRegionCount
                      
                      let result : Result<(Selection * TurnStatus), HttpError> = 
                        match turn.selections.Length with
                        | 0 -> Ok(Subject(pieceIndex.Item(cellId).id), AwaitingSelection)
                        | 1 -> let subject = turn.getSubjectPiece(game.currentGameState).Value
                               match pieceIndex.TryFind(cellId) with
                                | None -> match subject.pieceType with
                                          | Reporter -> if board.neighborsFromCellId cellId
                                                            |> Seq.map (fun c -> pieceIndex.TryFind c.id)
                                                            |> Seq.filter (fun o -> o.IsSome)
                                                            |> Seq.map (fun o -> o.Value)
                                                            |> Seq.filter (fun p -> p.isAlive && p.playerId <> subject.playerId)
                                                            |> Seq.isEmpty
                                                        then Ok(Move(cellId), AwaitingConfirmation)
                                                        else Ok(Move(cellId), AwaitingSelection) //Awaiting target
                                          | _ -> Ok(Move(cellId), AwaitingConfirmation)
                                | Some target when subject.pieceType = Assassin ->
                                    if board.cell(cellId).isCenter()
                                    then Ok(MoveWithTarget(cellId, target.id), AwaitingSelection) //Awaiting vacate                                
                                    else Ok(MoveWithTarget(cellId, target.id), AwaitingConfirmation)
                                | Some piece -> Ok(MoveWithTarget(cellId, piece.id), AwaitingSelection) //Awaiting drop
                        | 2 -> match (turn.getSubjectPiece game.currentGameState).Value.pieceType with
                                | Thug
                                | Chief
                                | Diplomat
                                | Gravedigger -> Ok(Drop(cellId), AwaitingConfirmation)
                                | Assassin -> Ok(Move(cellId), AwaitingConfirmation) //Vacate
                                | Reporter -> Ok(Target(pieceIndex.Item(cellId).id), AwaitingConfirmation) 
                        | 3 -> match (turn.getSubjectPiece game.currentGameState).Value.pieceType with
                                | Diplomat
                                | Gravedigger -> Ok(Move(cellId), AwaitingConfirmation) //Vacate
                                | _ -> Error({ statusCode = 400; message = "Cannot make fourth selection unless vacating center" })
                        | _ -> Error({ statusCode = 400; message = "Cannot make more than 4 selections" })
                      
                      match result with
                      | Error msg -> return Error(msg)
                      | Ok (selection, status) -> 
                              let stateWithNewSelection =
                                {
                                    status = status
                                    selections = turn.selections |> List.append [selection]
                                    selectionOptions = List.empty
                                }
                              let stateWithSelectionOptions = 
                                {
                                    stateWithNewSelection with 
                                        selectionOptions = this.getSelectableCellsFromState(game.currentGameState, stateWithNewSelection, game.boardRegionCount)
                                }
                              let! _ = repository.updateCurrentTurnState(gameId, stateWithSelectionOptions)
                              return Ok stateWithSelectionOptions
        }

    member private this.emptyTurn : TurnState =
        {
            status = TurnStatus.AwaitingSelection
            selections = List.empty
            selectionOptions = List.empty
        }

    member this.resetTurn(gameId : int) : TurnState Task =
        task {            
            let! game = repository.getGame gameId
            
            let emptyTurn = this.emptyTurn

            let selectionOptions = this.getSelectableCellsFromState(game.currentGameState, emptyTurn, game.boardRegionCount)

            let turnWithSelectionOptions = 
                { emptyTurn with selectionOptions = selectionOptions }

            let! _ = repository.updateCurrentTurnState(gameId, turnWithSelectionOptions)

            return turnWithSelectionOptions
        }

    //member this.commitTurn(gameId : int) : Game Task =
    //    task {
    //        let! game = repository.getGame gameId

            

            
    //    }