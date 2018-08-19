namespace Djambi.Api.Persistence

module PlaySqlModelMappings =

    open Djambi.Api.Persistence.PlaySqlModels
    open Djambi.Api.Domain.PlayModels
    open Newtonsoft.Json

    let mapGameSqlModelResponse(sqlModel : GameSqlModel) : Game =
        {
            boardRegionCount = sqlModel.boardRegionCount
            currentGameState = JsonConvert.DeserializeObject<GameState>(sqlModel.currentGameStateJson)
            currentTurnState = JsonConvert.DeserializeObject<TurnState>(sqlModel.currentTurnStateJson)
        }
    