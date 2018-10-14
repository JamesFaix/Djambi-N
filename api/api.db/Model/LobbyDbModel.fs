module Djambi.Api.Db.Model.LobbyDbModel

open System

[<CLIMutable>]
type UserSqlModel = 
    {
        id : int
        name : string
        roleId : byte
        password : string
        failedLoginAttempts : byte
        lastFailedLoginAttemptOn : Nullable<DateTime>
    }

[<CLIMutable>]
type LobbyGamePlayerSqlModel =
    {
        gameId : int
        gameDescription : string
        boardRegionCount : int
        gameStatusId : byte
        userId : int Nullable
        playerName : string
        playerId : int Nullable
        createdByUserId : int
    }

[<CLIMutable>]
type SessionUserSqlModel =
    {
        sessionId : int
        userId : Nullable<int>
        token : string
        primaryUserId : int
        createdOn : DateTime
        expiresOn : DateTime
        isShared : bool   
    }