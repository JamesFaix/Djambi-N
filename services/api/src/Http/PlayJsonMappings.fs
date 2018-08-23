namespace Djambi.Api.Http

open Djambi.Api.Domain.PlayModels
open Djambi.Api.Http.PlayJsonModels
open System

module PlayJsonMappings =
    open Djambi.Api.Domain.BoardModels

    let mapPlayerToJson(player : Player) : PlayerJsonModel =
        {
            id = player.id
            userId = if player.userId.IsNone 
                     then new Nullable<int>() 
                     else new Nullable<int>(player.userId.Value)
            name = player.name
            isAlive = player.isAlive
        }

    let mapPieceToJson(piece : Piece) : PieceJsonModel =
        {
            id = piece.id
            ``type`` = piece.pieceType.ToString()
            playerId = if piece.playerId.IsNone
                       then new Nullable<int>()
                       else new Nullable<int>(piece.playerId.Value)
            originalPlayerId = piece.originalPlayerId
            cellId = piece.cellId
        }

    let mapGameStateToJsonModel(gameState : GameState) : GameStateJsonModel =
        {
            players = gameState.players |> List.map mapPlayerToJson
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
            ``type`` = selection.selectionType.ToString()
            pieceId = if selection.pieceId.IsNone then new Nullable<int>() else new Nullable<int>(selection.pieceId.Value)
            cellId = selection.cellId
        }

    let mapTurnStateToJsonModel(turnState : TurnState) : TurnStateJsonModel =
        {
            status = turnState.status.ToString()
            selections = turnState.selections |> List.map mapSelectionToJsonModel
            selectionOptions = turnState.selectionOptions
            requiredSelectionType = string turnState.requiredSelectionType
        }

    let mapGameStartResponseToJson(response : GameStartResponse) : GameStartResponseJsonModel = 
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
