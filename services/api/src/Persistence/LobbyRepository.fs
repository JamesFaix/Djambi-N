namespace Djambi.Api.Persistence

open System.Threading.Tasks

open Dapper
open Giraffe
    
open Djambi.Api.Persistence.LobbySqlModels
open Djambi.Api.Domain.LobbyModels
open Djambi.Api.Common.Enums
open Djambi.Api.Persistence.LobbySqlMappings
open Djambi.Api.Persistence.DapperExtensions
open Djambi.Api.Domain.PlayModels
open Giraffe
    
type LobbyRepository(connectionString : string) =
    inherit RepositoryBase(connectionString)

//Users
    member this.createUser(request : CreateUserRequest) : User Task =
        let param = new DynamicParameters()
        param.Add("Name", request.name)
        let cmd = this.proc("Lobby.Insert_User", param)

        task {
            use cn = this.getConnection()
            let! id = cn.ExecuteScalarAsync<int>(cmd)
            return {
                id = id
                name = request.name
            }
        }

    member this.getUser(id : int) : User Task =
        let param = new DynamicParameters()
        param.Add("UserId", id)
        let cmd = this.proc("Lobby.Get_User", param)

        task {
            use cn = this.getConnection()
            let! sqlModel = cn.QuerySingleAsync<UserSqlModel>(cmd)
            if sqlModel.id = 0 then failwith "User not found" else ()
            return sqlModel |> mapUserResponse
        }

    member this.deleteUser(id : int) : Unit Task =
        let param = new DynamicParameters()
        param.Add("UserId", id)
        let cmd = this.proc("Lobby.Delete_User", param)

        task {
            use cn = this.getConnection()
            let! _  = cn.ExecuteAsync(cmd) 
            return ()
        }

//Games
    member this.createGame(request : CreateGameRequest) : LobbyGameMetadata Task =
        let param = new DynamicParameters()
        param.Add("BoardRegionCount", request.boardRegionCount)
        param.AddOptional("Description", request.description)
        let cmd = this.proc("Lobby.Insert_Game", param)

        task {
            use cn = this.getConnection()
            let! id = cn.QuerySingleAsync<int>(cmd)
            return {
                id = id 
                description = request.description
                status = GameStatus.Open
                boardRegionCount = request.boardRegionCount
                players = List.empty
            }
        }
        
    member this.deleteGame(id : int) : Unit Task =
        let param = new DynamicParameters()
        param.Add("GameId", id)
        let cmd = this.proc("Lobby.Delete_Game", param)

        task {
            use cn = this.getConnection()
            let! _ = cn.ExecuteAsync(cmd)
            return ()
        }

    member this.getGame(id : int) : LobbyGameMetadata Task =
        let param = new DynamicParameters()
        param.Add("GameId", id)
        let cmd = this.proc("Lobby.Get_GameWithPlayers", param)

        task {
            use cn = this.getConnection()
            let! sqlModels = cn.QueryAsync<LobbyGamePlayerSqlModel>(cmd)
            return sqlModels |> Seq.toList |> mapLobbyGamesResponse |> List.head
        }
        
    member this.getOpenGames() : LobbyGameMetadata list Task =
        let param = new DynamicParameters()
        let cmd = this.proc("Lobby.Get_OpenGamesWithPlayers", param)

        task {
            use cn = this.getConnection()
            let! sqlModels = cn.QueryAsync<LobbyGamePlayerSqlModel>(cmd)
            return sqlModels |> Seq.toList |> mapLobbyGamesResponse
        }

    member this.updateGame(request : UpdateGameRequest) : LobbyGameMetadata Task =
        let param = new DynamicParameters()
        param.Add("GameId", request.id)
        param.AddOptional("Description", request.description)
        param.Add("StatusId", request.status |> mapGameStatusToId)
        let cmd = this.proc("Lobby.Update_Game", param)

        task {
            use cn = this.getConnection()
            let! _ = cn.ExecuteAsync(cmd)
            return! this.getGame(request.id)
        }

//Players
    member this.addPlayerToGame(gameId : int, userId : int) : Unit Task =
        let param = new DynamicParameters()
        param.Add("GameId", gameId)
        param.Add("UserId", userId)
        let cmd = this.proc("Lobby.InsertPlayer", param)

        task {
            use cn = this.getConnection()
            let! _ = cn.ExecuteAsync(cmd)
            return ()
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

    member this.removePlayerFromGame(gameId : int, userId : int) : Unit Task =
        let param = new DynamicParameters()
        param.Add("GameId", gameId)
        param.Add("UserId", userId)
        let cmd = this.proc("Lobby.Delete_Player", param)

        task {
            use cn = this.getConnection()
            let! _ = cn.ExecuteAsync(cmd)
            return ()
        }

    member this.getVirtualPlayerNames() : string list Task =
        let param = new DynamicParameters()
        let cmd = this.proc("Lobby.Get_VirtualPlayerNames", param)
        task {            
            use cn = this.getConnection()
            let! names = cn.QueryAsync<string>(cmd)
            return names |> Seq.toList
        }
