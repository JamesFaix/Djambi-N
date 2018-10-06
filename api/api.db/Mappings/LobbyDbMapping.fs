namespace Djambi.Api.Db.Mappings

open Djambi.Api.Common.Enums
open Djambi.Api.Common.Utilities
open Djambi.Api.Db.Model.LobbyDbModel
open Djambi.Api.Model.LobbyModel

module LobbyDbMapping =

    let mapGameStatusFromId(gameStatusId : byte) : GameStatus =
        match gameStatusId with 
        | 1uy -> Open
        | 2uy -> Started
        | 3uy -> Complete
        | 4uy -> Cancelled
        | _ -> failwith ("Invalid game status id: " + gameStatusId.ToString())

    let mapGameStatusToId(status : GameStatus) : byte =
        match status with 
        | Open -> 1uy
        | Started -> 2uy
        | Complete -> 3uy
        | Cancelled -> 4uy

    let mapRoleFromId(roleId : byte) : Role =
        match roleId with
        | 1uy -> Admin
        | 2uy -> Normal
        | 3uy -> Guest
        | _ -> failwith ("Invalid role id: " + roleId.ToString())

    let mapRoleToId(role : Role) : byte =
        match role with
        | Admin -> 1uy
        | Normal -> 2uy
        | Guest -> 3uy

    let mapUserResponse(sqlModel : UserSqlModel) : User =
        {
            id = sqlModel.id
            name = sqlModel.name
            role = sqlModel.roleId |> mapRoleFromId
            password = sqlModel.password
            failedLoginAttempts = int sqlModel.failedLoginAttempts
            lastFailedLoginAttemptOn = sqlModel.lastFailedLoginAttemptOn |> nullableToOption
        }

    let mapLobbyGamesResponse(sqlModels : LobbyGamePlayerSqlModel seq) : LobbyGameMetadata list =
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

        sqlModels 
        |> Seq.groupBy (fun sql -> sql.gameId)
        |> Seq.map (fun (_, sqls) -> sqls |> Seq.toList |> sqlModelsToGame)
        |> Seq.toList

    let mapSession (sqlModel : SessionSqlModel) : Session =
        {
            id = sqlModel.sessionId
            userId = sqlModel.userId
            token = sqlModel.token
            createdOn = sqlModel.createdOn
            expiresOn = sqlModel.expiresOn
        }