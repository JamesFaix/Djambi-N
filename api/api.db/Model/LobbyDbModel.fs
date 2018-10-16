module Djambi.Api.Db.Model.LobbyDbModel

open System
open Djambi.Api.Common.Utilities
open Djambi.Api.Model.Enums
open Djambi.Api.Model.LobbyModel

[<CLIMutable>]
type LobbySqlModel =
    {
        lobbyId : int
        description : string
        regionCount : int
        createdOn : DateTime
        createdByUserId : int
        isPublic : bool
        allowGuests : bool
        //TODO: Add player count
    }

[<CLIMutable>]
type LobbyPlayerSqlModel =
    {
        lobbyPlayerId : int
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

let mapLobbyPlayer (sqlModel : LobbyPlayerSqlModel) : LobbyPlayer =
    {
        id = sqlModel.lobbyPlayerId
        lobbyId = sqlModel.lobbyId
        userId = sqlModel.userId |> nullableToOption
        playerType = mapPlayerTypeId sqlModel.playerTypeId
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