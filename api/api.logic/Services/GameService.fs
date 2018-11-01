﻿module Djambi.Api.Logic.Services.GameService

open System.Collections.Generic
open System.Linq
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.Logic.ModelExtensions
open Djambi.Api.Logic.ModelExtensions.BoardModelExtensions
open Djambi.Api.Logic.ModelExtensions.GameModelExtensions
open Djambi.Api.Model.BoardModel
open Djambi.Api.Model.GameModel
open Djambi.Api.Model.SessionModel

let getGameState(gameId : int) (session : Session) : GameState AsyncHttpResult =
    GameRepository.getGame gameId
    //TODO: Must be either
        //Admin
        //User in game
    |> thenMap (fun g -> g.gameState)
        
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

let selectCell(gameId : int, cellId : int) (session: Session) : TurnState AsyncHttpResult = 
    GameRepository.getGame gameId
    //TODO: Must be either
        //Admin
        //Current user/guest in game
    |> thenBind (fun game -> 
        if game.turnState.selectionOptions |> List.contains cellId |> not
        then Error <| HttpException(400, (sprintf "Cell %i is not currently selectable" cellId))
        else 
            let turn = game.turnState
            if turn.status = AwaitingConfirmation
            then Error <| HttpException(400, "Cannot make seletion when awaiting turn confirmation")
            else 
            let pieceIndex = game.gameState.piecesIndexedByCell
            let board = BoardModelUtility.getBoardMetadata game.regionCount
  
            match turn.selections.Length with
            | 0 -> Ok (Selection.subject(cellId, pieceIndex.Item(cellId).id), AwaitingSelection)
            | 1 -> let subject = turn.subjectPiece(game.gameState).Value
                   match pieceIndex.TryFind(cellId) with
                    | None -> match subject.pieceType with
                                | Reporter -> 
                                    if board.neighborsFromCellId cellId
                                        |> Seq.map (fun c -> pieceIndex.TryFind c.id)
                                        |> Seq.filter (fun o -> o.IsSome)
                                        |> Seq.map (fun o -> o.Value)
                                        |> Seq.filter (fun p -> p.isAlive && p.playerId <> subject.playerId)
                                        |> Seq.isEmpty
                                    then Ok (Selection.move(cellId), AwaitingConfirmation)
                                    else Ok (Selection.move(cellId), AwaitingSelection) //Awaiting target
                                | _ -> Ok <| (Selection.move(cellId), AwaitingConfirmation)
                    | Some target when subject.pieceType = Assassin ->
                        if board.cell(cellId).isCenter
                        then Ok (Selection.moveWithTarget(cellId, target.id), AwaitingSelection) //Awaiting vacate                                
                        else Ok (Selection.moveWithTarget(cellId, target.id), AwaitingConfirmation)
                    | Some piece -> Ok (Selection.moveWithTarget(cellId, piece.id), AwaitingSelection) //Awaiting drop
            | 2 -> match (turn.subjectPiece game.gameState).Value.pieceType with
                    | Thug
                    | Chief
                    | Diplomat
                    | Gravedigger -> Ok (Selection.drop(cellId), AwaitingConfirmation)
                    | Assassin -> Ok (Selection.move(cellId), AwaitingConfirmation) //Vacate
                    | Reporter -> Ok (Selection.target(cellId, pieceIndex.Item(cellId).id), AwaitingConfirmation)
                    | Corpse -> Error <| HttpException(400, "Subject cannot be corpse")
            | 3 -> match (turn.subjectPiece game.gameState).Value.pieceType with
                    | Diplomat
                    | Gravedigger -> Ok (Selection.move(cellId), AwaitingConfirmation) //Vacate
                    | _ -> Error <| HttpException(400, "Cannot make fourth selection unless vacating center")
            | _ -> Error <| HttpException(400, "Cannot make more than 4 selections")
                    
            |> Result.map (fun (selection, turnStatus) ->
                    let stateWithNewSelection =
                        {
                            status = turnStatus
                            selections = List.append turn.selections [selection]
                            selectionOptions = List.empty
                            requiredSelectionType = None
                        }
                    let updatedGame = 
                        {
                            gameState = game.gameState
                            turnState = stateWithNewSelection
                            regionCount = game.regionCount
                        }
                    let (selectionOptions, requiredSelectionType) = getSelectableCellsFromState updatedGame
                      
                    {
                    stateWithNewSelection with 
                        selectionOptions = selectionOptions
                        requiredSelectionType = requiredSelectionType
                    }))
    |> thenDoAsync (fun turnState -> GameRepository.updateTurnState(gameId, turnState))            

