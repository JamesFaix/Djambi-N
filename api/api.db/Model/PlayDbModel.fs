namespace Djambi.Api.Db.Model

module PlayDbModel =
    
    [<CLIMutable>]
    type GameSqlModel =
        {
            gameId : int
            boardRegionCount : int
            currentGameStateJson : string
            currentTurnStateJson : string
        }
