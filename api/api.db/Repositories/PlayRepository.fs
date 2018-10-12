namespace Djambi.Api.Db.Repositories

open Dapper
open Newtonsoft.Json
open Djambi.Api.Common
open Djambi.Api.Db.Mappings.PlayDbMapping
open Djambi.Api.Db.Model.PlayDbModel
open Djambi.Api.Db.SqlUtility
open Djambi.Api.Model.PlayModel

module PlayRepository =
    
    let startGame(request : UpdateGameForStartRequest) : Unit AsyncHttpResult =
        let startingConditionsJson = request.startingConditions |> JsonConvert.SerializeObject
        let currentGameStateJson = request.currentGameState |> JsonConvert.SerializeObject
        let currentTurnStateJson = request.currentTurnState |> JsonConvert.SerializeObject

        let param = DynamicParameters()
                        .add("GameId", request.id)
                        .add("StartingConditionsJson", startingConditionsJson)
                        .add("CurrentGameStateJson", currentGameStateJson)
                        .add("CurrentTurnStateJson", currentTurnStateJson)

        let cmd = proc("Play.UpdateGameForStart", param)
        queryUnit(cmd, "Game")

    let getGame(gameId : int) : Game AsyncHttpResult =
        let param = DynamicParameters()
                        .add("GameId", gameId)
        let cmd = proc("Play.GetGame", param)

        querySingle<GameSqlModel>(cmd, "Game")
        |> Task.thenMap mapGameSqlModelResponse

    let updateCurrentGameState(gameId : int, state : GameState) : Unit AsyncHttpResult =
        let json = state |> JsonConvert.SerializeObject
        let param = DynamicParameters()
                        .add("GameId", gameId)
                        .add("CurrentGameStateJson", json)

        let cmd = proc("Play.UpdateCurrentGameState", param)
        queryUnit(cmd, "Game")

    let updateCurrentTurnState(gameId: int, state : TurnState) : Unit AsyncHttpResult =
        let json = state |> JsonConvert.SerializeObject
        let param = DynamicParameters()
                        .add("GameId", gameId)
                        .add("CurrentTurnStateJson", json)
        
        let cmd = proc("Play.UpdateCurrentTurnState", param)
        queryUnit(cmd, "Game")