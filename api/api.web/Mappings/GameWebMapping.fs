module Djambi.Api.Web.Mappings.GameWebMapping

open System
open Djambi.Api.Common.Utilities
open Djambi.Api.Model.GameModel
open Djambi.Api.Web.Model.GameWebModel

let mapPlayerStateToJson(player : PlayerState) : PlayerStateJsonModel =
    {
        id = player.id
        isAlive = player.isAlive
    }

let mapPieceToJson(piece : Piece) : PieceJsonModel =
    {
        id = piece.id
        ``type`` = piece.pieceType.ToString()
        playerId = piece.playerId |> optionToNullable
        originalPlayerId = piece.originalPlayerId
        cellId = piece.cellId
    }

let mapGameStateToJsonModel(gameState : GameState) : GameStateJsonModel =
    {
        players = gameState.players |> List.map mapPlayerStateToJson
        pieces = gameState.pieces |> List.map mapPieceToJson
        turnCycle = gameState.turnCycle
    }

let mapPlayerStartConditionsToJson(conditions : PlayerStartConditions) : PlayerStartConditionsJsonModel =
    {
        playerId = conditions.playerId
        color = conditions.color
        region = conditions.region
        turnNumber = conditions.turnNumber |> optionToNullable
    }

let mapSelectionToJsonModel(selection : Selection) : SelectionJsonModel =
    {
        ``type`` = selection.selectionType.ToString()
        pieceId = if selection.pieceId.IsNone then new Nullable<int>() else new Nullable<int>(selection.pieceId.Value)
        cellId = selection.cellId
    }

let mapTurnStateToJsonModel(turnState : TurnState) : TurnStateJsonModel =
    {
        status = turnState.status.ToString()
        selections = turnState.selections |> List.map mapSelectionToJsonModel
        selectionOptions = turnState.selectionOptions
        requiredSelectionType = if turnState.requiredSelectionType.IsNone then null else turnState.requiredSelectionType.Value.ToString()
    }

let mapGameStartResponseToJson(response : StartGameResponse) : GameStartResponseJsonModel =
    {
        gameState = response.gameState |> mapGameStateToJsonModel
        turnState = response.turnState |> mapTurnStateToJsonModel
        startingConditions = response.startingConditions |> List.map mapPlayerStartConditionsToJson
    }

let mapCommitTurnResponseToJsonModel(response : CommitTurnResponse) : CommitTurnResponseJsonModel =
    {
        gameState = response.gameState |> mapGameStateToJsonModel
        turnState = response.turnState |> mapTurnStateToJsonModel
    }
