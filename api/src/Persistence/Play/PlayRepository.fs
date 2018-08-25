namespace Djambi.Api.Persistence

open System.Threading.Tasks

open Dapper
open Giraffe
open Newtonsoft.Json

open Djambi.Api.Domain.PlayModels
open Djambi.Api.Persistence.PlaySqlModels
open Djambi.Api.Persistence.SqlUtility

open Djambi.Api.Persistence.PlaySqlModelMappings

module PlayRepository =
    
    let getGame(gameId : int) : Game Task =
        let param = new DynamicParameters()
        param.Add("GameId", gameId)
        let cmd = proc("Play.Get_Game", param)

        task {            
            use cn = getConnection()
            let! sqlModel = cn.QuerySingleAsync<GameSqlModel>(cmd)
            return sqlModel |> mapGameSqlModelResponse
        }

    let updateCurrentGameState(gameId : int, state : GameState) : Unit Task =
        let json = state |> JsonConvert.SerializeObject
        let param = new DynamicParameters()
        param.Add("GameId", gameId)
        param.Add("CurrentGameStateJson", json)
        let cmd = proc("Play.Update_CurrentGameState", param)
          
        task {            
            use cn = getConnection()
            let! _ = cn.ExecuteAsync(cmd)
            return ()
        }

    let updateCurrentTurnState(gameId: int, state : TurnState) : Unit Task =
        let json = state |> JsonConvert.SerializeObject
        let param = new DynamicParameters()
        param.Add("GameId", gameId)
        param.Add("CurrentTurnStateJson", json)
        let cmd = proc("Play.Update_CurrentTurnState", param)

        task {
            use cn = getConnection()
            let! _ = cn.ExecuteAsync(cmd)
            return ()
        }