namespace Djambi.Api.Persistence

open Djambi.Api.Persistence.PlaySqlModels
open Djambi.Api.Model.Play
open Newtonsoft.Json
open Djambi.Api.Common.Enums

module PlaySqlModelMappings =

    let mapGameSqlModelResponse(sqlModel : GameSqlModel) : Game =
        {
            boardRegionCount = sqlModel.boardRegionCount
            currentGameState = JsonConvert.DeserializeObject<GameState>(sqlModel.currentGameStateJson)
            currentTurnState = 
                match sqlModel.currentTurnStateJson with
                | null -> { 
                            status = TurnStatus.AwaitingSelection
                            selections = List.empty 
                            selectionOptions = List.empty
                            requiredSelectionType = Some Subject
                          }
                | _ -> JsonConvert.DeserializeObject<TurnState>(sqlModel.currentTurnStateJson)
        }
    