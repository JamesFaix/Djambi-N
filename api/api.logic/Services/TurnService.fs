module Djambi.Api.Logic.Services.TurnService

open System.Collections.Generic
open System.Linq
open Djambi.Api.Common.Collections
open Djambi.Api.Common.Control
open Djambi.Api.Logic.ModelExtensions
open Djambi.Api.Logic.ModelExtensions.BoardModelExtensions
open Djambi.Api.Logic.ModelExtensions.GameModelExtensions
open Djambi.Api.Model

let getCellSelectedEvent(game : Game, cellId : int) (session: Session) : CreateEventRequest HttpResult =
    SecurityService.ensureAdminOrCurrentPlayer session game
    |> Result.bind (fun _ ->
        let board = BoardModelUtility.getBoardMetadata game.parameters.regionCount
        match board.cell cellId with
        | Some _ -> Ok ()
        | None -> Error <| HttpException(404, "Cell not found.")
    )
    |> Result.bind (fun _ ->
        let currentTurn = game.currentTurn.Value
        if currentTurn.selectionOptions |> List.contains cellId |> not
        then Error <| HttpException(400, (sprintf "Cell %i is not currently selectable." cellId))
        else
            if currentTurn.status = AwaitingConfirmation
            then Error <| HttpException(400, "Cannot make seletion when awaiting turn confirmation.")
            else
            let pieceIndex = game.piecesIndexedByCell
            let board = BoardModelUtility.getBoardMetadata game.parameters.regionCount

            match currentTurn.selections.Length with
            | 0 -> Ok (Selection.subject(cellId, pieceIndex.Item(cellId).id), AwaitingSelection)
            | 1 -> let subject = currentTurn.subjectPiece(game).Value
                   match pieceIndex.TryFind(cellId) with
                    | None -> match subject.kind with
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
                    | Some target when subject.kind = Assassin ->
                        match board.cell cellId with
                        | None -> Error <| HttpException(404, "Cell not found.")
                        | Some c when c.isCenter ->
                            Ok (Selection.moveWithTarget(cellId, target.id), AwaitingSelection) //Awaiting vacate
                        | _ ->
                            Ok (Selection.moveWithTarget(cellId, target.id), AwaitingConfirmation)
                    | Some piece -> Ok (Selection.moveWithTarget(cellId, piece.id), AwaitingSelection) //Awaiting drop
            | 2 -> match (currentTurn.subjectPiece game).Value.kind with
                    | Thug
                    | Chief
                    | Diplomat
                    | Gravedigger -> Ok (Selection.drop(cellId), AwaitingConfirmation)
                    | Assassin -> Ok (Selection.move(cellId), AwaitingConfirmation) //Vacate
                    | Reporter -> Ok (Selection.target(cellId, pieceIndex.Item(cellId).id), AwaitingConfirmation)
                    | Corpse -> Error <| HttpException(400, "Subject cannot be corpse")
            | 3 -> match (currentTurn.subjectPiece game).Value.kind with
                    | Diplomat
                    | Gravedigger -> Ok (Selection.move(cellId), AwaitingConfirmation) //Vacate
                    | _ -> Error <| HttpException(400, "Cannot make fourth selection unless vacating center")
            | _ -> Error <| HttpException(400, "Cannot make more than 4 selections")

            |> Result.map (fun (selection, turnStatus) ->
                    let turnWithNewSelection =
                        {
                            status = turnStatus
                            selections = List.append currentTurn.selections [selection]
                            selectionOptions = List.empty
                            requiredSelectionKind = None
                        }
                    let updatedGame = { game with currentTurn = Some turnWithNewSelection }
                    let (selectionOptions, requiredSelectionType) = SelectionOptionsService.getSelectableCellsFromState updatedGame

                    let updatedTurn = 
                        { turnWithNewSelection with
                            selectionOptions = selectionOptions
                            requiredSelectionKind = requiredSelectionType
                        }
                    
                    {
                        kind = EventKind.CellSelected
                        effects = [Effect.currentTurnChanged(game.currentTurn, Some updatedTurn)]
                        createdByUserId = session.user.id                    
                    }
                    ))

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
            effects.Add(Effect.pieceMoved(subject.id, subject.cellId, destination.id))
        | Some vacateCellId -> 
            pieces.[subject.id] <- subject.moveTo vacateCellId
            effects.Add(Effect.pieceMoved(subject.id, destination.id, vacateCellId))

        match currentTurn.targetPiece game with
        | None -> ()
        | Some target ->
            //Kill target
            if subject.isKiller
            then 
                pieces.[target.id] <- target.kill
                effects.Add(Effect.pieceKilled(target.id))

            //Enlist players pieces if killing chief
            if subject.isKiller && target.kind = Chief
            then 
                let enlistedPieces = game.piecesControlledBy target.playerId.Value
                for p in enlistedPieces do
                    pieces.[p.id] <- pieces.[p.id].enlistBy subject.playerId.Value

                effects.Add(Effect.piecesOwnershipChanged(
                                enlistedPieces |> List.map(fun p -> p.id), 
                                target.playerId, 
                                subject.playerId))

            //Drop target if drop cell exists
            match currentTurn.dropCellId with
            | Some dropCellId ->  
                pieces.[target.id] <- pieces.[target.id].moveTo dropCellId
                effects.Add(Effect.pieceMoved(target.id, target.cellId, dropCellId))
            | None -> ()

            //Move target back to origin if subject is assassin
            if subject.kind = Assassin
            then 
                pieces.[target.id] <- pieces.[target.id].moveTo originCellId
                effects.Add(Effect.pieceMoved(target.id, target.cellId, originCellId))
        (pieces.Values |> Seq.toList, effects |> Seq.toList)

