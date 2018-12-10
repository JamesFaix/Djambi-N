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
        lobbyId : int
        userId : int Nullable
        name : string
        playerTypeId : byte
        isAlive : bool
    }

[<CLIMutable>]
type GameParametersSqlModel =
    {
        lobbyId : int
        description : string
        regionCount : int
        createdOn : DateTime
        createdByUserId : int
        isPublic : bool
        allowGuests : bool
        //TODO: Add player count
    }
    
[<CLIMutable>]
type GameSqlModel =
    {
        gameId : int
        regionCount : int
        gameStateJson : string
        turnStateJson : string
    }