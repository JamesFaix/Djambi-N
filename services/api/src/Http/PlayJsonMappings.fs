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
        match selection with 
        | Subject pieceId ->
            {
                ``type`` = "Subject"
                pieceId = new Nullable<int>(pieceId) 
                cellId = new Nullable<int>() 
            }                              
        | Move cellId -> 
            {
                ``type`` = "Move"
                pieceId = new Nullable<int>() 
                cellId = new Nullable<int>(cellId) 
            }
        | MoveWithTarget (cellId, pieceId) -> 
            {
                ``type`` = "MoveWithTarget"
                pieceId = new Nullable<int>(pieceId) 
                cellId = new Nullable<int>(cellId) 
            }
        | Target pieceId -> 
            {
                ``type`` = "Target"
                pieceId = new Nullable<int>(pieceId) 
                cellId = new Nullable<int>() 
            }
        | Drop cellId -> 
            {
                ``type`` = "Drop"
                pieceId = new Nullable<int>() 
                cellId = new Nullable<int>(cellId) 
            }

    let mapTurnStateToJsonModel(turnState : TurnState) : TurnStateJsonModel =
        {
            status = turnState.status.ToString()
            selections = turnState.selections |> List.map mapSelectionToJsonModel
            selectionOptions = turnState.selectionOptions
        }

    let mapGameStartResponseToJson(response : GameStartResponse) : GameStartResponseJsonModel = 
        {
            gameState = response.gameState |> mapGameStateToJsonModel
            turnState = response.turnState |> mapTurnStateToJsonModel
            startingConditions = response.startingConditions |> List.map mapPlayerStartConditionsToJson
        }