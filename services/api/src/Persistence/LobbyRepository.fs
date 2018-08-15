namespace Djambi.Api.Persistence

open System.Threading.Tasks
    
open Dapper
open Giraffe
    
open Djambi.Api.Persistence.LobbySqlModels
open Djambi.Api.Domain.LobbyModels
open Djambi.Api.Common.Enums
open Djambi.Api.Persistence.LobbySqlMappings
    
type LobbyRepository(connectionString : string) =
    inherit RepositoryBase(connectionString)

//Users
    member this.createUser(request : CreateUserRequest) : User Task =
        let query = "INSERT INTO Users (Name) \
                     VALUES (@Name) \
                     SELECT SCOPE_IDENTITY()"
        task {
            use cn = this.getConnection()
            let! id = cn.ExecuteScalarAsync<int>(query, request)
            return {
                id = id
                name = request.name
            }
        }

    member this.getUser(id : int) : User Task =
        let query = "SELECT UserId AS Id, Name AS Name \
                     FROM Users \
                     WHERE UserId = @Id"
        let param = new DynamicParameters()
        param.Add("Id", id)
        task {
            use cn = this.getConnection()
            let! sqlModel = cn.QuerySingleAsync<UserSqlModel>(query, param)
            if sqlModel.id = 0 then failwith "User not found" else ()
            return sqlModel |> mapUserResponse
        }

    member this.deleteUser(id : int) : Unit Task =
        let query = "IF NOT EXISTS(SELECT 1 FROM Users WHERE UserId = @Id) \
                        THROW 50000, 'User not found', 1 \
        
                     DELETE FROM Users \
                     WHERE UserId = @Id"
        let param = new DynamicParameters()
        param.Add("Id", id)
        task {
            use cn = this.getConnection()
            let! _  = cn.ExecuteAsync(query, param) 
            return ()
        }

//Games
    member this.createGame(request : CreateGameRequest) : LobbyGameMetadata Task =
        let query = "INSERT INTO Games (BoardRegionCount, Description, GameStatusId) \
                     VALUES (@BoardRegionCount, @Description, @Status) \
                     SELECT SCOPE_IDENTITY()"

        let param = new DynamicParameters()
        param.Add("BoardRegionCount", request.boardRegionCount)
        param.Add("Description", if request.description.IsSome then request.description.Value else null)
        param.Add("Status", 1) //1 = Open

        task {
            use cn = this.getConnection()
            let! id = cn.QuerySingleAsync<int>(query, param)
            return {
                id = id 
                description = request.description
                status = GameStatus.Open
                boardRegionCount = request.boardRegionCount
                players = List.empty
            }
        }
        
    member this.deleteGame(id : int) : Unit Task =
        let query = "IF NOT EXISTS(SELECT 1 FROM Games WHERE GameId = @Id) \
                        THROW 50000, 'Game not found', 1 \

                     DELETE FROM Games \
                     WHERE GameId = @Id"
        let param = new DynamicParameters()
        param.Add("Id", id)
        task {
            use cn = this.getConnection()
            let! _ = cn.ExecuteAsync(query, param)
            return ()
        }

    member this.getGame(id : int) : LobbyGameMetadata Task =
        let query = "SELECT g.GameId, g.GameStatusId, g.Description AS GameDescription, g.BoardRegionCount,
                        u.UserId, u.Name as UserName \
                    FROM Games g \
                        LEFT OUTER JOIN Players p ON g.GameId = p.GameId \
                        LEFT OUTER JOIN Users u ON u.UserId = p.UserId \
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
                        u.UserId, u.Name as UserName \
                     FROM Games g \
                        LEFT OUTER JOIN Players p ON g.GameId = p.GameId \
                        LEFT OUTER JOIN Users u ON u.UserId = p.UserId \
                     WHERE g.GameStatusId = 1" //1 = Open
        task {
            use cn = this.getConnection()
            let! sqlModels = cn.QueryAsync<LobbyGamePlayerSqlModel>(query)
            return sqlModels |> Seq.toList |> mapLobbyGamesResponse
        }

    member this.addPlayerToGame(gameId : int, userId : int) : Unit Task =
        let query = "IF EXISTS(SELECT 1 FROM Players WHERE GameId = @GameId AND UserId = @UserId)
                        THROW 50000, 'Duplicate player', 1
                        
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

    member this.removePlayerFromGame(gameId : int, userId : int) : Unit Task =
        let query = "IF NOT EXISTS(SELECT 1 FROM Players WHERE GameId = @GameId AND UserId = @UserId)
                        THROW 50000, 'Player not found', 1
                        
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