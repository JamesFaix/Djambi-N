namespace Djambi.Api.Persistence

open System.Threading.Tasks

open Dapper
open Giraffe
open Newtonsoft.Json

open Djambi.Api.Domain.PlayModels

type PlayRepository(connectionString) =
    inherit RepositoryBase(connectionString)
    
    member this.getGameState(gameId : int) : GameState Task =
        let param = new DynamicParameters()
        param.Add("GameId", gameId)
        let cmd = this.proc("Play.Get_GameState", param)

        task {            
            use cn = this.getConnection()
            let! json = cn.QuerySingleAsync<string>(cmd)
            return JsonConvert.DeserializeObject<GameState>(json)
        }

    member this.updateGameState(gameId : int, state : GameState) : Unit Task =
        let json = state |> JsonConvert.SerializeObject
        let param = new DynamicParameters()
        param.Add("GameId", gameId)
        param.Add("CurrentStateJson", json)
        let cmd = this.proc("Play.Update_GameState", param)
          
        task {            
            use cn = this.getConnection()
            let! _ = cn.ExecuteAsync(cmd)
            return ()
        }
        