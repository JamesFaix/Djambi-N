module Djambi.Api.Web.Model.PlayWebModel

open System

[<CLIMutable>]
type CreateSelectionJsonModel =
    {
        cellId : int
    }

type PlayerStateJsonModel =
    {
        id : int
        isAlive : bool
    }

type PieceJsonModel =
    {
        id : int
        ``type`` : string
        playerId : Nullable<int>
        originalPlayerId : int
        cellId : int
    }

type GameStateJsonModel =
    {
        players : PlayerStateJsonModel list 
        pieces : PieceJsonModel list
        turnCycle : int list
    }

type PlayerStartConditionsJsonModel =
    {
        playerId : int
        turnNumber : int
        region : int
        color : int
    }

type SelectionJsonModel =
    {
        ``type`` : string
        cellId : int
        pieceId : Nullable<int>
    }

type TurnStateJsonModel = 
    {
        status : string
        selections : SelectionJsonModel list
        selectionOptions : int list
        requiredSelectionType : string
    }
        
type GameStartResponseJsonModel =
    {
        gameState : GameStateJsonModel
        startingConditions : PlayerStartConditionsJsonModel list
        turnState : TurnStateJsonModel
    }

type CommitTurnResponseJsonModel =
    {
        gameState : GameStateJsonModel
        turnState : TurnStateJsonModel
    }
