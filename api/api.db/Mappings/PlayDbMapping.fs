namespace Djambi.Api.Db.Mappings

open Newtonsoft.Json
open Djambi.Api.Common.Enums
open Djambi.Api.Db.Model.PlayDbModel
open Djambi.Api.Model.PlayModel

module PlayDbMapping =

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
    