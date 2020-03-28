module Apex.Api.Db.Mapping

open Apex.Api.Common.Json
open Apex.Api.Db.Model
open Apex.Api.Model
open System.ComponentModel
open Apex.Api.Enums

let mapUserResponse (sqlModel : UserSqlModel) (privileges : Privilege list) : UserDetails =
    {
        id = sqlModel.userId
        name = sqlModel.name
        privileges = privileges
        password = sqlModel.password
        failedLoginAttempts = int sqlModel.failedLoginAttempts
        lastFailedLoginAttemptOn = sqlModel.lastFailedLoginAttemptOn |> Option.ofNullable
    }

let mapSessionResponse (sqlModel : SessionSqlModel) (user : User) : Session =
    {
        id = sqlModel.sessionId
        token = sqlModel.token
        createdOn = sqlModel.createdOn
        expiresOn = sqlModel.expiresOn
        user = user
    }

let mapPlayerResponse (sqlModel : PlayerSqlModel) : Player =
    {
        id = sqlModel.playerId
        gameId = sqlModel.gameId
        userId = sqlModel.userId |> Option.ofNullable
        kind = sqlModel.playerKindId
        name = sqlModel.name
        status = sqlModel.playerStatusId
        colorId = sqlModel.colorId |> Option.ofNullable |> Option.map int
        startingRegion = sqlModel.startingRegion |> Option.ofNullable |> Option.map int
        startingTurnNumber = sqlModel.startingTurnNumber |> Option.ofNullable |> Option.map int
    }

let mapGameResponse(sqlModel : GameSqlModel) : Game =
    {
        id = sqlModel.gameId
        status = sqlModel.gameStatusId
        createdBy = {
            userId = sqlModel.createdByUserId
            userName = sqlModel.createdByUserName
            time = sqlModel.createdOn
        }
        parameters =
            {
                regionCount = sqlModel.regionCount
                description = sqlModel.description |> Option.ofObj
                isPublic = sqlModel.isPublic
                allowGuests = sqlModel.allowGuests
            }
        players = []
        pieces = JsonUtility.deserializeList sqlModel.piecesJson
        turnCycle = JsonUtility.deserializeList sqlModel.turnCycleJson
        currentTurn = JsonUtility.deserializeOption sqlModel.currentTurnJson
    }
    
let mapSearchGameResponse(sqlModel : SearchGameSqlModel) : SearchGame =
    {
        id = sqlModel.gameId
        status = sqlModel.gameStatusId
        createdBy = {
            userId = sqlModel.createdByUserId
            userName = sqlModel.createdByUserName
            time = sqlModel.createdOn
        }
        parameters =
            {
                regionCount = sqlModel.regionCount
                description = sqlModel.description |> Option.ofObj
                isPublic = sqlModel.isPublic
                allowGuests = sqlModel.allowGuests
            }
        lastEventOn = sqlModel.lastEventOn
        playerCount = sqlModel.playerCount
        containsMe = sqlModel.containsMe
    }
    
let mapEventResponse (sqlModel : EventSqlModel) : Event =
    {
        id = sqlModel.eventId
        kind = sqlModel.eventKindId
        createdBy = {
            userId = sqlModel.createdByUserId
            userName = sqlModel.createdByUserName
            time = sqlModel.createdOn
        }
        actingPlayerId = sqlModel.actingPlayerId |> Option.ofNullable
        effects = JsonUtility.deserializeList sqlModel.effectsJson
    }

let mapResultsDirectionToAscendingBool (direction : ListSortDirection) : bool =
    direction = ListSortDirection.Ascending

let mapSnapshotFromSql (sqlModel : SnapshotSqlModel) : Snapshot =
    let data : SnapshotJson = JsonUtility.deserialize sqlModel.snapshotJson
    {
        id = sqlModel.snapshotId
        createdBy = {
            userId = sqlModel.createdByUserId
            userName = sqlModel.createdByUserName
            time = sqlModel.createdOn
        }
        description = sqlModel.description
        game = data.game
        history = data.history
    }

let mapSnapshotInfoFromSql (sqlModel : SnapshotSqlModel) : SnapshotInfo =
    {
        id = sqlModel.snapshotId
        createdBy = {
            userId = sqlModel.createdByUserId
            userName = sqlModel.createdByUserName
            time = sqlModel.createdOn
        }
        description = sqlModel.description
    }