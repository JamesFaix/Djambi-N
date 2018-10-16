module Djambi.Api.Db.Model.PlayDbModel

open Newtonsoft.Json
open Djambi.Api.Model.PlayModel
    
[<CLIMutable>]
type GameSqlModel =
    {
        gameId : int
        regionCount : int
        gameStateJson : string
        turnStateJson : string
    }

module GameSqlModel =
    
    let toModel (sqlModel : GameSqlModel) : Game =
        {
            regionCount = sqlModel.regionCount
            gameState = JsonConvert.DeserializeObject<GameState>(sqlModel.gameStateJson)
            turnState = 
                match sqlModel.turnStateJson with
                | null -> TurnState.empty
                | _ -> JsonConvert.DeserializeObject<TurnState>(sqlModel.turnStateJson)
        }
    