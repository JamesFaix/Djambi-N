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
type GamesQuery =
    {
        gameId : int option
        descriptionContains : string option
        createdByUserId : int option
        playerUserId : int option
        isPublic : bool option
        allowGuests : bool option
    }

module GamesQuery =

    let empty : GamesQuery =
        {
            gameId = None
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

//---Internal requests

type UpdateFailedLoginsRequest =
    {
        userId : int
        failedLoginAttempts : int
        lastFailedLoginAttemptOn : DateTime option
    }

module UpdateFailedLoginsRequest =
    let reset (userId : int) = 
        {
            userId = userId
            failedLoginAttempts = 0
            lastFailedLoginAttemptOn = None
        }

    let increment (userId : int, attempts : int) =
        {
            userId = userId
            failedLoginAttempts = attempts
            lastFailedLoginAttemptOn = Some DateTime.UtcNow
        }

type CreateSessionRequest = 
    {
        userId : int
        token : string
        expiresOn : DateTime
    }

type SessionQuery = 
    {
        sessionId : int option
        token : string option
        userId : int option
    }

module SessionQuery = 
    let byId (sessionId : int) =
        {
            sessionId = Some sessionId
            token = None
            userId = None
        }
    
    let byToken (token : string) = 
        {
            sessionId = None
            token = Some token
            userId = None
        }

    let byUserId (userId : int) = 
        {
            sessionId = None
            token = None
            userId = Some userId
        }

type UpdateGameStateRequest = 
    {
        gameId : int
        status : GameStatus
        pieces : Piece list
        currentTurn : Turn option
        turnCycle : int list
    }

type SetPlayerStartConditionsRequest = 
    {
        playerId : int
        colorId : int
        startingRegion : int
        startingTurnNumber : int option
    }