let resetTurn(gameId : int) (session : Session) : TurnState AsyncHttpResult =
    GameRepository.getGame gameId
    //TODO: Must be either
        //Admin
        //Current user/guest in game    
    |> thenMap (fun game -> 
        let updatedGame = 
            {
                gameState = game.gameState
                turnState = TurnState.empty
                regionCount = game.regionCount
            }
                
        let (selectionOptions, requiredSelectionType) = getSelectableCellsFromState updatedGame
            
        {
            updatedGame.turnState with 
                selectionOptions = selectionOptions
                requiredSelectionType = requiredSelectionType
        })
    |> thenDoAsync (fun turnState -> GameRepository.updateTurnState(gameId, turnState))
    
let private applyTurnStateToPieces(game : Game) : Piece list =
    let pieces = game.gameState.pieces.ToDictionary(fun p -> p.id)
        
    match (game.turnState.subjectPiece game.gameState, game.turnState.destinationCell game.regionCount) with
    | (None, _) 
    | (_, None) -> raise (HttpException(500, "Cannot commit turn without subject and destination selected."))
    | (Some subject, Some destination) -> 
        let origin = subject.cellId
                    
        //Move subject to destination or vacate cell
        match game.turnState.vacateCellId with
        | None ->        pieces.[subject.id] <- subject.moveTo destination.id
        | Some vacate -> pieces.[subject.id] <- subject.moveTo vacate
                    
        match game.turnState.targetPiece game.gameState with 
        | None -> ()
        | Some target -> 
            //Kill target 
            if subject.isKiller 
            then pieces.[target.id] <- target.kill

            //Enlist players pieces if killing chief
            if subject.isKiller && target.pieceType = Chief
            then for p in game.gameState.piecesControlledBy target.playerId.Value do
                    pieces.[p.id] <- pieces.[p.id].enlistBy subject.playerId.Value
                        
            //Drop target if drop cell exists                        
            match game.turnState.dropCellId with
            | Some drop ->  pieces.[target.id] <- pieces.[target.id].moveTo drop
            | None -> ()
                        
            //Move target back to origin if subject is assassin
            if subject.pieceType = Assassin
            then pieces.[target.id] <- pieces.[target.id].moveTo origin
        pieces.Values |> Seq.toList

let private removeSequentialDuplicates(turnCycle : int list) : int list =
    let list = turnCycle.ToList()
    for i in [(list.Count-1)..1] do 
        if list.[i] = list.[i-1]
        then list.RemoveAt(i)
    list |> Seq.toList

