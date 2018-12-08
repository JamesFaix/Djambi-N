[<AutoOpen>]
module Djambi.Api.Web.Model.GameWebModel

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
        turnNumber : int Nullable
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
