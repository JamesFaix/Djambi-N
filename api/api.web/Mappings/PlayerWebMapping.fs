[<AutoOpen>]
module Djambi.Api.Web.Mappings.PlayerWebMapping

open Djambi.Api.Model
open Djambi.Api.Web.Model

let mapCreatePlayerRequest (jsonModel : CreatePlayerJsonModel, lobbyId : int)  : CreatePlayerRequest =
    {
        lobbyId = lobbyId
        userId = jsonModel.userId
        name = jsonModel.name
        playerType = jsonModel.``type``
    }

let mapPlayerResponse(player : Player) : PlayerResponseJsonModel =
    {
        id = player.id
        userId = player.userId
        name = player.name
        ``type`` = player.playerType
    }