let private applyTurnStateToTurnCycle(game : Game) : int list =
    let mutable turns = game.gameState.turnCycle

    let removeBonusTurnsForPlayer playerId = 
        //Copy only the last turn of the given player, plus all other turns
        let stack = new Stack<int>()
        let mutable hasAddedTargetPlayer = false
        for t in turns |> Seq.rev do
            if t = playerId && not hasAddedTargetPlayer
            then 
                hasAddedTargetPlayer <- true
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

    match (game.turnState.subjectPiece game.gameState, game.turnState.targetPiece game.gameState, game.turnState.dropCell game.regionCount) with            
    //If chief being killed, remove all its turns
    | (Some subject, Some target, _) 
        when subject.isKiller && target.pieceType = Chief -> 
        turns <- turns |> Seq.filter(fun playerId -> playerId <> target.playerId.Value) |> Seq.toList
    //If chief being forced out of power, remove its bonus turns only
    | (Some subject, Some target, Some drop) 
        when subject.pieceType = Diplomat && target.pieceType = Chief && not drop.isCenter ->
        removeBonusTurnsForPlayer target.playerId.Value
    | _ -> ()

    match (game.turnState.subjectPiece game.gameState, game.turnState.subjectCell game.regionCount, game.turnState.destinationCell game.regionCount) with
    //If subject is chief in center and destination is not center, remove extra turns
    | (Some subject, Some origin, Some destination) 
        when subject.pieceType = Chief && origin.isCenter && not destination.isCenter ->
        removeBonusTurnsForPlayer subject.playerId.Value
    //If subject is chief and destination is center, add extra turns to queue
    | (Some subject, _, Some destination)
        when subject.pieceType = Chief && destination.isCenter ->
        addBonusTurnsForPlayer subject.playerId.Value
    | _ -> ()
            
    turns <- removeSequentialDuplicates turns

    //Cycle turn queue      
    turns <- List.append turns.Tail [turns.Head]

    turns

let private killCurrentPlayer(gameState : GameState) : GameState =
    let playerId = gameState.turnCycle.Head

    let pieces = gameState.pieces
                    |> List.map (fun p -> if p.playerId = Some(playerId) then p.abandon else p)
    let players = gameState.players
                    |> List.map (fun p -> if p.id = playerId then p.kill else p)

    let turns = gameState.turnCycle
                |> List.filter (fun t -> t <> playerId)
                |> removeSequentialDuplicates
    {
        pieces = pieces
        players = players
        turnCycle = turns
    }

let commitTurn(gameId : int) (session : Session) : CommitTurnResponse AsyncHttpResult =
    GameRepository.getGame gameId
    //TODO: Must be either
        //Admin
        //Current user/guest in game
    |> thenMap (fun game -> 
        let turnCycle = applyTurnStateToTurnCycle game
        let pieces = applyTurnStateToPieces game
                                        
        let mutable players = game.gameState.players

        //Kill player if chief killed
        match (game.turnState.subjectPiece game.gameState, game.turnState.targetPiece game.gameState) with
        | (Some subject, Some target) 
            when subject.isKiller && target.pieceType = Chief ->
            players <- players |> List.map (fun p -> if p.id = target.playerId.Value then p.kill else p)
        | _ -> ()

        let mutable updatedGame : Game = 
            {
                gameState = 
                    {
                        pieces = pieces
                        players = players
                        turnCycle = turnCycle
                    }
                turnState = TurnState.empty
                regionCount = game.regionCount
            }

        //While next player has no moves, kill chief and abandon all pieces
        let (options, reqSelType) = getSelectableCellsFromState updatedGame
        let mutable selectionOptions = options
        let mutable requiredSelectionType = reqSelType
                                
        while selectionOptions.IsEmpty && updatedGame.gameState.turnCycle.Length > 1 do
            updatedGame <- 
                {
                    gameState = killCurrentPlayer(updatedGame.gameState)
                    turnState = TurnState.empty
                    regionCount = game.regionCount
                }
            let (opt, rst) = getSelectableCellsFromState updatedGame
            selectionOptions <- opt
            requiredSelectionType <- rst

        //TODO: If only 1 player, game over
              
        {
            gameState = updatedGame.gameState
            turnState =  
            { 
                TurnState.empty with 
                    selectionOptions = selectionOptions 
                    requiredSelectionType = requiredSelectionType
            }
        })

    |> thenDoAsync (fun response -> GameRepository.updateGameState(gameId, response.gameState))
    |> thenDoAsync (fun response -> GameRepository.updateTurnState(gameId, response.turnState))