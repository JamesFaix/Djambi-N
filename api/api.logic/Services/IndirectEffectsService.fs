module Djambi.Api.Logic.Services.IndirectEffectsService

open System.Collections.Generic
open System.Linq
open Djambi.Api.Common.Collections
open Djambi.Api.Common.Control
open Djambi.Api.Logic.ModelExtensions.BoardModelExtensions
open Djambi.Api.Logic.ModelExtensions.GameModelExtensions
open Djambi.Api.Logic.PieceStrategies
open Djambi.Api.Logic.Services
open Djambi.Api.Model

let private removeSequentialDuplicates(turnCycle : int list) : int list =
    if turnCycle.Length = 1 then turnCycle
    else
        let list = turnCycle.ToList()
        for i in [(list.Count-1)..1] do
            if list.[i] = list.[i-1]
            then list.RemoveAt(i)
        list |> Seq.toList

let private getKillAbandonedChiefEffects (game : Game) (chiefOriginalPlayerId : int) (killingPlayerId : int) : Effect list =
    game.pieces 
    |> List.filter (fun p -> 
        p.originalPlayerId = chiefOriginalPlayerId 
        && p.playerId = None)
    |> List.map (fun p ->
        Effect.PieceEnlisted {
            oldPiece = p
            newPlayerId = Some killingPlayerId
        }
    )

let private getEliminatePlayerEffects (game : Game) (playerId : int) (killingPlayerId : int option) : Effect list =
    let p = game.players |> List.find (fun p -> p.id = playerId)
    
    let effects = new ArrayList<Effect>()
    
    effects.Add(Effect.PlayerStatusChanged {
        playerId = p.id
        oldStatus = p.status
        newStatus = Eliminated
    })

    let newCycle = 
        game.turnCycle 
        |> List.filter (fun pId -> pId <> p.id) 
        |> removeSequentialDuplicates

    effects.Add(Effect.TurnCyclePlayerRemoved {
        playerId = p.id
        oldValue = game.turnCycle
        newValue = newCycle
    })

    let pieces = game.pieces |> List.filter (fun piece -> piece.playerId = Some p.id)

    let addPieceEffect =
        if killingPlayerId.IsSome
        then
            (fun piece -> effects.Add(Effect.PieceEnlisted {
                oldPiece = piece
                newPlayerId = killingPlayerId
            }))
        else
            (fun piece -> effects.Add(Effect.PieceAbandoned {
                oldPiece = piece
            }))
    
    for piece in pieces do
        addPieceEffect piece

    effects |> Seq.toList

let private getRiseOrFallFromPowerEffects (game : Game, updatedGame : Game) : Effect list =
    //Copy only the last turn of the given player, plus all other turns
    let removeBonusTurnsForPlayer playerId turns =
        let stack = new Stack<int>()
        let mutable hasAddedTargetPlayer = false
        for t in turns |> Seq.rev do
            if t = playerId && not hasAddedTargetPlayer
            then
                hasAddedTargetPlayer <- true
                stack.Push t
            else stack.Push t
        stack |> Seq.toList |> removeSequentialDuplicates

    //Insert a turn for the given player before every turn, except the current one
    let addBonusTurnsForPlayer playerId turns =
        let stack = new Stack<int>()
        for t in turns |> Seq.skip(1) |> Seq.rev do
            stack.Push t
            stack.Push playerId
        stack |> Seq.toList |> removeSequentialDuplicates

    //A player in power has multiple turns
    let hasPower (g : Game) (pId : int) =
        let turns = 
            g.turnCycle 
            |> Seq.filter (fun n -> n = pId) 
            |> Seq.length 
        turns > 1

    //Players can only rise to power if there are more than 2 left
    let powerCanBeHad (g : Game) =
        let players = 
            g.turnCycle 
            |> Seq.distinct 
            |> Seq.length 
        players > 2

    let turn = updatedGame.currentTurn.Value
    let subject = (turn.subjectPiece game).Value
    let destination = (turn.destinationCell game.parameters.regionCount).Value
    let origin = (turn.subjectCell game.parameters.regionCount).Value
    let subjectStrategy = PieceService.getStrategy subject

    let effects = new ArrayList<Effect>()
    let mutable turns = updatedGame.turnCycle

    let subjectPlayerId = subject.playerId.Value
    if subjectStrategy.canStayInCenter 
    then
        if origin.isCenter 
            && not destination.isCenter
            && hasPower updatedGame subjectPlayerId
        then 
            //If chief subject leaves power, remove bonus turns
            let newTurns = removeBonusTurnsForPlayer subjectPlayerId turns
            effects.Add(Effect.TurnCyclePlayerFellFromPower { playerId = subjectPlayerId; oldValue = turns; newValue = newTurns })
            turns <- newTurns
        elif not origin.isCenter 
            && destination.isCenter
            && powerCanBeHad updatedGame
        then 
            //If chief subject rises to power, add bonus turns
            let newTurns = addBonusTurnsForPlayer subjectPlayerId turns
            effects.Add(Effect.TurnCyclePlayerRoseToPower { playerId = subjectPlayerId; oldValue = turns; newValue = newTurns })
            turns <- newTurns
        else ()

    match (turn.targetPiece game, turn.dropCell game.parameters.regionCount) with
    | (Some target, Some drop) ->
        let targetStrategy = PieceService.getStrategy target
        
        if subjectStrategy.canEnterCenterToEvictPiece 
            && not subjectStrategy.killsTarget
            && subjectStrategy.canDropTarget
            && targetStrategy.canStayInCenter
        then
            //If chief target is moved out of power and not killed, remove bonus turns
            if destination.isCenter 
                && not drop.isCenter
                && hasPower updatedGame subjectPlayerId
            then
                let newTurns = removeBonusTurnsForPlayer subjectPlayerId turns
                effects.Add(Effect.TurnCyclePlayerFellFromPower { playerId = subjectPlayerId; oldValue = turns; newValue = newTurns })
                turns <- newTurns
            //If chief target is dropped in power and not killed, add bonus turns
            elif not destination.isCenter 
                && drop.isCenter
                && powerCanBeHad updatedGame
            then
                let newTurns = addBonusTurnsForPlayer subjectPlayerId turns
                effects.Add(Effect.TurnCyclePlayerRoseToPower { playerId = subjectPlayerId; oldValue = turns; newValue = newTurns })
                turns <- newTurns
            else ()
        else ()
    | _ -> ()

    effects |> Seq.toList

