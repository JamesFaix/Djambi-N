module Djambi.Api.Web.Model.LobbyWebModel

open System
       
[<CLIMutable>]
type CreateLobbyJsonModel =
    {
        regionCount : int    
        description : string
        allowGuestPlayers : bool
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
        allowGuestPlayers : bool
        isPublic : bool
        status : string
        players : LobbyPlayerResponseJsonModel list
    }