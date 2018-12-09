[<AutoOpen>]
module Djambi.Api.Web.Model.LobbyWebModel

open System

[<CLIMutable>]
type CreateLobbyJsonModel =
    {
        regionCount : int
        description : string option
        allowGuests : bool
        isPublic : bool
    }

type LobbyResponseJsonModel =
    {
        id : int
        regionCount : int
        description : string option
        allowGuests : bool
        isPublic : bool
        createdByUserId : int
        createdOn : DateTime
    }

type LobbyWithPlayersResponseJsonModel =
    {
        id : int
        regionCount : int
        description : string option
        allowGuests : bool
        isPublic : bool
        players : PlayerResponseJsonModel list
        createdByUserId : int
        createdOn : DateTime
    }

[<CLIMutable>]
type LobbiesQueryJsonModel =
    {
        descriptionContains : string option
        createdByUserId : int option
        playerUserId : int option
        isPublic : bool option
        allowGuests : bool option
    }

module LobbiesQueryJsonModel =
    let empty =
        {
            descriptionContains = None
            createdByUserId = None
            playerUserId = None
            isPublic = None
            allowGuests = None
        }