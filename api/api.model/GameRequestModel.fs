[<AutoOpen>]
module Djambi.Api.Model.GameRequestModel

open System

[<CLIMutable>]
type CreatePlayerRequest = 
    {
        kind : PlayerKind
        userId : int option
        name : string option
    }

module CreatePlayerRequest =
    
    let user (userId : int) : CreatePlayerRequest =
        {
            kind = PlayerKind.User
            userId = Some userId
            name = None
        }

    let guest (userId : int, name : string) : CreatePlayerRequest =
        {
            kind = PlayerKind.Guest
            userId = Some userId
            name = Some name
        }

    let neutral (name : string) : CreatePlayerRequest =
        {
            kind = PlayerKind.Neutral
            userId = None
            name = Some name
        }   
        
[<CLIMutable>]
type CreateLobbyRequest =
    {
        description : string option
        regionCount : int
        isPublic : bool
        allowGuests : bool
    }
    
[<CLIMutable>]
type GamesQuery =
    {
        lobbyId : int option
        descriptionContains : string option
        createdByUserId : int option
        playerUserId : int option
        isPublic : bool option
        allowGuests : bool option
    }

module GamesQuery =

    let empty : GamesQuery =
        {
            lobbyId = None
            descriptionContains = None
            createdByUserId = None
            playerUserId = None
            isPublic = None
            allowGuests = None
        }

[<CLIMutable>]
type SelectionRequest =
    {
        cellId : int
    }
         
[<Obsolete>]
type StartGameResponse =
    {
        gameId : int
        startingConditions : PlayerStartConditions list
        gameState : GameState
        turnState : Turn
    }
   
[<Obsolete>] 
type CommitTurnResponse =
    {
        gameState : GameState
        turnState : Turn
    }