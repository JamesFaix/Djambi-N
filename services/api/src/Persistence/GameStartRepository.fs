namespace Djambi.Api.Persistence

open System.Threading.Tasks

open Dapper
open Giraffe
open Newtonsoft.Json

open Djambi.Api.Domain.PlayModels
open Djambi.Api.Domain.LobbyModels


type GameStartRepository(connectionString, lobbyRepository : LobbyRepository) =
    inherit RepositoryBase(connectionString)

    member this.getGame(gameId : int) : LobbyGameMetadata Task =
        lobbyRepository.getGame gameId

    member this.getVirtualPlayerNames() : string list Task =
        let param = new DynamicParameters()
        let cmd = this.proc("Lobby.Get_VirtualPlayerNames", param)
        task {            
            use cn = this.getConnection()
            let! names = cn.QueryAsync<string>(cmd)
            return names |> Seq.toList
        }
        
    member this.addVirtualPlayerToGame(gameId : int, name : string) : Unit Task =
        let param = new DynamicParameters()
        param.Add("GameId", gameId)
        param.Add("Name", name)
        let cmd = this.proc("Lobby.Insert_VirtualPlayer", param)

        task {
            use cn = this.getConnection()
            let! _ = cn.ExecuteAsync(cmd)
            return ()
        }

    member this.startGame(request : UpdateGameForStartRequest) : Unit Task =
        let startingConditionsJson = request.startingConditions |> JsonConvert.SerializeObject
        let currentStateJson = request.currentState |> JsonConvert.SerializeObject

        let param = new DynamicParameters()
        param.Add("GameId", request.id)
        param.Add("StartingConditionsJson", startingConditionsJson)
        param.Add("CurrentGameStateJson", currentStateJson)
        let cmd = this.proc("Play.Update_GameForStart", param)

        task {
            use cn = this.getConnection()
            let! _ = cn.ExecuteAsync(cmd)
            return ()
        }
