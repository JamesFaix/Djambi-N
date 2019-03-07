module Djambi.Api.Logic.Services.TurnService

open System.Collections.Generic
open System.Linq
open Djambi.Api.Common.Collections
open Djambi.Api.Common.Control
open Djambi.Api.Logic
open Djambi.Api.Logic.ModelExtensions
open Djambi.Api.Logic.ModelExtensions.BoardModelExtensions
open Djambi.Api.Logic.ModelExtensions.GameModelExtensions
open Djambi.Api.Logic.PieceStrategies
open Djambi.Api.Logic.Services
open Djambi.Api.Model

let getSubjectSelectionEventDetails (game : Game, cellId : int) : (Selection * TurnStatus * SelectionKind option) HttpResult =
    let pieces = game.piecesIndexedByCell
    match pieces.TryFind(cellId) with
    | None -> ErrorService.noPieceInCell()
    | Some p -> 
        let selection = Selection.subject(cellId, p.id)
        Ok (selection, AwaitingSelection, Some Move)

let getMoveSelectionEventDetails (game : Game, cellId : int) : (Selection * TurnStatus * SelectionKind option) HttpResult =
    let pieces = game.piecesIndexedByCell
    let board = BoardModelUtility.getBoardMetadata game.parameters.regionCount
    let subject = game.currentTurn.Value.subjectPiece(game).Value
    let subjectStrategy = PieceService.getStrategy subject
    match pieces.TryFind(cellId) with
    | None -> 
        let selection = Selection.move(cellId)
        if subjectStrategy.canTargetAfterMove
            && board.neighborsFromCellId cellId
                |> Seq.map (fun c -> pieces.TryFind c.id)
                |> Seq.filter (fun o -> o.IsSome)
                |> Seq.map (fun o -> o.Value)
                |> Seq.filter (fun p -> p.isAlive && p.playerId <> subject.playerId)
                |> (not << Seq.isEmpty)
        then Ok (selection, AwaitingSelection, Some Target)
        else Ok (selection, AwaitingCommit, None)
    | Some target ->
        let selection = Selection.moveWithTarget(cellId, target.id)
        if subjectStrategy.movesTargetToOrigin then
            match board.cell cellId with
            | None -> ErrorService.cellNotFound()
            | Some c when c.isCenter ->
                Ok (selection, AwaitingSelection, Some Vacate)
            | _ ->
                Ok (selection, AwaitingCommit, None)
        else Ok (selection, AwaitingSelection, Some Drop)

let getTargetSelectionEventDetails (game : Game, cellId : int) : (Selection * TurnStatus * SelectionKind option) HttpResult =
    let pieces = game.piecesIndexedByCell
    match pieces.TryFind(cellId) with
    | None -> ErrorService.noPieceInCell()
    | Some target ->
        let selection = Selection.target(cellId, target.id)
        Ok (selection, AwaitingCommit, None)

let getDropSelectionEventDetails (game : Game, cellId : int) : (Selection * TurnStatus * SelectionKind option) HttpResult =
    let turn = game.currentTurn.Value
    let subject = turn.subjectPiece(game).Value
    let destination = turn.destinationCell(game.parameters.regionCount).Value
    let subjectStrategy = PieceService.getStrategy subject
    let selection = Selection.drop(cellId)
    if subjectStrategy.canEnterSeatToEvictPiece 
        && (not subjectStrategy.canStayInCenter) 
        && destination.isCenter
    then Ok (selection, AwaitingSelection, Some Vacate)
    else Ok (selection, AwaitingCommit, None)

let getVacateSelectionEventDetails (game : Game, cellId : int) : (Selection * TurnStatus * SelectionKind option) HttpResult =
    let selection = Selection.vacate(cellId)
    Ok (selection, AwaitingCommit, None)

