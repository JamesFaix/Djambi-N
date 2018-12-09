[<AutoOpen>]
module Djambi.Api.Web.Mappings.LobbyWebMapping

open Djambi.Api.Model
open Djambi.Api.Web.Model

let mapLobbyResponse(lobby : Lobby) : LobbyResponseJsonModel =
    {
        id = lobby.id
        regionCount = lobby.regionCount
        description = lobby.description
        isPublic = lobby.isPublic
        allowGuests = lobby.allowGuests
        createdByUserId = lobby.createdByUserId
        createdOn = lobby.createdOn
    }

let mapLobbyWithPlayersResponse(lobby : LobbyWithPlayers) : LobbyWithPlayersResponseJsonModel =
    {
        id = lobby.id
        regionCount = lobby.regionCount
        description = lobby.description
        isPublic = lobby.isPublic
        allowGuests = lobby.allowGuests
        createdByUserId = lobby.createdByUserId
        createdOn = lobby.createdOn
        players = lobby.players
    }

let mapCreateLobbyRequest(jsonModel : CreateLobbyJsonModel) : CreateLobbyRequest =
    {
        regionCount = jsonModel.regionCount
        description = jsonModel.description
        isPublic = jsonModel.isPublic
        allowGuests = jsonModel.allowGuests
    }

let mapLobbiesQuery(jsonModel : LobbiesQueryJsonModel) : LobbiesQuery =
    {
        lobbyId = None
        descriptionContains = jsonModel.descriptionContains
        createdByUserId = jsonModel.createdByUserId
        playerUserId = jsonModel.playerUserId
        isPublic = jsonModel.isPublic
        allowGuests = jsonModel.allowGuests
    }