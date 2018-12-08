namespace Djambi.Api.Db.Repositories

open Dapper
open Newtonsoft.Json
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Model
open Djambi.Api.Db.SqlUtility
open Djambi.Api.Model

module GameRepository =
    
    let startGame(request : StartGameRequest) : int AsyncHttpResult =
        let startingConditionsJson = request.startingConditions |> JsonConvert.SerializeObject
        let gameStateJson = request.gameState |> JsonConvert.SerializeObject
        let turnStateJson = request.turnState |> JsonConvert.SerializeObject

        let param = DynamicParameters()
                        .add("LobbyId", request.lobbyId)
                        .add("StartingConditionsJson", startingConditionsJson)
                        .add("GameStateJson", gameStateJson)
                        .add("TurnStateJson", turnStateJson)

        let cmd = proc("Games_Start", param)
        querySingle<int>(cmd, "Game")

    let getGame(gameId : int) : Game AsyncHttpResult =
        let param = DynamicParameters()
                        .add("GameId", gameId)
        let cmd = proc("Games_Get", param)

        querySingle<GameSqlModel>(cmd, "Game")
        |> thenMap mapGameSqlModelResponse

    let updateGameState(gameId : int, state : GameState) : Unit AsyncHttpResult =
        let json = state |> JsonConvert.SerializeObject
        let param = DynamicParameters()
                        .add("GameId", gameId)
                        .add("GameStateJson", json)

        let cmd = proc("Games_UpdateGameState", param)
        queryUnit(cmd, "Game")

    let updateTurnState(gameId: int, state : TurnState) : Unit AsyncHttpResult =
        let json = state |> JsonConvert.SerializeObject
        let param = DynamicParameters()
                        .add("GameId", gameId)
                        .add("TurnStateJson", json)
        
        let cmd = proc("Games_UpdateTurnState", param)
        queryUnit(cmd, "Game")