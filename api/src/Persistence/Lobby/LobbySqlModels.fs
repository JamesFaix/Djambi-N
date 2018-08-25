namespace Djambi.Api.Persistence

module LobbySqlModels =

    open System

    [<CLIMutable>]
    type UserSqlModel = 
        {
            id : int
            name : string
            isGuest : bool
            isAdmin : bool
        }

    [<CLIMutable>]
    type LobbyGamePlayerSqlModel =
        {
            gameId : int
            gameDescription : string
            boardRegionCount : int
            gameStatusId : int
            userId : int Nullable
            playerName : string
            playerId : int Nullable
        }