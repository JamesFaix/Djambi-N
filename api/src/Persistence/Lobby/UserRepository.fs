namespace Djambi.Api.Persistence

open System
open System.Threading.Tasks

open Dapper
open Giraffe
    
open Djambi.Api.Persistence.LobbySqlModels
open Djambi.Api.Domain.LobbyModels
open Djambi.Api.Persistence.LobbySqlMappings
open Djambi.Api.Persistence.SqlUtility
open Djambi.Api.Common

module UserRepository =

    let private getUsersInner(id : int option, name : string option) : User seq Task =
        let param = new DynamicParameters()
        param.AddOption("UserId", id)
        param.AddOption("Name", name)        
        let cmd = proc("Lobby.Get_Users", param)

        task {
            use cn = getConnection()
            let! sqlModels = cn.QueryAsync<UserSqlModel>(cmd)
            return sqlModels 
                    |> Seq.map mapUserResponse
                    |> Seq.sortBy (fun u -> u.id)
        }

    let getUser(id : int) : User Task =
        getUsersInner(Some id, None)
        |> Task.map Seq.head
    
    let getUserByName(name : string) : User Task =
        getUsersInner(None, Some name)
        |> Task.map Seq.head

    let getUsers() : User seq Task =
        getUsersInner(None, None)
        
    let createUser(request : CreateUserRequest) : User Task =
        let param = new DynamicParameters()
        param.Add("Name", request.name)
        param.Add("RoleId", request.role |> mapRoleToId)
        param.Add("Password", request.password)
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

    let updateUserLoginData (id : int,
                             failedLoginAttempts : int, 
                             lastFailedLoginAttemptOn : DateTime option,
                             activeSessionToken : string option) = 
        let param = new DynamicParameters()
        param.Add("UserId", id)
        param.Add("FailedLoginAttempts", failedLoginAttempts)
        param.AddOption("LastFailedLoginAttemptOn", lastFailedLoginAttemptOn)
        param.AddOption("ActiveSessionToken", activeSessionToken)
        let cmd = proc("Lobby.Update_UserLoginData", param)

        task {
            use cn = getConnection()
            let! _ = cn.ExecuteAsync(cmd)
            return ()
        }