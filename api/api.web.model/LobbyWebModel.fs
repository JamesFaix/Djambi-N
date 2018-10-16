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

type LobbyPlayerResponseJsonModel =
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
        status : string
    }

type LobbyWithPlayersResponseJsonModel = 
    {
        id : int
        regionCount : int
        description : string        
        allowGuests : bool
        isPublic : bool
        status : string
        players : LobbyPlayerResponseJsonModel list
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

[<CLIMutable>]
type CreatePlayerJsonModel =
    {
        userId : int Nullable
        name : string
        ``type`` : string
    }