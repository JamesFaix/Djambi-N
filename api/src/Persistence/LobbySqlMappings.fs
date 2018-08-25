﻿namespace Djambi.Api.Persistence

module LobbySqlMappings =
    open LobbySqlModels
    open Djambi.Api.Domain.LobbyModels
    open Djambi.Api.Common.Enums

    let mapUserResponse(user : UserSqlModel) : User =
        {
            id = user.id
            name = user.name
            isAdmin = user.isAdmin
            isGuest = user.isGuest
        }

    let mapGameStatusFromId(gameStatusId : int) : GameStatus =
        match gameStatusId with 
        | 1 -> GameStatus.Open
        | 2 -> GameStatus.Started
        | 3 -> GameStatus.Complete
        | 4 -> GameStatus.Cancelled
        | _ -> failwith ("Invalid game status: " + gameStatusId.ToString())

    let mapGameStatusToId(status : GameStatus) : int =
        match status with 
        | GameStatus.Open -> 1
        | GameStatus.Started -> 2
        | GameStatus.Complete -> 3
        | GameStatus.Cancelled -> 4

    let mapLobbyGamesResponse(players : LobbyGamePlayerSqlModel list) : LobbyGameMetadata list =
        let sqlModelToUser(sqlModel : LobbyGamePlayerSqlModel) : LobbyPlayer =
            {
                id = sqlModel.playerId.Value
                userId = if sqlModel.userId.HasValue then Some sqlModel.userId.Value else None
                name = sqlModel.playerName
            }

        let sqlModelsToGame(sqlModels : LobbyGamePlayerSqlModel list) : LobbyGameMetadata =
            let head = sqlModels.Head
            {
                id = head.gameId
                status = head.gameStatusId |> mapGameStatusFromId
                boardRegionCount = head.boardRegionCount
                description = if head.gameDescription = null then None else Some head.gameDescription
                players = 
                    if head.playerId.HasValue
                    then sqlModels |> List.map sqlModelToUser
                    else List.empty
            }

        players 
        |> Seq.groupBy (fun sql -> sql.gameId)
        |> Seq.map (fun (_, sqls) -> sqls |> Seq.toList |> sqlModelsToGame)
        |> Seq.toList