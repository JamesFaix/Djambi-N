module Djambi.Api.Web.Model.LobbyWebModel

open System
       
[<CLIMutable>]
type CreateLobbyJsonModel =
    {
        regionCount : int    
        description : string
        allowGuests : bool
        isPublic : bool
    }

type PlayerResponseJsonModel =
    {
        id : int
        userId : int Nullable
        name : string
        ``type`` : string
    }

type LobbyResponseJsonModel =
    {
        id : int
        regionCount : int
        description : string        
        allowGuests : bool
        isPublic : bool
    }

type LobbyWithPlayersResponseJsonModel = 
    {
        id : int
        regionCount : int
        description : string        
        allowGuests : bool
        isPublic : bool
        players : PlayerResponseJsonModel list
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

[<CLIMutable>]
type CreatePlayerJsonModel =
    {
        userId : int Nullable
        name : string
        ``type`` : string
    }