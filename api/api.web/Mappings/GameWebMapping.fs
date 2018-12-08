[<AutoOpen>]
module Djambi.Api.Web.Mappings.GameWebMapping

open Djambi.Api.Model
open Djambi.Api.Web.Model

let mapPlayerStateToJson(player : PlayerState) : PlayerStateJsonModel =
    {
        id = player.id
        isAlive = player.isAlive
    }

let mapPieceToJson(piece : Piece) : PieceJsonModel =
    {
        id = piece.id
        ``type`` = piece.pieceType
        playerId = piece.playerId
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
        turnNumber = conditions.turnNumber
    }

let mapSelectionToJsonModel(selection : Selection) : SelectionJsonModel =
    {
        ``type`` = selection.selectionType
        pieceId = selection.pieceId
        cellId = selection.cellId
    }

let mapTurnStateToJsonModel(turnState : TurnState) : TurnStateJsonModel =
    {
        status = turnState.status
        selections = turnState.selections |> List.map mapSelectionToJsonModel
        selectionOptions = turnState.selectionOptions
        requiredSelectionType = turnState.requiredSelectionType
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
