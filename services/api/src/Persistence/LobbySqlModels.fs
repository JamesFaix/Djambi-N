namespace Djambi.Api.Persistence

module LobbySqlModels =

    open System

    [<CLIMutable>]
    type UserSqlModel = 
        {
            id : int
            name : string
        }

    [<CLIMutable>]
    type LobbyGamePlayerSqlModel =
        {
            gameId : int
            gameDescription : string
            boardRegionCount : int
            gameStatusId : int
            userId : int Nullable
            userName : string
        }