let getCellSelectedEvent(game : Game, cellId : int) (session: Session) : CreateEventRequest HttpResult =
    SecurityService.ensureAdminOrCurrentPlayer session game
    |> Result.bind (fun _ ->
        let board = BoardModelUtility.getBoardMetadata game.parameters.regionCount
        match board.cell cellId with
        | Some _ -> Ok ()
        | None -> ErrorService.cellNotFound()
    )
    |> Result.bind (fun _ ->
        let currentTurn = game.currentTurn.Value
        if currentTurn.selectionOptions |> List.contains cellId |> not
        then Error <| HttpException(400, (sprintf "Cell %i is not currently selectable." cellId))
        elif currentTurn.status <> AwaitingSelection || currentTurn.requiredSelectionKind.IsNone
        then ErrorService.turnStatusDoesNotAllowSelection()
        else
            match currentTurn.requiredSelectionKind with
            | None -> ErrorService.turnStatusDoesNotAllowSelection()
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
                SelectionOptionsService.getSelectableCellsFromState updatedGame
                |> Result.map (fun selectionOptions -> 
                    let turn =
                        if selectionOptions.IsEmpty && turn.status = AwaitingSelection
                        then Turn.deadEnd turn.selections
                        else { turn with selectionOptions = selectionOptions }
                                      
                    {
                        kind = EventKind.CellSelected
                        effects = [Effect.CurrentTurnChanged { oldValue = game.currentTurn; newValue = Some turn }]
                        createdByUserId = session.user.id     
                        actingPlayerId = ContextService.getActingPlayerId session game
                    }
                )                
            )
    )

let private applyTurnToPieces(game : Game) : (Piece list * Effect list) =
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
            //Kill target
            if subject.isKiller
            then 
                pieces.[target.id] <- target.kill
                effects.Add(Effect.PieceKilled { 
                    oldPiece = target
                })

            //Enlist players pieces if killing chief
            if subject.isKiller && target.kind = Chief
            then 
                let enlistedPieces = game.piecesControlledBy target.playerId.Value
                for p in enlistedPieces do
                    pieces.[p.id] <- pieces.[p.id].enlistBy subject.playerId.Value
                    effects.Add(Effect.PieceEnlisted{
                                oldPiece = p
                                newPlayerId = subject.playerId
                            })

            //Drop target if drop cell exists
            match currentTurn.dropCellId with
            | Some dropCellId ->  
                pieces.[target.id] <- pieces.[target.id].moveTo dropCellId
                effects.Add(Effect.PieceDropped { oldPiece = target; newCellId = dropCellId })
            | None -> ()

            //Move target back to origin if subject is assassin
            if subject.kind = Assassin
            then 
                pieces.[target.id] <- pieces.[target.id].moveTo originCellId
                effects.Add(Effect.PieceDropped { oldPiece = target; newCellId = originCellId })
        (pieces.Values |> Seq.toList, effects |> Seq.toList)

let private removeSequentialDuplicates(turnCycle : int list) : int list =
    if turnCycle.Length = 1 then turnCycle
    else
        let list = turnCycle.ToList()
        for i in [(list.Count-1)..1] do
            if list.[i] = list.[i-1]
            then list.RemoveAt(i)
        list |> Seq.toList

let private applyTurnToTurnCycle(game : Game) : (int list * Effect list) =
    let mutable turns = game.turnCycle
    let currentTurn = game.currentTurn.Value
    let regions = game.parameters.regionCount
    let effects = ArrayList<Effect>()

    let removeAllTurnsForPlayer playerId = 
        let newTurns = turns |> Seq.filter(fun playerId -> playerId <> playerId) |> Seq.toList |> removeSequentialDuplicates
        effects.Add(Effect.TurnCyclePlayerRemoved { oldValue = turns; newValue = newTurns; playerId = playerId })
        turns <- newTurns

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
        let newTurns = stack |> Seq.toList |> removeSequentialDuplicates
        effects.Add(Effect.TurnCyclePlayerFellFromPower { oldValue = turns; newValue = newTurns; playerId = playerId })
        turns <- newTurns

    let addBonusTurnsForPlayer playerId =
        //Insert a turn for the given player before every turn, except the current one
        let stack = new Stack<int>()
        for t in turns |> Seq.skip(1) |> Seq.rev do
            stack.Push t
            stack.Push playerId
        let newTurns = stack |> Seq.toList |> removeSequentialDuplicates
        effects.Add(Effect.TurnCyclePlayerRoseToPower { oldValue = turns; newValue = newTurns; playerId = playerId })
        turns <- newTurns

    match (currentTurn.subjectPiece game, currentTurn.targetPiece game, currentTurn.dropCell regions) with
    //If chief being killed, remove all its turns
    | (Some subject, Some target, _)
        when subject.isKiller && target.kind = Chief ->
        removeAllTurnsForPlayer target.playerId.Value

    //If chief being forced out of power, remove its bonus turns only
    | (Some subject, Some target, Some drop)
        when subject.kind = Diplomat && target.kind = Chief && not drop.isCenter ->
        removeBonusTurnsForPlayer target.playerId.Value
    | _ -> ()

    match (currentTurn.subjectPiece game, currentTurn.subjectCell regions, currentTurn.destinationCell regions) with
    //If subject is chief in center and destination is not center, remove extra turns
    | (Some subject, Some origin, Some destination)
        when subject.kind = Chief && origin.isCenter && not destination.isCenter ->
        removeBonusTurnsForPlayer subject.playerId.Value

    //If subject is chief and destination is center, add extra turns to queue
    | (Some subject, _, Some destination)
        when subject.kind = Chief && destination.isCenter ->
        addBonusTurnsForPlayer subject.playerId.Value
    | _ -> ()

    //Cycle turn queue
    let newTurns = List.append turns.Tail [turns.Head]
    effects.Add(Effect.TurnCycleAdvanced { oldValue = turns; newValue = newTurns })
    turns <- newTurns

    (turns, effects |> Seq.toList)

