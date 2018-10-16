module Djambi.Api.Db.Model.PlayDbModel

open Newtonsoft.Json
open Djambi.Api.Model.Enums
open Djambi.Api.Model.PlayModel
    
[<CLIMutable>]
type GameSqlModel =
    {
        gameId : int
        boardRegionCount : int
        currentGameStateJson : string
        currentTurnStateJson : string
    }

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
    