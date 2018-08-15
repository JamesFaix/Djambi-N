namespace Djambi.Api.Persistence

module LobbySqlMappings =
    open LobbySqlModels
    open Djambi.Api.Domain.LobbyModels
    open Djambi.Api.Common.Enums

    let mapUserResponse(user : UserSqlModel) : User =
        {
            id = user.id
            name = user.name
        }

    let mapGameStatusFromId(gameStatusId : int) : GameStatus =
        match gameStatusId with 
        | 1 -> GameStatus.Open
        | 2 -> GameStatus.Started
        | 3 -> GameStatus.Complete
        | 4 -> GameStatus.Cancelled
        | _ -> failwith ("Invalid game status: " + gameStatusId.ToString())

    let mapLobbyGamesResponse(players : LobbyGamePlayerSqlModel list) : LobbyGameMetadata list =
        let sqlModelToUser(sqlModel : LobbyGamePlayerSqlModel) : User =
            {
                id = sqlModel.userId.Value
                name = sqlModel.userName
            }

        let sqlModelsToGame(sqlModels : LobbyGamePlayerSqlModel list) : LobbyGameMetadata =
            let head = sqlModels.Head
            {
                id = head.gameId
                status = head.gameStatusId |> mapGameStatusFromId
                boardRegionCount = head.boardRegionCount
                description = if head.gameDescription = null then None else Some head.gameDescription
                players = 
                    if head.userId.HasValue
                    then sqlModels |> List.map sqlModelToUser
                    else List.empty
            }

        players 
        |> Seq.groupBy (fun sql -> sql.gameId)
        |> Seq.map (fun (_, sqls) -> sqls |> Seq.toList |> sqlModelsToGame)
        |> Seq.toList

