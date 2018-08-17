namespace Djambi.Api.Persistence

open System.Threading.Tasks

open Dapper
open Giraffe
open Newtonsoft.Json

open Djambi.Api.Domain.PlayModels

type PlayRepository(connectionString : string) =
    inherit RepositoryBase(connectionString)

    member this.startGame(request : UpdateGameForStartRequest) : Unit Task =
        let startingConditionsJson = request.startingConditions |> JsonConvert.SerializeObject
        let currentStateJson = request.currentState |> JsonConvert.SerializeObject

        let param = new DynamicParameters()
        param.Add("GameId", request.id)
        param.Add("StartingConditionsJson", startingConditionsJson)
        param.Add("CurrentStateJson", currentStateJson)
        let cmd = this.proc("Play.Update_GameForStart", param)

        task {
            use cn = this.getConnection()
            let! _ = cn.ExecuteAsync(cmd)
            return ()
        }
