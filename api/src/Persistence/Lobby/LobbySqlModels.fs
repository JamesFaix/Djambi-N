namespace Djambi.Api.Persistence

module LobbySqlModels =

    open System

    [<CLIMutable>]
    type UserSqlModel = 
        {
            id : int
            name : string
            roleId : byte
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