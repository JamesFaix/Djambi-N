module Djambi.Api.Db.Model.LobbyDbModel

open System
open Djambi.Api.Common.Utilities
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