let getSecondaryEffects (game : Game, updatedGame : Game) : Effect list =
    let effects = new ArrayList<Effect>()
    
    let turn = updatedGame.currentTurn.Value
    let subject = (turn.subjectPiece game).Value
    let subjectStrategy = PieceService.getStrategy subject

    let killChiefEffects = 
        match turn.targetPiece game with
        | Some target ->
            let targetStrategy = PieceService.getStrategy target

            if subjectStrategy.killsTarget 
                && targetStrategy.killsControllingPlayerWhenKilled
            then 
                if target.playerId.IsSome
                then getEliminatePlayerEffects game target.playerId.Value subject.playerId
                else getKillAbandonedChiefEffects game target.originalPlayerId subject.playerId.Value
            else []
        | _ -> []

    effects.AddRange killChiefEffects
    let updatedGame = EventService.applyEffects killChiefEffects updatedGame

    let riseFallEffects = getRiseOrFallFromPowerEffects (game, updatedGame)
    effects.AddRange riseFallEffects
    
    effects |> Seq.toList

let private getVictoryEffects (game : Game) : Effect list =
    let remainingPlayers = game.players |> List.filter (fun p -> p.status = Alive)
    if remainingPlayers.Length = 1        
    then
        let p = remainingPlayers.[0]
        [
            Effect.PlayerStatusChanged { oldStatus = p.status; newStatus = Victorious; playerId = p.id}
            Effect.GameStatusChanged { oldValue = game.status; newValue = Finished }
            Effect.CurrentTurnChanged { oldValue = game.currentTurn; newValue = None }
        ]
    else []

let private getOutOfMovesEffects (game : Game) : Effect list =
    let effects = new ArrayList<Effect>()
    let mutable game = game
    let mutable selectionOptions = (SelectionOptionsService.getSelectableCellsFromState game) |> Result.value
   
    while selectionOptions.IsEmpty && game.turnCycle.Length > 1 do
        let currentPlayerId = game.turnCycle.[0]
        let fx = [Effect.PlayerOutOfMoves { playerId = currentPlayerId }]
        let fx = List.append fx (getEliminatePlayerEffects game currentPlayerId None)
        effects.AddRange(fx)
        game <- EventService.applyEffects fx game
        selectionOptions <- (SelectionOptionsService.getSelectableCellsFromState) game |> Result.value

    effects |> Seq.toList

let getTernaryEffects (updatedGame : Game) : Effect list = 
    
    let effects = new ArrayList<Effect>()

    let victoryEffects = getVictoryEffects updatedGame
    effects.AddRange victoryEffects

    if victoryEffects.IsEmpty
    then
        let advanceEffect = Effect.TurnCycleAdvanced { 
            oldValue = updatedGame.turnCycle
            newValue = updatedGame.turnCycle |> List.rotate 1
        }
        effects.Add advanceEffect
        let updatedGame = EventService.applyEffect advanceEffect updatedGame

        let outOfMovesEffects = getOutOfMovesEffects updatedGame
        effects.AddRange outOfMovesEffects
        let updatedGame = EventService.applyEffects outOfMovesEffects updatedGame

        //Check for victory caused by players being out of moves
        let victoryEffects = getVictoryEffects updatedGame
        effects.AddRange victoryEffects
        let updatedGame = EventService.applyEffects victoryEffects updatedGame

        if victoryEffects.IsEmpty
        then 
            let seletionOptions = SelectionOptionsService.getSelectableCellsFromState updatedGame |> Result.value
            let turn = { Turn.empty with selectionOptions = seletionOptions }
            effects.Add(Effect.CurrentTurnChanged { oldValue = updatedGame.currentTurn; newValue = Some turn })
        else ()
    else ()

    effects |> Seq.toList