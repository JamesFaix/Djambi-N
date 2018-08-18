namespace Djambi.Api.Persistence

open System.Threading.Tasks

open Dapper
open Giraffe
open Newtonsoft.Json

open Djambi.Api.Domain.PlayModels

type PlayRepository(connectionString) =
    inherit RepositoryBase(connectionString)
    
    member this.getCurrentGameState(gameId : int) : GameState Task =
        let param = new DynamicParameters()
        param.Add("GameId", gameId)
        let cmd = this.proc("Play.Get_CurrentGameState", param)

        task {            
            use cn = this.getConnection()
            let! json = cn.QuerySingleAsync<string>(cmd)
            return JsonConvert.DeserializeObject<GameState>(json)
        }

    member this.updateCurrentGameState(gameId : int, state : GameState) : Unit Task =
        let json = state |> JsonConvert.SerializeObject
        let param = new DynamicParameters()
        param.Add("GameId", gameId)
        param.Add("CurrentGameStateJson", json)
        let cmd = this.proc("Play.Update_CurrentGameState", param)
          
        task {            
            use cn = this.getConnection()
            let! _ = cn.ExecuteAsync(cmd)
            return ()
        }

    member this.getCurrentTurnState(gameId : int) : TurnState Task =
        let param = new DynamicParameters()
        param.Add("GameId", gameId)
        let cmd = this.proc("Play.Get_CurrentTurnState", param)

        task {            
            use cn = this.getConnection()
            let! json = cn.QuerySingleAsync<string>(cmd)
            return JsonConvert.DeserializeObject<TurnState>(json)
        }

    member this.updateCurrentTurnState(gameId: int, state : TurnState) : Unit Task =
        let json = state |> JsonConvert.SerializeObject
        let param = new DynamicParameters()
        param.Add("GameId", gameId)
        param.Add("CurrentTurnStateJson", json)
        let cmd = this.proc("Play.Update_CurrentTurnState", param)

        task {
            use cn = this.getConnection()
            let! _ = cn.ExecuteAsync(cmd)
            return ()
        }