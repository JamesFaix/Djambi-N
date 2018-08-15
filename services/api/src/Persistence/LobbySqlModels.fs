namespace Djambi.Api.Persistence

module LobbySqlModels =

    [<CLIMutable>]
    type UserSqlModel = 
        {
            id : int
            name : string
        }

    [<CLIMutable>]
    type GameSqlModel =
        {
            id : int
            description : string
            status : int
            boardRegionCount : int
        }

    [<CLIMutable>]
    type OpenGamePlayerSqlModel =
        {
            gameId : int
            gameDescription : string
            boardRegionCount : int
            userId : int
            userName : string
        }