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
    
module LobbySqlModel =

    let toModel (sqlModel : LobbySqlModel) : Lobby =
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

[<CLIMutable>]
type PlayerSqlModel =
    {
        playerId : int
        lobbyId : int
        userId : int Nullable
        name : string    
        playerTypeId : byte
    }

module PlayerSqlModel =
        
    let typeIdToModel (playerTypeId : byte) : PlayerType =
        match playerTypeId with
        | 1uy -> PlayerType.User
        | 2uy -> PlayerType.Guest
        | 3uy -> PlayerType.Virtual
        | _ -> raise <| Exception("Invalid player type")

    let typeIdFromModel (playerType : PlayerType) : byte =
        match playerType with
        | PlayerType.User -> 1uy
        | PlayerType.Guest -> 2uy
        | PlayerType.Virtual -> 3uy

    let toModel (sqlModel : PlayerSqlModel) : Player =
        {
            id = sqlModel.playerId
            lobbyId = sqlModel.lobbyId
            userId = sqlModel.userId |> nullableToOption
            playerType = typeIdToModel sqlModel.playerTypeId
            name = sqlModel.name
        }