module Djambi.Api.Web.Mappings.LobbyWebMapping

open Djambi.Api.Model.LobbyModel
open Djambi.Api.Web.Model.LobbyWebModel
open Djambi.Api.Model.Enums
open Djambi.Api.Common.Utilities

let mapPlayerTypeFromString(playerTypeName : string) : PlayerType =
    match playerTypeName.ToUpperInvariant() with
    | "USER" -> PlayerType.User
    | "GUEST" -> PlayerType.Guest
    | "VIRTUAL" -> PlayerType.Virtual
    | _ -> failwith ("Invalid player type name: " + playerTypeName)

let mapCreatePlayerRequest (jsonModel : CreatePlayerJsonModel, lobbyId : int)  : CreatePlayerRequest =
    {
        lobbyId = lobbyId
        userId = jsonModel.userId |> nullableToOption
        name = jsonModel.name |> referenceToOption
        playerType = jsonModel.``type`` |> mapPlayerTypeFromString
    }

let mapPlayerResponse(player : Player) : PlayerResponseJsonModel =
    {
        id = player.id
        userId = player.userId |> optionToNullable
        name = player.name
        ``type`` = player.playerType.ToString()
    }

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