let private killCurrentPlayer(game : Game) : (Game * Effect list) =
    let playerId = game.turnCycle.Head

    let pieces =
        game.pieces 
        |> List.replaceIf
            (fun p -> p.playerId = Some(playerId))
            (fun p -> p.abandon)

    let players = 
        game.players
        |> List.replaceIf
            (fun p -> p.id = playerId)
            (fun p -> p.kill)

    let turns = 
        game.turnCycle
        |> List.filter (fun t -> t <> playerId)
        |> removeSequentialDuplicates

    let effects = new ArrayList<Effect>()
    effects.Add(Effect.PlayerOutOfMoves { playerId = playerId })
    effects.Add(Effect.TurnCyclePlayerRemoved { oldValue = game.turnCycle; newValue = turns; playerId = playerId })

    let abandonedPieces = game.pieces |> List.filter (fun p -> p.playerId = Some playerId)

    for p in abandonedPieces do
        effects.Add(Effect.PieceAbandoned { oldPiece = p })

    let updatedGame =
        { game with
            pieces = pieces
            players = players
            turnCycle = turns
            currentTurn = Some Turn.empty
        }

    (updatedGame, effects |> Seq.toList)

let private detectPlayersOutOfMoves(game : Game, effects : Effect seq) : Game * Effect list * int list =
    //While next player has no moves, kill chief and abandon all pieces
    let mutable game = game
    let mutable selectionOptions = (SelectionOptionsService.getSelectableCellsFromState game) |> Result.value
    let effects = new ArrayList<Effect>(effects)

    while selectionOptions.IsEmpty && game.turnCycle.Length > 1 do
        let (g, fx) = killCurrentPlayer game
        effects.AddRange(fx)
        game <- g
        selectionOptions <- (SelectionOptionsService.getSelectableCellsFromState) game |> Result.value

    (game, effects |> Seq.toList, selectionOptions)

let getCommitTurnEvent(game : Game) (session : Session) : CreateEventRequest HttpResult =
    SecurityService.ensureAdminOrCurrentPlayer session game
    |> Result.map (fun _ ->
        let effects = new ArrayList<Effect>()
        
        let (turnCycle, turnEffects) = applyTurnToTurnCycle game
        effects.AddRange(turnEffects)

        let (pieces, pieceEffects) = applyTurnToPieces game
        effects.AddRange(pieceEffects)

        let currentTurn = game.currentTurn.Value
        
        let mutable players = game.players

        //Kill player if chief killed
        match (currentTurn.subjectPiece game, currentTurn.targetPiece game) with
        | (Some subject, Some target) 
            when subject.isKiller && target.kind = Chief ->
                effects.Add(Effect.PlayerEliminated { playerId = target.playerId.Value })
                players <- players |> List.map (fun p -> if p.id = target.playerId.Value then p.kill else p)
        | _ -> ()

        let updatedGame =
            { game with 
                pieces = pieces
                turnCycle = turnCycle
                players = players
                currentTurn = Some Turn.empty
            }

        let (updatedGame, effects, selectionOptions) = detectPlayersOutOfMoves(updatedGame, effects)
        let effects = new ArrayList<Effect>(effects)

        //TODO: If only 1 player, game over
        let updatedTurn = { Turn.empty with selectionOptions = selectionOptions }

        effects.Add(Effect.CurrentTurnChanged { 
            oldValue = game.currentTurn; 
            newValue = Some updatedTurn 
        })

        {
            kind = EventKind.TurnCommitted
            effects = effects |> Seq.toList
            createdByUserId = session.user.id
            actingPlayerId = ContextService.getActingPlayerId session game
        }
    )

let getResetTurnEvent(game : Game) (session : Session) : CreateEventRequest HttpResult =
    SecurityService.ensureAdminOrCurrentPlayer session game
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