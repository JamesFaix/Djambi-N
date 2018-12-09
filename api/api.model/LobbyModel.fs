[<AutoOpen>]
module Djambi.Api.Model.LobbyModel

open System

type Lobby =
    {
        id : int
        description : string option
        regionCount : int
        createdOn : DateTime
        createdByUserId : int
        isPublic : bool
        allowGuests : bool
        //TODO: Add player count
    }

type LobbyWithPlayers =
    {
        id : int
        description : string option
        regionCount : int
        createdOn : DateTime
        createdByUserId : int
        isPublic : bool
        allowGuests : bool
        players : Player list
    }

type Lobby with
    member this.addPlayers (players : Player list) : LobbyWithPlayers =
        {
            id = this.id
            description = this.description
            regionCount = this.regionCount
            createdOn = this.createdOn
            createdByUserId = this.createdByUserId
            isPublic = this.isPublic
            allowGuests = this.allowGuests
            players = players
        }

[<CLIMutable>]
type CreateLobbyRequest =
    {
        description : string option
        regionCount : int
        isPublic : bool
        allowGuests : bool
    }

type LobbiesQuery =
    {
        lobbyId : int option
        descriptionContains : string option
        createdByUserId : int option
        playerUserId : int option
        isPublic : bool option
        allowGuests : bool option
    }

module LobbiesQuery =

    let empty : LobbiesQuery =
        {
            lobbyId = None
            descriptionContains = None
            createdByUserId = None
            playerUserId = None
            isPublic = None
            allowGuests = None
        }