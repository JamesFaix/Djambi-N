module Djambi.Api.Model.LobbyModel

open System
open Djambi.Api.Model.Enums

type Player =
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

module CreatePlayerRequest =
    
    let user (lobbyId : int, userId : int) : CreatePlayerRequest =
        {
            lobbyId = lobbyId
            playerType = PlayerType.User
            userId = Some userId
            name = None
        }

    let guest (lobbyId : int, userId : int, name : string) : CreatePlayerRequest =
        {
            lobbyId = lobbyId
            playerType = PlayerType.Guest
            userId = Some userId
            name = Some name
        }

    let virtual (lobbyId : int, name : string) : CreatePlayerRequest =
        {
            lobbyId = lobbyId
            playerType = PlayerType.Virtual
            userId = None
            name = Some name
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