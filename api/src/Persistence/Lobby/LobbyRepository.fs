namespace Djambi.Api.Persistence

open System.Threading.Tasks

open Dapper
open Giraffe
    
open Djambi.Api.Persistence.LobbySqlModels
open Djambi.Api.Domain.LobbyModels
open Djambi.Api.Common.Enums
open Djambi.Api.Persistence.LobbySqlMappings
open Djambi.Api.Persistence.DapperExtensions
open Djambi.Api.Persistence.SqlUtility
    
module LobbyRepository =

//Users

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

//Games
    let createGame(request : CreateGameRequest) : LobbyGameMetadata Task =
        let param = new DynamicParameters()
        param.Add("BoardRegionCount", request.boardRegionCount)
        param.AddOptional("Description", request.description)
        let cmd = proc("Lobby.Insert_Game", param)

        task {
            use cn = getConnection()
            let! id = cn.QuerySingleAsync<int>(cmd)
            return {
                id = id 
                description = request.description
                status = GameStatus.Open
                boardRegionCount = request.boardRegionCount
                players = List.empty
            }
        }
        
    let deleteGame(id : int) : Unit Task =
        let param = new DynamicParameters()
        param.Add("GameId", id)
        let cmd = proc("Lobby.Delete_Game", param)

        task {
            use cn = getConnection()
            let! _ = cn.ExecuteAsync(cmd)
            return ()
        }

    let getGame(id : int) : LobbyGameMetadata Task =
        let param = new DynamicParameters()
        param.Add("GameId", id)
        let cmd = proc("Lobby.Get_GamesWithPlayers", param)

        task {
            use cn = getConnection()
            let! sqlModels = cn.QueryAsync<LobbyGamePlayerSqlModel>(cmd)
            return sqlModels |> Seq.toList |> mapLobbyGamesResponse |> List.head
        }
        
    let getGames() : LobbyGameMetadata list Task =
        let param = new DynamicParameters()
        param.Add("GameId", null)
        let cmd = proc("Lobby.Get_GamesWithPlayers", param)

        task {
            use cn = getConnection()
            let! sqlModels = cn.QueryAsync<LobbyGamePlayerSqlModel>(cmd)
            return sqlModels |> Seq.toList |> mapLobbyGamesResponse
        }

    let getOpenGames() : LobbyGameMetadata list Task =
        let param = new DynamicParameters()
        let cmd = proc("Lobby.Get_OpenGamesWithPlayers", param)

        task {
            use cn = getConnection()
            let! sqlModels = cn.QueryAsync<LobbyGamePlayerSqlModel>(cmd)
            return sqlModels |> Seq.toList |> mapLobbyGamesResponse
        }

//Players
    let addPlayerToGame(gameId : int, userId : int) : Unit Task =
        let param = new DynamicParameters()
        param.Add("GameId", gameId)
        param.Add("UserId", userId)
        let cmd = proc("Lobby.Insert_Player", param)

        task {
            use cn = getConnection()
            let! _ = cn.ExecuteAsync(cmd)
            return ()
        }

    let removePlayerFromGame(gameId : int, userId : int) : Unit Task =
        let param = new DynamicParameters()
        param.Add("GameId", gameId)
        param.Add("UserId", userId)
        let cmd = proc("Lobby.Delete_Player", param)

        task {
            use cn = getConnection()
            let! _ = cn.ExecuteAsync(cmd)
            return ()
        }