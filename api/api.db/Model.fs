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
        token : string
        createdOn : DateTime
        expiresOn : DateTime
        isAdmin : bool
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