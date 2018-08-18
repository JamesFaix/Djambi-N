namespace Djambi.Api.Http

open Djambi.Api.Domain.PlayModels
open Djambi.Api.Http.PlayJsonModels
open System

module PlayJsonMappings =

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
            isAlive = piece.isAlive
            cellId = piece.cellId
        }

    let mapGameStateToJson(gameState : GameState) : GameStateJsonModel =
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

    let mapGameStartResponseToJson(response : GameStartResponse) : GameStartResponseJsonModel = 
        {
            currentState = response.currentState |> mapGameStateToJson
            startingConditions = response.startingConditions |> List.map mapPlayerStartConditionsToJson
        }
