module Djambi.Api.Web.Mappings.PlayerWebMapping

open Djambi.Api.Model.PlayerModel
open Djambi.Api.Web.Model.PlayerWebModel
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