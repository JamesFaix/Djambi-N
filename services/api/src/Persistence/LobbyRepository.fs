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
    
type LobbyRepository(connectionString : string) =
    inherit RepositoryBase(connectionString)

//Users
    member this.createUser(request : CreateUserRequest) : User Task =
        let cmd = this.procCommand("Insert_User", request)

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
        let cmd = this.procCommand("Get_User", param)

        task {
            use cn = this.getConnection()
            let! sqlModel = cn.QuerySingleAsync<UserSqlModel>(cmd)
            if sqlModel.id = 0 then failwith "User not found" else ()
            return sqlModel |> mapUserResponse
        }

    member this.deleteUser(id : int) : Unit Task =
        let param = new DynamicParameters()
        param.Add("UserId", id)
        let cmd = this.procCommand("Delete_User", param)

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
        let cmd = this.procCommand("Insert_Game", param)

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
        let cmd = this.procCommand("Delete_Game", param)

        task {
            use cn = this.getConnection()
            let! _ = cn.ExecuteAsync(cmd)
            return ()
        }

    member this.getGame(id : int) : LobbyGameMetadata Task =
        let query = "SELECT g.GameId, g.GameStatusId, g.Description AS GameDescription, g.BoardRegionCount,
                        p.PlayerId, p.UserId, p.Name as PlayerName \
                    FROM Games g \
                        LEFT OUTER JOIN Players p ON g.GameId = p.GameId \
                    WHERE g.GameId = @Id"
        let param = new DynamicParameters()
        param.Add("Id", id)
        task {
            use cn = this.getConnection()
            let! sqlModels = cn.QueryAsync<LobbyGamePlayerSqlModel>(query, param)
            return sqlModels |> Seq.toList |> mapLobbyGamesResponse |> List.head
        }
        
    member this.getOpenGames() : LobbyGameMetadata list Task =
        let query = "SELECT g.GameId, g.GameStatusId, g.Description AS GameDescription, g.BoardRegionCount,
                        p.PlayerId, p.UserId, p.Name as PlayerName \
                     FROM Games g \
                        LEFT OUTER JOIN Players p ON g.GameId = p.GameId \
                     WHERE g.GameStatusId = 1" //1 = Open
        task {
            use cn = this.getConnection()
            let! sqlModels = cn.QueryAsync<LobbyGamePlayerSqlModel>(query)
            return sqlModels |> Seq.toList |> mapLobbyGamesResponse
        }

    member this.updateGame(request : UpdateGameRequest) : LobbyGameMetadata Task =
        let query = "IF NOT EXISTS(SELECT 1 FROM Games WHERE GameId = @Id)
                        THROW 50000, 'Game not found', 1

                     UPDATE Games
                     SET GameStatusId = @StatusId,
                         Description = @Description
                     WHERE GameId = @Id"
        let param = new DynamicParameters()
        param.Add("Id", request.id)
        param.AddOptional("Description", request.description)
        param.Add("StatusId", request.status |> mapGameStatusToId)
        task {
            use cn = this.getConnection()
            let! _ = cn.ExecuteAsync(query, param)
            let! updated = this.getGame(request.id)
            return updated
        }

//Players
    member this.addPlayerToGame(gameId : int, userId : int) : Unit Task =
        let query = "IF EXISTS(SELECT 1 FROM Players WHERE GameId = @GameId AND UserId = @UserId)
                        THROW 50000, 'Duplicate player', 1

                     IF (SELECT COUNT(1) FROM Players WHERE GameId = @GameId) 
                      = (SELECT BoardRegionCount FROM Games WHERE GameId = @GameId)
                        THROW 50000, 'Max player count reached', 1
                      
                     IF (SELECT GameStatusId FROM Games WHERE GameId = @GameId) <> 1
                        THROW 50000, 'Game no longer open', 1

                     INSERT INTO Players (GameId, UserId, Name)
                     SELECT @GameId, UserId, Name
                     FROM Users 
                     WHERE UserId = @UserId"

        let param = new DynamicParameters()
        param.Add("GameId", gameId)
        param.Add("UserId", userId)

        task {
            use cn = this.getConnection()
            let! _ = cn.ExecuteAsync(query, param)
            return ()
        }

    member this.addVirtualPlayerToGame(gameId : int, name : string) : Unit Task =
        let query = "IF EXISTS(SELECT 1 FROM Players WHERE GameId = @GameId AND Name = @Name)
                        THROW 50000, 'Duplicate player', 1

                     IF (SELECT COUNT(1) FROM Players WHERE GameId = @GameId) 
                      = (SELECT BoardRegionCount FROM Games WHERE GameId = @GameId)
                        THROW 50000, 'Max player count reached', 1
                      
                     IF (SELECT GameStatusId FROM Games WHERE GameId = @GameId) <> 1
                        THROW 50000, 'Game no longer open', 1

                     INSERT INTO Players (GameId, UserId, Name)
                     VALUES (@GameId, NULL, @Name)"

        let param = new DynamicParameters()
        param.Add("GameId", gameId)
        param.Add("Name", name)

        task {
            use cn = this.getConnection()
            let! _ = cn.ExecuteAsync(query, param)
            return ()
        }

    member this.removePlayerFromGame(gameId : int, userId : int) : Unit Task =
        let query = "IF NOT EXISTS(SELECT 1 FROM Players WHERE GameId = @GameId AND UserId = @UserId)
                        THROW 50000, 'Player not found', 1
                        
                     IF (SELECT GameStatusId FROM Games WHERE GameId = @GameId) <> 1
                        THROW 50000, 'Game no longer open', 1

                     DELETE FROM Players
                     WHERE GameId = @GameId AND UserId = @UserId"

        let param = new DynamicParameters()
        param.Add("GameId", gameId)
        param.Add("UserId", userId)

        task {
            use cn = this.getConnection()
            let! _ = cn.ExecuteAsync(query, param)
            return ()
        }

    member this.getVirtualPlayerNames() : string list Task =
        let query = "SELECT Name FROM VirtualPlayerNames"
        task {            
            use cn = this.getConnection()
            let! names = cn.QueryAsync<string>(query)
            return names |> Seq.toList
        }

    member this.updatePlayer(gameId : int, player : Player) : Unit Task =
        let query = "UPDATE Players 
                     SET Color = @Color,
                         IsAlive = @IsAlive
                     WHERE PlayerId = @PlayerId
                        AND GameId = @GameId"
        let param = new DynamicParameters()
        param.Add("GameId", gameId)
        param.Add("PlayerId", player.id)
        param.Add("Color", player.color |> mapPlayerColorToId)
        param.Add("IsAlive", player.isAlive)
        task {
            use cn = this.getConnection()
            let! _ = cn.ExecuteAsync(query, param)
            return ()
        }