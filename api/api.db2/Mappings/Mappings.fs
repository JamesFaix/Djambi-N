module Apex.Api.Db.Mappings

open Apex.Api.Db.Model
open Apex.Api.Model
open System.ComponentModel
open Apex.Api.Common.Json
open System

let toPrivilege (source : PrivilegeSqlModel) : Privilege =
    match source.Id with
    | 1uy -> Privilege.EditUsers
    | 2uy -> Privilege.EditPendingGames
    | 3uy -> Privilege.OpenParticipation
    | 4uy -> Privilege.Snapshots
    | 5uy -> Privilege.ViewGames
    | _ -> raise <| InvalidEnumArgumentException()
    
let toPrivilegeSqlId (source : Privilege) : byte =
    match source with
    | Privilege.EditUsers -> 1uy
    | Privilege.EditPendingGames -> 2uy
    | Privilege.OpenParticipation -> 3uy
    | Privilege.Snapshots -> 4uy
    | Privilege.ViewGames -> 5uy

let toUserDetails (source : UserSqlModel) : UserDetails =
    {
        id = source.Id
        name = source.Name
        password = source.Password
        failedLoginAttempts = int source.FailedLoginAttempts
        lastFailedLoginAttemptOn = source.LastFailedLoginAttemptOn |> Option.ofNullable
        privileges = source.Privileges |> Seq.map toPrivilege |> Seq.toList
    }    

let toUser (source : UserSqlModel) : User =
    {
        id = source.Id
        name = source.Name
        privileges = source.Privileges |> Seq.map toPrivilege |> Seq.toList
    } 

let toSession (source : SessionSqlModel) : Session =
    {
        id = source.Id
        token = source.Token
        user = source.User |> toUser
        expiresOn = source.ExpiresOn
        createdOn = source.CreatedOn
    }

let toGame (source : GameSqlModel) (players : seq<PlayerSqlModel>) : Game =
    raise <| NotImplementedException()

let toGameSqlModel (source : CreateGameRequest) : GameSqlModel =
    raise <| NotImplementedException()

let toPlayerSqlModel (source : Player) : PlayerSqlModel =
    raise <| NotImplementedException()

let createPlayerRequestToPlayerSqlModel (source : CreatePlayerRequest) : PlayerSqlModel =
    raise <| NotImplementedException()

let toEvent (source : EventSqlModel) : Event =
    raise <| NotImplementedException()

let toEventSqlModel (source : Event) : EventSqlModel =
    raise <| NotImplementedException()

let toSnapshotInfo (source : SnapshotSqlModel) : SnapshotInfo =
    {
        id = source.Id
        description = source.Description
        createdBy = {
            userId = source.CreatedByUser.Id
            userName = source.CreatedByUser.Name
            time = source.CreatedOn
        }
    }

let toSnapshot (source : SnapshotSqlModel) : Snapshot =
    let data = JsonUtility.deserialize<SnapshotJson> source.SnapshotJson
    {
        id = source.Id
        description = source.Description
        createdBy = {
            userId = source.CreatedByUser.Id
            userName = source.CreatedByUser.Name
            time = source.CreatedOn
        }
        game = data.game
        history = data.history
    }

let toSearchGame (source : GameSqlModel) : SearchGame =
    raise <| NotImplementedException()

let toGameStatusSqlId (source : GameStatus) : byte =
    raise <| NotImplementedException()
