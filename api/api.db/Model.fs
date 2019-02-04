module Djambi.Api.Db.Model

open System

[<CLIMutable>]
type UserSqlModel = 
    {
        userId : int
        name : string
        isAdmin : bool
        password : string
        failedLoginAttempts : byte
        lastFailedLoginAttemptOn : DateTime Nullable
    }

[<CLIMutable>]
type SessionSqlModel =
    {
        sessionId : int
        userId : int
        userName : string
        isAdmin : bool
        token : string
        createdOn : DateTime
        expiresOn : DateTime
    }
    
[<CLIMutable>]
type PlayerSqlModel =
    {
        playerId : int
        gameId : int
        userId : int Nullable
        name : string
        playerKindId : byte
        playerStatusId : byte
        colorId : byte Nullable
        startingRegion : byte Nullable
        startingTurnNumber : byte Nullable
    }
    
[<CLIMutable>]
type GameSqlModel =
    {
        gameId : int
        createdOn : DateTime
        createdByUserId : int
        gameStatusId : byte
        //Game parameters
        description : string
        regionCount : int
        isPublic : bool
        allowGuests : bool
        //State
        turnCycleJson : string
        piecesJson : string
        currentTurnJson : string
    }

[<CLIMutable>]
type EventSqlModel =
    {
        eventId : int
        gameId : int
        createdByUserId : int
        actingPlayerId : int Nullable
        createdOn : DateTime
        eventKindId : byte
        effectsJson : string
    }