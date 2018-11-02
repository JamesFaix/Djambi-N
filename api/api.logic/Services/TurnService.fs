module Djambi.Api.Logic.Services.TurnService

open System.Collections.Generic
open System.Linq
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.Logic.ModelExtensions.BoardModelExtensions
open Djambi.Api.Logic.ModelExtensions.GameModelExtensions
open Djambi.Api.Model.GameModel
open Djambi.Api.Model.SessionModel

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
        let (options, reqSelType) = SelectionService.getSelectableCellsFromState updatedGame
        let mutable selectionOptions = options
        let mutable requiredSelectionType = reqSelType

        while selectionOptions.IsEmpty && updatedGame.gameState.turnCycle.Length > 1 do
            updatedGame <-
                {
                    gameState = killCurrentPlayer(updatedGame.gameState)
                    turnState = TurnState.empty
                    regionCount = game.regionCount
                }
            let (opt, rst) = SelectionService.getSelectableCellsFromState updatedGame
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

        let (selectionOptions, requiredSelectionType) = SelectionService.getSelectableCellsFromState updatedGame

        {
            updatedGame.turnState with
                selectionOptions = selectionOptions
                requiredSelectionType = requiredSelectionType
        })
    |> thenDoAsync (fun turnState -> GameRepository.updateTurnState(gameId, turnState))