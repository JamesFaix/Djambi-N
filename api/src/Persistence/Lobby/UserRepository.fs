namespace Djambi.Api.Persistence

open System.Threading.Tasks

open Dapper
open Giraffe
    
open Djambi.Api.Persistence.LobbySqlModels
open Djambi.Api.Domain.LobbyModels
open Djambi.Api.Persistence.LobbySqlMappings
open Djambi.Api.Persistence.SqlUtility

module UserRepository =

    let getUser(id : int) : User Task =
        let param = new DynamicParameters()
        param.Add("UserId", id)
        let cmd = proc("Lobby.Get_Users", param)

        task {
            use cn = getConnection()
            let! sqlModel = cn.QuerySingleAsync<UserSqlModel>(cmd)
            if sqlModel.id = 0 then failwith "User not found" else ()
            return sqlModel |> mapUserResponse
        }

    let getUsers() : User seq Task =
        let param = new DynamicParameters()
        param.Add("UserId", null)
        let cmd = proc("Lobby.Get_Users", param)

        task {
            use cn = getConnection()
            let! sqlModels = cn.QueryAsync<UserSqlModel>(cmd)
            return sqlModels 
                    |> Seq.map mapUserResponse
                    |> Seq.sortBy (fun u -> u.id)
        }
        
    let createUser(request : CreateUserRequest) : User Task =
        let param = new DynamicParameters()
        param.Add("Name", request.name)
        param.Add("IsGuest", request.isGuest)
        param.Add("IsAdmin", false)
        let cmd = proc("Lobby.Insert_User", param)

        task {
            use cn = getConnection()
            let! id = cn.ExecuteScalarAsync<int>(cmd)
            return! getUser id
        }

    let deleteUser(id : int) : Unit Task =
        let param = new DynamicParameters()
        param.Add("UserId", id)
        let cmd = proc("Lobby.Delete_User", param)

        task {
            use cn = getConnection()
            let! _  = cn.ExecuteAsync(cmd) 
            return ()
        }

