namespace Djambi.Api.Persistence

module PlaySqlModels =
    
    [<CLIMutable>]
    type GameSqlModel =
        {
            gameId : int
            boardRegionCount : int
            currentGameStateJson : string
            currentTurnStateJson : string
        }
