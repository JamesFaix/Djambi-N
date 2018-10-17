module Djambi.Api.Db.Model.PlayerDbModel

open System
open Djambi.Api.Common.Utilities
open Djambi.Api.Model.PlayerModel

[<CLIMutable>]
type PlayerSqlModel =
    {
        playerId : int
        lobbyId : int
        userId : int Nullable
        name : string    
        playerTypeId : byte
    }

let mapPlayerTypeId (playerTypeId : byte) : PlayerType =
    match playerTypeId with
    | 1uy -> PlayerType.User
    | 2uy -> PlayerType.Guest
    | 3uy -> PlayerType.Virtual
    | _ -> raise <| Exception("Invalid player type")

let mapPlayerTypeToId (playerType : PlayerType) : byte =
    match playerType with
    | PlayerType.User -> 1uy
    | PlayerType.Guest -> 2uy
    | PlayerType.Virtual -> 3uy

let mapPlayer (sqlModel : PlayerSqlModel) : Player =
    {
        id = sqlModel.playerId
        lobbyId = sqlModel.lobbyId
        userId = sqlModel.userId |> nullableToOption
        playerType = mapPlayerTypeId sqlModel.playerTypeId
        name = sqlModel.name
    }    