namespace Djambi.Api.Persistence

module LobbySqlModels =

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
        }

    [<CLIMutable>]
    type SessionSqlModel =
        {
            sessionId : int
            userId : int
            token : string
            createdOn : DateTime
            expiresOn : DateTime        
        }