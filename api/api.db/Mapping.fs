module Apex.Api.Db.Mapping

open Apex.Api.Common
open Apex.Api.Common.Json
open Apex.Api.Db.Model
open Apex.Api.Model

let private findRight<'a, 'b when 'a : equality> (map : ('a * 'b) list) (key : 'a) : 'b =
    let result = map |> List.tryFind(fun (a, _) -> a = key)
    match result with
    | Some (_, b) -> b
    | _ -> failwith "Invalid enum value"


let private findLeft<'a, 'b when 'b : equality> (map : ('a * 'b) list) (key : 'b) : 'a =
    let result = map |> List.tryFind(fun (_, b) -> b = key)
    match result with
    | Some (a, _) -> a
    | _ -> failwith "Invalid enum value"

let private playerKindsMap =
    [
        1uy, User
        2uy, Guest
        3uy, Neutral
    ]

let mapPlayerKindId (playerKindId : byte) : PlayerKind =
    findRight playerKindsMap playerKindId

let mapPlayerKindToId (kind : PlayerKind) : byte =
    findLeft playerKindsMap kind

let private playerStatusMap =
    [
        1uy, PlayerStatus.Pending
        2uy, Alive
        3uy, Eliminated
        4uy, Conceded
        5uy, WillConcede
        6uy, AcceptsDraw
        7uy, Victorious
    ]

let mapPlayerStatusId (playerStatusId : byte) : PlayerStatus =
    findRight playerStatusMap playerStatusId

let mapPlayerStatusToId (status : PlayerStatus) : byte =
    findLeft playerStatusMap status

let private gameStatusMap =
    [
        1uy, GameStatus.Pending
        2uy, InProgress
        3uy, Canceled
        4uy, Over
    ]

let mapGameStatusId (gameStatusId : byte) : GameStatus =
    findRight gameStatusMap gameStatusId

let mapGameStatusToId (status : GameStatus) : byte =
    findLeft gameStatusMap status

let private eventKindsMap =
    [
        1uy, EventKind.GameParametersChanged
        2uy, EventKind.GameCanceled
        3uy, EventKind.PlayerJoined
        4uy, EventKind.PlayerRemoved
        5uy, EventKind.GameStarted
        6uy, EventKind.TurnCommitted
        7uy, EventKind.TurnReset
        8uy, EventKind.CellSelected
        9uy, EventKind.PlayerStatusChanged
    ]

let mapEventKindId (eventKindId : byte) : EventKind =
    findRight eventKindsMap eventKindId

let mapEventKindToId (kind : EventKind) : byte =
    findLeft eventKindsMap kind

let privilegeMap =
    [
        1uy, Privilege.EditUsers
        2uy, Privilege.EditPendingGames
        3uy, Privilege.OpenParticipation
        4uy, Privilege.ViewGames
        5uy, Privilege.Snapshots
    ]

let mapPrivilegeId (privilegeId : byte) : Privilege =
    findRight privilegeMap privilegeId

let mapPrivilegeToId (privilege : Privilege) : byte =
    findLeft privilegeMap privilege

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
        kind = mapPlayerKindId sqlModel.playerKindId
        name = sqlModel.name
        status = sqlModel.playerStatusId |> mapPlayerStatusId
        colorId = sqlModel.colorId |> Option.ofNullable |> Option.map int
        startingRegion = sqlModel.startingRegion |> Option.ofNullable |> Option.map int
        startingTurnNumber = sqlModel.startingTurnNumber |> Option.ofNullable |> Option.map int
    }

let mapGameResponse(sqlModel : GameSqlModel) : Game =
    {
        id = sqlModel.gameId
        status = sqlModel.gameStatusId |> mapGameStatusId
        createdBy = {
            userId = sqlModel.createdByUserId
            userName = sqlModel.createdByUserName
            time = sqlModel.createdOn
        }
        parameters =
            {
                regionCount = sqlModel.regionCount
                description = sqlModel.description |> Option.ofReference
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
        status = sqlModel.gameStatusId |> mapGameStatusId
        createdBy = {
            userId = sqlModel.createdByUserId
            userName = sqlModel.createdByUserName
            time = sqlModel.createdOn
        }
        parameters =
            {
                regionCount = sqlModel.regionCount
                description = sqlModel.description |> Option.ofReference
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
        kind = sqlModel.eventKindId |> mapEventKindId
        createdBy = {
            userId = sqlModel.createdByUserId
            userName = sqlModel.createdByUserName
            time = sqlModel.createdOn
        }
        actingPlayerId = sqlModel.actingPlayerId |> Option.ofNullable
        effects = JsonUtility.deserializeList sqlModel.effectsJson
    }

let mapResultsDirectionToAscendingBool (direction : ResultsDirection) : bool =
    direction = ResultsDirection.Ascending

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