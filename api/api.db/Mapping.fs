module Djambi.Api.Db.Mapping

open Djambi.Api.Common
open Djambi.Api.Common.Json
open Djambi.Api.Db.Model
open Djambi.Api.Model

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
        2uy, AbortedWhilePending
        3uy, Started
        4uy, Aborted
        5uy, Finished
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
    ]

let mapEventKindId (eventKindId : byte) : EventKind =
    findRight eventKindsMap eventKindId

let mapEventKindToId (kind : EventKind) : byte =
    findLeft eventKindsMap kind

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
        token = sqlModel.token
        createdOn = sqlModel.createdOn
        expiresOn = sqlModel.expiresOn
        user = 
            {
                id = sqlModel.userId
                name = sqlModel.userName
                isAdmin = sqlModel.isAdmin
            }
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
        createdOn = sqlModel.createdOn
        createdByUserId = sqlModel.createdByUserId
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

let mapEventResponse (sqlModel : EventSqlModel) : Event =
    {
        id = sqlModel.eventId
        kind = sqlModel.eventKindId |> mapEventKindId
        createdByUserId = sqlModel.createdByUserId
        actingPlayerId = sqlModel.actingPlayerId |> Option.ofNullable
        createdOn = sqlModel.createdOn
        effects = JsonUtility.deserializeList sqlModel.effectsJson
    }

let mapResultsDirectionToAscendingBool (direction : ResultsDirection) : bool =
    direction = ResultsDirection.Ascending