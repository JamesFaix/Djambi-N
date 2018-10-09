namespace Djambi.Api.Db.Repositories

open System.Threading.Tasks
open Dapper
open FSharp.Control.Tasks
open Newtonsoft.Json
open Djambi.Api.Db.Mappings.PlayDbMapping
open Djambi.Api.Db.Model.PlayDbModel
open Djambi.Api.Db.SqlUtility
open Djambi.Api.Model.PlayModel

module PlayRepository =
    
    let startGame(request : UpdateGameForStartRequest) : Unit Task =
        let startingConditionsJson = request.startingConditions |> JsonConvert.SerializeObject
        let currentGameStateJson = request.currentGameState |> JsonConvert.SerializeObject
        let currentTurnStateJson = request.currentTurnState |> JsonConvert.SerializeObject

        let param = new DynamicParameters()
        param.Add("GameId", request.id)
        param.Add("StartingConditionsJson", startingConditionsJson)
        param.Add("CurrentGameStateJson", currentGameStateJson)
        param.Add("CurrentTurnStateJson", currentTurnStateJson)
        let cmd = proc("Play.UpdateGameForStart", param)
        queryUnit(cmd, "Game")

    let getGame(gameId : int) : Game Task =
        let param = new DynamicParameters()
        param.Add("GameId", gameId)
        let cmd = proc("Play.GetGame", param)

        task {            
            let! sqlModel = querySingle<GameSqlModel>(cmd, "Game")
            return sqlModel |> mapGameSqlModelResponse
        }

    let updateCurrentGameState(gameId : int, state : GameState) : Unit Task =
        let json = state |> JsonConvert.SerializeObject
        let param = new DynamicParameters()
        param.Add("GameId", gameId)
        param.Add("CurrentGameStateJson", json)
        let cmd = proc("Play.UpdateCurrentGameState", param)
        queryUnit(cmd, "Game")

    let updateCurrentTurnState(gameId: int, state : TurnState) : Unit Task =
        let json = state |> JsonConvert.SerializeObject
        let param = new DynamicParameters()
        param.Add("GameId", gameId)
        param.Add("CurrentTurnStateJson", json)
        let cmd = proc("Play.UpdateCurrentTurnState", param)
        queryUnit(cmd, "Game")