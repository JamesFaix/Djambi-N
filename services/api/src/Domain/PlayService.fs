namespace Djambi.Api.Domain

open Giraffe
open System.Collections.Generic
open System.Linq
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
            let (cellIds, _) = this.getSelectableCellsFromState game
            return cellIds
        }

    member private this.getSelectableCellsFromState(game : Game) : (int list * SelectionType option) =
        let gameState = game.currentGameState
        let turnState = game.currentTurnState
        let regionCount = game.boardRegionCount
        let currentPlayerId = gameState.currentPlayerId
        match turnState.selections.Length with
        | 0 -> let cellIds = gameState.piecesControlledBy currentPlayerId
                                |> Seq.map (fun piece -> (piece, this.getMoveSelectionOptions(gameState, piece, regionCount)))
                                |> Seq.filter (fun (_, cells) -> cells.IsEmpty |> not)
                                |> Seq.map (fun (piece, _) -> piece.cellId)
                                |> Seq.toList
               (cellIds, Some Subject)
        | 1 -> let subject = turnState.subjectPiece(gameState).Value
               let cellIds = this.getMoveSelectionOptions(gameState, subject, regionCount)
               (cellIds, Some Move)
        | 2 -> let subject = turnState.subjectPiece(gameState).Value
               match (subject.pieceType, turnState.selections.[1]) with 
                | Reporter, sel -> 
                    let cellIds = this.getTargetSelectionOptions(gameState, turnState, regionCount)
                    (cellIds, Some Target)
                | Thug, sel 
                | Chief, sel
                | Diplomat, sel
                | Gravedigger, sel when sel.pieceId.IsSome -> 
                    let cellIds = this.getDropSelectionOptions(gameState, turnState, regionCount)
                    (cellIds, Some Drop)
                | Assassin, sel when sel.pieceId.IsSome -> 
                    let cellIds = this.getVacateSelectionOptions(gameState, turnState, regionCount)
                    (cellIds, Some Vacate)
                | _ -> (List.empty, None)
        | 3 -> let subject = turnState.subjectPiece(gameState).Value
               match (subject.pieceType, turnState.selections.[1]) with 
                | Diplomat, sel
                | Gravedigger, sel when sel.pieceId.IsSome -> 
                    let cellIds = this.getVacateSelectionOptions(gameState, turnState, regionCount)
                    (cellIds, Some Vacate)
                | _ -> (List.empty, None)
        | _ -> (List.empty, None)

    member private this.getMoveSelectionOptions(game : GameState, piece : Piece, regionCount : int) : int list =
        let board = BoardUtility.getBoardMetadata regionCount
        let paths = board.pathsFromCellId piece.cellId
        let pieceIndex = game.piecesIndexedByCell

        let cellIsEmptyOrContainsEnemy(cell : Cell) =
            match pieceIndex.TryFind cell.id with
            | None -> true
            | Some occupant -> occupant.isAlive && occupant.playerId <> piece.playerId

        let cellIsNotCenterOrIsCenterWithChief(cell : Cell) =
             if not cell.isCenter
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
                   |> Seq.filter (fun cell -> not cell.isCenter)
                   |> Seq.map (fun cell -> cell.id)
                   |> Seq.toList
        | Assassin -> paths 
                       |> Seq.collect (fun path -> path |> Seq.takeWhile cellIsEmptyOrContainsEnemy)
                       |> Seq.filter cellIsNotCenterOrIsCenterWithChief
                       |> Seq.map (fun cell -> cell.id)
                       |> Seq.toList
        | Reporter -> paths 
                       |> Seq.collect (fun path -> path |> Seq.takeWhile cellIsEmptyOrContainsEnemy)
                       |> Seq.filter (fun cell -> not cell.isCenter)
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
                       |> Seq.filter (fun cell -> if not cell.isCenter
                                                  then true
                                                  else match pieceIndex.TryFind cell.id with
                                                       | None -> false
                                                       | Some occupant -> occupant.isAlive |> not)
                       |> Seq.map (fun cell -> cell.id)
                       |> Seq.toList
        | _ -> List.empty

    member private this.getTargetSelectionOptions(game : GameState, turn : TurnState, regionCount : int) : int list =
        match turn.destinationCellId with
        | None -> List.empty
        | Some destinationCellId -> 
            match turn.subjectPiece game with
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
        match turn.subjectPiece game with
        | None -> List.empty
        | Some subject -> 
            match turn.destinationCellId with
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
                     | Diplomat -> let board = BoardUtility.getBoardMetadata regionCount
                                   let pieceIndex = game.piecesIndexedByCell       
                                   board.pathsFromCell destination
                                   |> Seq.collect (fun path -> path |> Seq.takeWhile (fun cell -> (pieceIndex.TryFind cell.id).IsNone))
                                   |> Seq.filter (fun cell -> not cell.isCenter)
                                   |> Seq.map (fun cell -> cell.id)
                                   |> Seq.toList
                     | _ -> List.empty

    member this.selectCell(gameId : int, cellId : int) : Result<TurnState, HttpError> Task = 
        task {
            let! game = repository.getGame gameId
            let (selectableCells, _) = this.getSelectableCellsFromState game
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
                        | 0 -> Ok(Selection.subject(cellId, pieceIndex.Item(cellId).id), AwaitingSelection)
                        | 1 -> let subject = turn.subjectPiece(game.currentGameState).Value
                               match pieceIndex.TryFind(cellId) with
                                | None -> match subject.pieceType with
                                          | Reporter -> if board.neighborsFromCellId cellId
                                                            |> Seq.map (fun c -> pieceIndex.TryFind c.id)
                                                            |> Seq.filter (fun o -> o.IsSome)
                                                            |> Seq.map (fun o -> o.Value)
                                                            |> Seq.filter (fun p -> p.isAlive && p.playerId <> subject.playerId)
                                                            |> Seq.isEmpty
                                                        then Ok(Selection.move(cellId), AwaitingConfirmation)
                                                        else Ok(Selection.move(cellId), AwaitingSelection) //Awaiting target
                                          | _ -> Ok(Selection.move(cellId), AwaitingConfirmation)
                                | Some target when subject.pieceType = Assassin ->
                                    if board.cell(cellId).isCenter
                                    then Ok(Selection.moveWithTarget(cellId, target.id), AwaitingSelection) //Awaiting vacate                                
                                    else Ok(Selection.moveWithTarget(cellId, target.id), AwaitingConfirmation)
                                | Some piece -> Ok(Selection.moveWithTarget(cellId, piece.id), AwaitingSelection) //Awaiting drop
                        | 2 -> match (turn.subjectPiece game.currentGameState).Value.pieceType with
                                | Thug
                                | Chief
                                | Diplomat
                                | Gravedigger -> Ok(Selection.drop(cellId), AwaitingConfirmation)
                                | Assassin -> Ok(Selection.move(cellId), AwaitingConfirmation) //Vacate
                                | Reporter -> Ok(Selection.target(cellId, pieceIndex.Item(cellId).id), AwaitingConfirmation)
                                | Corpse -> Error({ statusCode = 400; message = "Subject cannot be corpse" })
                        | 3 -> match (turn.subjectPiece game.currentGameState).Value.pieceType with
                                | Diplomat
                                | Gravedigger -> Ok(Selection.move(cellId), AwaitingConfirmation) //Vacate
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
                                    requiredSelectionType = None
                                }
                            let updatedGame = 
                                {
                                    currentGameState = game.currentGameState
                                    currentTurnState = stateWithNewSelection
                                    boardRegionCount = game.boardRegionCount
                                }
                            let (selectionOptions, requiredSelectionType) = this.getSelectableCellsFromState updatedGame
                            let updatedState =
                                {
                                    stateWithNewSelection with 
                                        selectionOptions = selectionOptions
                                        requiredSelectionType = requiredSelectionType
                                }
                            let! _ = repository.updateCurrentTurnState(gameId, updatedState)
                            return Ok updatedState
        }

    member private this.emptyTurn : TurnState =
        {
            status = TurnStatus.AwaitingSelection
            selections = List.empty
            selectionOptions = List.empty
            requiredSelectionType = Some Subject
        }

    member this.resetTurn(gameId : int) : TurnState Task =
        task {            
            let! game = repository.getGame gameId
            
            let updatedGame = 
                {
                    currentGameState = game.currentGameState
                    currentTurnState = this.emptyTurn
                    boardRegionCount = game.boardRegionCount
                }
                
            let (selectionOptions, requiredSelectionType) = this.getSelectableCellsFromState updatedGame
            let updatedState =
                {
                    updatedGame.currentTurnState with 
                        selectionOptions = selectionOptions
                        requiredSelectionType = requiredSelectionType
                }

            let! _ = repository.updateCurrentTurnState(gameId, updatedState)

            return updatedState
        }
    
    member private this.applyTurnStateToPieces(game : Game) : Result<Piece list, HttpError> =
        let gameState = game.currentGameState
        let turn = game.currentTurnState
        let regionCount = game.boardRegionCount        
        let pieces = gameState.pieces.ToDictionary(fun p -> p.id)
        
        match (turn.subjectPiece gameState, turn.destinationCell regionCount) with
        | (None, _) 
        | (_, None) -> Error({ statusCode = 500; message = "Cannot commit turn without subject selected." })
        | (Some subject, Some destination) -> 
            let origin = subject.cellId
                    
            //Move subject to destination or vacate cell
            match turn.vacateCellId with
            | None ->        pieces.[subject.id] <- subject.moveTo destination.id
            | Some vacate -> pieces.[subject.id] <- subject.moveTo vacate
                    
            match turn.targetPiece gameState with 
            | None -> ()
            | Some target -> 
                //Kill target 
                if subject.isKiller 
                then pieces.[target.id] <- target.kill

                //Enlist players pieces if killing chief
                if subject.isKiller && target.pieceType = Chief
                then for p in gameState.piecesControlledBy target.playerId.Value do
                        pieces.[p.id] <- p.enlistBy subject.playerId.Value
                        
                //Drop target if drop cell exists                        
                match turn.dropCellId with
                | Some drop ->  pieces.[target.id] <- target.moveTo drop
                | None -> ()
                        
                //Move target back to origin if subject is assassin
                if subject.pieceType = Assassin
                then pieces.[target.id] <- subject.moveTo origin
            Ok()
        |> Result.map(fun _ -> pieces.Values |> Seq.toList )

    member private this.removeSequentialDuplicates(turnCycle : int list) : int list =
        let list = turnCycle.ToList()
        for i in [(list.Count-1)..1] do 
            if list.[i] = list.[i-1]
            then list.RemoveAt(i)
        list |> Seq.toList

    member private this.applyTurnStateToTurnCycle(game : Game) : int list =
        let gameState = game.currentGameState
        let turn = game.currentTurnState
        let regionCount = game.boardRegionCount        
        let mutable turns = gameState.turnCycle

        let removeBonusTurnsForPlayer playerId = 
            //Copy only the last turn of the given player, plus all other turns
            let stack = new Stack<int>()
            let mutable hasAddedTargetPlayer = false
            for t in turns |> Seq.rev do
                if t = playerId && not hasAddedTargetPlayer
                then hasAddedTargetPlayer <- true
                     stack.Push t
                else stack.Push t
            turns <- stack |> Seq.toList

        let addBonusTurnsForPlayer playerId =
            //Insert a turn for the given player before every turn, except the current one
            let stack = new Stack<int>()
            for t in turns |> Seq.skip(1) |> Seq.rev do
                stack.Push t
                stack.Push playerId
            turns <- stack |> Seq.toList

        match (turn.subjectPiece gameState, turn.targetPiece gameState, turn.dropCell regionCount) with            
        //If chief being killed, remove all its turns
        | (Some subject, Some target, _) 
            when subject.isKiller && target.pieceType = Chief -> 
            turns <- turns |> Seq.filter(fun playerId -> playerId <> target.playerId.Value) |> Seq.toList
        //If chief being forced out of power, remove its bonus turns only
        | (Some subject, Some target, Some drop) 
            when subject.pieceType = Diplomat && target.pieceType = Chief && not drop.isCenter ->
            removeBonusTurnsForPlayer target.playerId.Value
        | _ -> ()

        match (turn.subjectPiece gameState, turn.subjectCell regionCount, turn.destinationCell regionCount) with
        //If subject is chief in center and destination is not center, remove extra turns
        | (Some subject, Some origin, Some destination) 
            when subject.pieceType = Chief && origin.isCenter && not destination.isCenter ->
            removeBonusTurnsForPlayer subject.playerId.Value
        //If subject is chief and destination is center, add extra turns to queue
        | (Some subject, _, Some destination)
            when subject.pieceType = Chief && destination.isCenter ->
            addBonusTurnsForPlayer subject.playerId.Value
        | _ -> ()
            
        turns <- this.removeSequentialDuplicates turns

        //Cycle turn queue      
        turns <- List.append turns.Tail [turns.Head]

        turns

    member private this.killCurrentPlayer(gameState : GameState) : GameState =
        let playerId = gameState.turnCycle.Head

        let pieces = gameState.pieces
                     |> List.map (fun p -> if p.playerId = Some(playerId) then p.abandon else p)
        let players = gameState.players
                      |> List.map (fun p -> if p.id = playerId then p.kill else p)

        let turns = gameState.turnCycle
                    |> List.filter (fun t -> t <> playerId)
                    |> this.removeSequentialDuplicates
        {
            pieces = pieces
            players = players
            turnCycle = turns
        }

    member this.commitTurn(gameId : int) : Result<CommitTurnResponse, HttpError> Task =
        task {
            let! game = repository.getGame gameId
            let gameState = game.currentGameState
            let turnState = game.currentTurnState
            let turnCycle = this.applyTurnStateToTurnCycle(game)
            let result = this.applyTurnStateToPieces(game)
                           |> Result.map(fun pieces -> 
                                        
                                let mutable players = game.currentGameState.players

                                //Kill player if chief killed
                                match (turnState.subjectPiece gameState, turnState.targetPiece gameState) with
                                | (Some subject, Some target) 
                                    when subject.isKiller && target.pieceType = Chief ->
                                    players <- players |> List.map (fun p -> if p.id = target.playerId.Value then p.kill else p)
                                | _ -> ()

                                let mutable updatedGame : Game = 
                                    {
                                        currentGameState = 
                                            {
                                                pieces = pieces
                                                players = players
                                                turnCycle = turnCycle
                                            }
                                        currentTurnState = this.emptyTurn
                                        boardRegionCount = game.boardRegionCount
                                    }

                                //While next player has no moves, kill chief and abandon all pieces
                                let mutable (selectionOptions, requiredSelectionType) = this.getSelectableCellsFromState updatedGame
                                while selectionOptions.IsEmpty && updatedGame.currentGameState.turnCycle.Length > 1 do
                                    updatedGame <- 
                                        {
                                            currentGameState = this.killCurrentPlayer(updatedGame.currentGameState)
                                            currentTurnState = this.emptyTurn
                                            boardRegionCount = game.boardRegionCount
                                        }
                                    (selectionOptions, requiredSelectionType) <- this.getSelectableCellsFromState updatedGame

                                //TODO: If only 1 player, game over
  
                                let response =
                                    {
                                        gameState = updatedGame.currentGameState
                                        turnState =  
                                        { 
                                            this.emptyTurn with 
                                                selectionOptions = selectionOptions 
                                                requiredSelectionType = requiredSelectionType
                                        }
                                    }
                            
                                response)

            match result with 
            | Ok(response) ->
                let! _ = repository.updateCurrentGameState(gameId, response.gameState)
                let! _ = repository.updateCurrentTurnState(gameId, response.turnState)
                ()
            | _ -> ()

            return result
        }