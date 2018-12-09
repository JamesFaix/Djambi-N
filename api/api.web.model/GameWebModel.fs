[<AutoOpen>]
module Djambi.Api.Web.Model.GameWebModel

open Djambi.Api.Model

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
        ``type`` : PieceType
        playerId : int option
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
        turnNumber : int option
        region : int
        color : int
    }

type SelectionJsonModel =
    {
        ``type`` : SelectionType
        cellId : int
        pieceId : int option
    }

type TurnStateJsonModel =
    {
        status : TurnStatus
        selections : SelectionJsonModel list
        selectionOptions : int list
        requiredSelectionType : SelectionType option
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