let private removeSequentialDuplicates(turnCycle : int list) : int list =
    let list = turnCycle.ToList()
    for i in [(list.Count-1)..1] do
        if list.[i] = list.[i-1]
        then list.RemoveAt(i)
    list |> Seq.toList

let private applyTurnToTurnCycle(game : Game) : (int list * Effect list) =
    let mutable turns = game.turnCycle
    let currentTurn = game.currentTurn.Value
    let regions = game.parameters.regionCount

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

    match (currentTurn.subjectPiece game, currentTurn.targetPiece game, currentTurn.dropCell regions) with
    //If chief being killed, remove all its turns
    | (Some subject, Some target, _)
        when subject.isKiller && target.kind = Chief ->
        turns <- turns |> Seq.filter(fun playerId -> playerId <> target.playerId.Value) |> Seq.toList
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

    turns <- removeSequentialDuplicates turns

    //Cycle turn queue
    turns <- List.append turns.Tail [turns.Head]

    let effect = Effect.turnCycleChanged(game.turnCycle, turns)
    (turns, [effect])

let private killCurrentPlayer(game : Game) : (Game * Effect list) =
    let playerId = game.turnCycle.Head

    let pieces = game.pieces
                    |> List.map (fun p -> if p.playerId = Some(playerId) then p.abandon else p)
    let players = game.players
                    |> List.map (fun p -> if p.id = playerId then p.kill else p)

    let turns = game.turnCycle
                |> List.filter (fun t -> t <> playerId)
                |> removeSequentialDuplicates

    let effects = new ArrayList<Effect>()
    effects.Add(Effect.playerOutOfMoves(playerId))
    effects.Add(Effect.turnCycleChanged(game.turnCycle, turns))

    let abandonedPieces = game.pieces |> List.filter (fun p -> p.playerId = Some playerId) |> List.map(fun p -> p.id)
    if abandonedPieces.IsEmpty |> not 
    then 
        effects.Add(Effect.piecesOwnershipChanged(abandonedPieces, Some playerId, None))

    let updatedGame =
        { game with
            pieces = pieces
            players = players
            turnCycle = turns
            currentTurn = Some Turn.empty
        }

    (updatedGame, effects |> Seq.toList)

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
                effects.Add(Effect.playerEliminated(target.playerId.Value))
                players <- players |> List.map (fun p -> if p.id = target.playerId.Value then p.kill else p)
        | _ -> ()

        let mutable updatedGame : Game =
            { game with 
                pieces = pieces
                turnCycle = turnCycle
                players = players
            }

        //While next player has no moves, kill chief and abandon all pieces
        let (options, reqSelType) = SelectionOptionsService.getSelectableCellsFromState updatedGame
        let mutable selectionOptions = options
        let mutable requiredSelectionType = reqSelType

        while selectionOptions.IsEmpty && updatedGame.turnCycle.Length > 1 do
            let (g, fx) = killCurrentPlayer updatedGame
            effects.AddRange(fx)
            updatedGame <- g
            let (opt, rst) = SelectionOptionsService.getSelectableCellsFromState updatedGame
            selectionOptions <- opt
            requiredSelectionType <- rst

        //TODO: If only 1 player, game over

        let updatedTurn =  
            { Turn.empty with
                selectionOptions = selectionOptions
                requiredSelectionKind = requiredSelectionType
            }

        effects.Add(Effect.currentTurnChanged(game.currentTurn, Some updatedTurn))

        {
            kind = EventKind.TurnCommitted
            effects = effects |> Seq.toList
            createdByUserId = session.user.id
        }
    )

let getResetTurnEvent(game : Game) (session : Session) : CreateEventRequest HttpResult =
    SecurityService.ensureAdminOrCurrentPlayer session game
    |> Result.map (fun _ ->
        let updatedGame = { game with currentTurn = Some Turn.empty }
        let (selectionOptions, requiredSelectionType) = SelectionOptionsService.getSelectableCellsFromState updatedGame
        let turn = 
            {
                Turn.empty with
                    selectionOptions = selectionOptions
                    requiredSelectionKind = requiredSelectionType
            }
        {
            kind = EventKind.TurnReset
            effects = [ Effect.currentTurnChanged(game.currentTurn, Some turn) ]
            createdByUserId = session.user.id
        }
    )