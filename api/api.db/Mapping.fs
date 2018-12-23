module Djambi.Api.Db.Mapping

open System
open Djambi.Api.Common
open Djambi.Api.Common.Json
open Djambi.Api.Db.Model
open Djambi.Api.Model

let mapUserResponse (sqlModel : UserSqlModel) : UserDetails =
    {
        id = sqlModel.userId
        name = sqlModel.name
        isAdmin = sqlModel.isAdmin
        password = sqlModel.password
        failedLoginAttempts = int sqlModel.failedLoginAttempts
        lastFailedLoginAttemptOn = sqlModel.lastFailedLoginAttemptOn |> Option.ofNullable
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
    
let mapPlayerKindId (playerKindId : byte) : PlayerKind =
    match playerKindId with
    | 1uy -> PlayerKind.User
    | 2uy -> PlayerKind.Guest
    | 3uy -> PlayerKind.Neutral
    | _ -> raise <| Exception("Invalid player kind")

let mapPlayerKindToId (kind : PlayerKind) : byte =
    match kind with
    | PlayerKind.User -> 1uy
    | PlayerKind.Guest -> 2uy
    | PlayerKind.Neutral -> 3uy

let mapPlayerResponse (sqlModel : PlayerSqlModel) : Player =
    {
        id = sqlModel.playerId
        gameId = sqlModel.gameId
        userId = sqlModel.userId |> Option.ofNullable
        kind = mapPlayerKindId sqlModel.playerKindId
        name = sqlModel.name
        isAlive = sqlModel.isAlive |> Option.ofNullable
        colorId = sqlModel.colorId |> Option.ofNullable |> Option.map int
        startingRegion = sqlModel.startingRegion |> Option.ofNullable |> Option.map int
        startingTurnNumber = sqlModel.startingTurnNumber |> Option.ofNullable |> Option.map int
    }

let mapGameStatusId (gameStatusId : byte) : GameStatus =
    match gameStatusId with
    | 1uy -> GameStatus.Pending
    | 2uy -> GameStatus.AbortedWhilePending
    | 3uy -> GameStatus.Started
    | 4uy -> GameStatus.Aborted
    | 5uy -> GameStatus.Finished
    | _ -> raise <| Exception("Invalid game status")

let mapGameStatusToId (status : GameStatus) : byte =
    match status with
    | GameStatus.Pending -> 1uy
    | GameStatus.AbortedWhilePending -> 2uy
    | GameStatus.Started -> 3uy
    | GameStatus.Aborted -> 4uy
    | GameStatus.Finished -> 5uy

let mapGameResponse(sqlModel : GameSqlModel) : Game =
    {
        id = sqlModel.gameId
        status = sqlModel.gameStatusId |> mapGameStatusId
        createdOn = sqlModel.createdOn
        createdByUserId = sqlModel.createdByUserId
        parameters = 
            {
                regionCount = sqlModel.regionCount
                description = sqlModel.description |> Option.ofReference
                isPublic = sqlModel.isPublic
                allowGuests = sqlModel.allowGuests
            }
        players = List.empty
        pieces = JsonUtility.deserializeList sqlModel.piecesJson
        turnCycle = JsonUtility.deserializeList sqlModel.turnCycleJson
        currentTurn = JsonUtility.deserializeOption sqlModel.currentTurnJson
    }