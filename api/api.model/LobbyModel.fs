module Djambi.Api.Model.LobbyModel

open System
open Djambi.Api.Model.Enums

type LobbyPlayer =
    {
        id : int
        lobbyId : int
        userId : int option
        playerType : PlayerType
        name : string
    }

type CreatePlayerRequest = 
    {
        lobbyId : int
        playerType : PlayerType
        userId : int option
        name : string option
    }

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

type CreateLobbyRequest = 
    {
        description : string option
        regionCount : int
        createdByUserId : int
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