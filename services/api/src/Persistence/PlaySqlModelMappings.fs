namespace Djambi.Api.Persistence

module PlaySqlModelMappings =

    open Djambi.Api.Persistence.PlaySqlModels
    open Djambi.Api.Domain.PlayModels
    open Newtonsoft.Json
    open Djambi.Api.Common.Enums

    let mapGameSqlModelResponse(sqlModel : GameSqlModel) : Game =
        {
            boardRegionCount = sqlModel.boardRegionCount
            currentGameState = JsonConvert.DeserializeObject<GameState>(sqlModel.currentGameStateJson)
            currentTurnState = 
                match sqlModel.currentTurnStateJson with
                | null -> { 
                            selections = List.empty 
                            status = TurnStatus.AwaitingSelection
                          }
                | _ -> JsonConvert.DeserializeObject<TurnState>(sqlModel.currentTurnStateJson)
        }
    