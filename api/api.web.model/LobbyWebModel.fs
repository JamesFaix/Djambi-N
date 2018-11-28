module Djambi.Api.Web.Model.LobbyWebModel

open System
open Djambi.Api.Web.Model.PlayerWebModel

[<CLIMutable>]
type CreateLobbyJsonModel =
    {
        regionCount : int
        description : string
        allowGuests : bool
        isPublic : bool
    }

type LobbyResponseJsonModel =
    {
        id : int
        regionCount : int
        description : string
        allowGuests : bool
        isPublic : bool
        createdByUserId : int
        createdOn : DateTime
    }

type LobbyWithPlayersResponseJsonModel =
    {
        id : int
        regionCount : int
        description : string
        allowGuests : bool
        isPublic : bool
        players : PlayerResponseJsonModel list
        createdByUserId : int
        createdOn : DateTime
    }

[<CLIMutable>]
type LobbiesQueryJsonModel =
    {
        descriptionContains : string
        createdByUserId : int Nullable
        playerUserId : int Nullable
        isPublic : bool Nullable
        allowGuests : bool Nullable
    }

module LobbiesQueryJsonModel =
    let empty =
        {
            descriptionContains = Unchecked.defaultof<string>
            createdByUserId = Unchecked.defaultof<int Nullable>
            playerUserId = Unchecked.defaultof<int Nullable>
            isPublic = Unchecked.defaultof<bool Nullable>
            allowGuests = Unchecked.defaultof<bool Nullable>
        }