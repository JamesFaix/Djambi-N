[<AutoOpen>]
module Djambi.Api.Db.Model.PlayerDbModel

open System
open Djambi.Api.Common.Utilities
open Djambi.Api.Model

[<CLIMutable>]
type PlayerSqlModel =
    {
        playerId : int
        lobbyId : int
        userId : int Nullable
        name : string
        playerTypeId : byte
    }

let mapPlayerTypeId (playerTypeId : byte) : PlayerKind =
    match playerTypeId with
    | 1uy -> PlayerKind.User
    | 2uy -> PlayerKind.Guest
    | 3uy -> PlayerKind.Virtual
    | _ -> raise <| Exception("Invalid player type")

let mapPlayerKindToId (kind : PlayerKind) : byte =
    match kind with
    | PlayerKind.User -> 1uy
    | PlayerKind.Guest -> 2uy
    | PlayerKind.Virtual -> 3uy

let mapPlayer (sqlModel : PlayerSqlModel) : Player =
    {
        id = sqlModel.playerId
        lobbyId = sqlModel.lobbyId
        userId = sqlModel.userId |> nullableToOption
        kind = mapPlayerTypeId sqlModel.playerTypeId
        name = sqlModel.name
    }