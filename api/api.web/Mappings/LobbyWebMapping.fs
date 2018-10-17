module Djambi.Api.Web.Mappings.LobbyWebMapping

open Djambi.Api.Model.LobbyModel
open Djambi.Api.Web.Model.LobbyWebModel
open Djambi.Api.Common.Utilities
open Djambi.Api.Web.Mappings.PlayerWebMapping

let mapLobbyResponse(lobby : Lobby) : LobbyResponseJsonModel =
    {
        id = lobby.id
        regionCount = lobby.regionCount
        description = lobby.description |> optionToReference
        isPublic = lobby.isPublic
        allowGuests = lobby.allowGuests
    }

let mapLobbyWithPlayersResponse(lobby : LobbyWithPlayers) : LobbyWithPlayersResponseJsonModel =
    {
        id = lobby.id
        regionCount = lobby.regionCount
        description = lobby.description |> optionToReference
        isPublic = lobby.isPublic
        allowGuests = lobby.allowGuests
        players = lobby.players |> List.map mapPlayerResponse
    }

let mapCreateLobbyRequest(jsonModel : CreateLobbyJsonModel, sessionUserId : int) : CreateLobbyRequest =
    {
        regionCount = jsonModel.regionCount
        description = jsonModel.description |> referenceToOption
        createdByUserId = sessionUserId
        isPublic = jsonModel.isPublic
        allowGuests = jsonModel.allowGuests
    }

let mapLobbiesQuery(jsonModel : LobbiesQueryJsonModel) : LobbiesQuery =
    {
        lobbyId = None
        descriptionContains = jsonModel.descriptionContains |> referenceToOption
        createdByUserId = jsonModel.createdByUserId |> nullableToOption
        playerUserId = jsonModel.playerUserId |> nullableToOption
        isPublic = jsonModel.isPublic |> nullableToOption
        allowGuests = jsonModel.allowGuests |> nullableToOption    
    }