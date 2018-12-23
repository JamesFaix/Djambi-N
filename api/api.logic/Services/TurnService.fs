module Djambi.Api.Logic.Services.TurnService

open System.Collections.Generic
open System.Linq
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.Logic.ModelExtensions
open Djambi.Api.Logic.ModelExtensions.BoardModelExtensions
open Djambi.Api.Logic.ModelExtensions.GameModelExtensions
open Djambi.Api.Model

let private ensureSessionIsAdminOrContainsCurrentPlayer (session : Session) (game : Game) : Game AsyncHttpResult =
    if session.isAdmin
    then okTask <| game
    else
        GameRepository.getPlayersForGames [game.id]
        |> thenBind (fun players ->
            let currentPlayer = players |> List.find (fun p -> p.id = game.turnCycle.Head)
            match currentPlayer.userId with
            | Some uId when uId = session.userId -> Ok game
            | _ -> Error <| HttpException(400, "Cannot perform this action during another player's turn.")
        )

let selectCell(gameId : int, cellId : int) (session: Session) : Turn AsyncHttpResult =
    GameRepository.getGame gameId
    |> thenBindAsync (ensureSessionIsAdminOrContainsCurrentPlayer session)
    |> thenBindAsync (fun game ->
        let board = BoardModelUtility.getBoardMetadata game.parameters.regionCount
        match board.cell cellId with
        | Some _ -> okTask game
        | None -> errorTask <| HttpException(404, "Cell not found.")
    )
    |> thenBind (fun game ->
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

                    {
                        turnWithNewSelection with
                            selectionOptions = selectionOptions
                            requiredSelectionKind = requiredSelectionType
                    }))

let private applyTurnToPieces(game : Game) : Piece list =
    let pieces = game.pieces.ToDictionary(fun p -> p.id)
    let currentTurn = game.currentTurn.Value

    match (currentTurn.subjectPiece game, currentTurn.destinationCell game.parameters.regionCount) with
    | (None, _)
    | (_, None) -> raise (HttpException(500, "Cannot commit turn without subject and destination selected."))
    | (Some subject, Some destination) ->
        let origin = subject.cellId

        //Move subject to destination or vacate cell
        match currentTurn.vacateCellId with
        | None ->        pieces.[subject.id] <- subject.moveTo destination.id
        | Some vacate -> pieces.[subject.id] <- subject.moveTo vacate

        match currentTurn.targetPiece game with
        | None -> ()
        | Some target ->
            //Kill target
            if subject.isKiller
            then pieces.[target.id] <- target.kill

            //Enlist players pieces if killing chief
            if subject.isKiller && target.kind = Chief
            then for p in game.piecesControlledBy target.playerId.Value do
                    pieces.[p.id] <- pieces.[p.id].enlistBy subject.playerId.Value

            //Drop target if drop cell exists
            match currentTurn.dropCellId with
            | Some drop ->  pieces.[target.id] <- pieces.[target.id].moveTo drop
            | None -> ()

            //Move target back to origin if subject is assassin
            if subject.kind = Assassin
            then pieces.[target.id] <- pieces.[target.id].moveTo origin
        pieces.Values |> Seq.toList

let private removeSequentialDuplicates(turnCycle : int list) : int list =
    let list = turnCycle.ToList()
    for i in [(list.Count-1)..1] do
        if list.[i] = list.[i-1]
        then list.RemoveAt(i)
    list |> Seq.toList

let private applyTurnToTurnCycle(game : Game) : int list =
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

    turns

let private killCurrentPlayer(game : Game) : Game =
    let playerId = game.turnCycle.Head

    let pieces = game.pieces
                    |> List.map (fun p -> if p.playerId = Some(playerId) then p.abandon else p)
    let players = game.players
                    |> List.map (fun p -> if p.id = playerId then p.kill else p)

    let turns = game.turnCycle
                |> List.filter (fun t -> t <> playerId)
                |> removeSequentialDuplicates
    
    { game with
        pieces = pieces
        players = players
        turnCycle = turns
        currentTurn = Some Turn.empty
    }

let commitTurn(gameId : int) (session : Session) : Game AsyncHttpResult =
    GameRepository.getGame gameId
    |> thenBindAsync (ensureSessionIsAdminOrContainsCurrentPlayer session)
    |> thenBindAsync (fun game ->
        let turnCycle = applyTurnToTurnCycle game
        let pieces = applyTurnToPieces game
        let currentTurn = game.currentTurn.Value
        let mutable players = game.players

        //Kill player if chief killed
        match (currentTurn.subjectPiece game, currentTurn.targetPiece game) with
        | (Some subject, Some target) 
            when subject.isKiller && target.kind = Chief ->
                GameRepository.killPlayer target.playerId.Value
                |> thenDoAsync (fun _ -> 
                    players <- players |> List.map (fun p -> if p.id = target.playerId.Value then p.kill else p)
                    okTask ()
                )
        | _ -> okTask ()
        |> thenBindAsync (fun _ ->
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
                updatedGame <- killCurrentPlayer updatedGame
                let (opt, rst) = SelectionOptionsService.getSelectableCellsFromState updatedGame
                selectionOptions <- opt
                requiredSelectionType <- rst

            //TODO: If only 1 player, game over

            let request : UpdateGameStateRequest =
                {
                    gameId = game.id
                    status = game.status
                    pieces = updatedGame.pieces
                    turnCycle = updatedGame.turnCycle
                    currentTurn = Some 
                        { Turn.empty with
                            selectionOptions = selectionOptions
                            requiredSelectionKind = requiredSelectionType
                        }
                }

            GameRepository.updateGameState request        
        )        
    )
    |> thenBindAsync (fun _ -> GameRepository.getGame gameId)

let resetTurn(gameId : int) (session : Session) : Turn AsyncHttpResult =
    GameRepository.getGame gameId
    |> thenBindAsync (ensureSessionIsAdminOrContainsCurrentPlayer session)
    |> thenBindAsync (fun game ->
        let updatedGame = { game with currentTurn = Some Turn.empty }
        let (selectionOptions, requiredSelectionType) = SelectionOptionsService.getSelectableCellsFromState updatedGame
        let turn = 
            {
                Turn.empty with
                    selectionOptions = selectionOptions
                    requiredSelectionKind = requiredSelectionType
            }
 
        let request : UpdateGameStateRequest =
            {
                gameId = game.id
                status = game.status
                pieces = game.pieces
                turnCycle = game.turnCycle
                currentTurn = Some turn
            }

        GameRepository.updateGameState request 
        |> thenMap (fun _ -> turn)
    )