module Djambi.Api.Db.Mapping

open System
open Newtonsoft.Json
open Djambi.Api.Db.Model
open Djambi.Api.Model
open Djambi.Api.Common.Utilities

let mapUserResponse (sqlModel : UserSqlModel) : UserDetails =
    {
        id = sqlModel.userId
        name = sqlModel.name
        isAdmin = sqlModel.isAdmin
        password = sqlModel.password
        failedLoginAttempts = int sqlModel.failedLoginAttempts
        lastFailedLoginAttemptOn = sqlModel.lastFailedLoginAttemptOn |> nullableToOption
    }

let mapSessionResponse (sqlModel : SessionSqlModel) : Session =
    {
        id = sqlModel.sessionId
        userId = sqlModel.userId
        token = sqlModel.token
        createdOn = sqlModel.createdOn
        expiresOn = sqlModel.expiresOn
        isAdmin = sqlModel.isAdmin
    }
    
let mapPlayerTypeId (playerTypeId : byte) : PlayerKind =
    match playerTypeId with
    | 1uy -> PlayerKind.User
    | 2uy -> PlayerKind.Guest
    | 3uy -> PlayerKind.Neutral
    | _ -> raise <| Exception("Invalid player type")

let mapPlayerKindToId (kind : PlayerKind) : byte =
    match kind with
    | PlayerKind.User -> 1uy
    | PlayerKind.Guest -> 2uy
    | PlayerKind.Neutral -> 3uy

let mapPlayer (sqlModel : PlayerSqlModel) : Player =
    {
        id = sqlModel.playerId
        lobbyId = sqlModel.lobbyId
        userId = sqlModel.userId |> nullableToOption
        kind = mapPlayerTypeId sqlModel.playerTypeId
        name = sqlModel.name
    }

let mapLobby (sqlModel : LobbySqlModel) : Lobby =
    {
        id = sqlModel.lobbyId
        description = sqlModel.description |> referenceToOption
        regionCount = sqlModel.regionCount
        createdOn = sqlModel.createdOn
        createdByUserId = sqlModel.createdByUserId
        isPublic = sqlModel.isPublic
        allowGuests = sqlModel.allowGuests
        //TODO: Add player count
    }
    
let mapGameSqlModelResponse(sqlModel : GameSqlModel) : Game =
    {
        id = sqlModel.gameId
        regionCount = sqlModel.regionCount
        gameState = JsonConvert.DeserializeObject<GameState>(sqlModel.gameStateJson)
        turnState =
            match sqlModel.turnStateJson with
            | null -> TurnState.empty
            | _ -> JsonConvert.DeserializeObject<TurnState>(sqlModel.turnStateJson)
    }