namespace Djambi.Api.Persistence

open System.Threading.Tasks
    
open Dapper
open Giraffe
    
open Djambi.Api.Persistence.LobbySqlModels
open Djambi.Api.Domain.LobbyModels
open Djambi.Api.Common.Enums
    
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
            return {
                id = sqlModel.id
                name = sqlModel.name
            }
        }

    member this.deleteUser(id : int) : Unit Task =
        let query = "DELETE FROM Users \
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
        let query = "INSERT INTO Games (BoardRegionCount, Description, Status) \
                     VALUES (@BoardRegionCount, @Description, @Status) \
                     SELECT SCOPE_IDENTITY()"

        let param = new DynamicParameters()
        param.Add("BoardRegionCount", request.boardRegionCount)
        param.Add("Description", if request.description.IsSome then request.description.Value else null)
        param.Add("Status", 1) //1 = Open

        task {
            use cn = this.getConnection()
            let! id = cn.QuerySingleAsync<int>(query, request)
            return {
                id = id 
                description = request.description
                status = GameStatus.Open
                boardRegionCount = request.boardRegionCount
                players = List.empty
            }
        }
        
    member this.deleteGame(id : int) : Unit Task =
        let query = "DELETE FROM Games \
                     WHERE GameId = @Id"
        let param = new DynamicParameters()
        param.Add("Id", id)
        task {
            use cn = this.getConnection()
            let! _ = cn.ExecuteAsync(query, param)
            return ()
        }
        
    member this.getOpenGames() : LobbyGameMetadata list Task =
        let query = "SELECT g.GameId, g.Description AS GameDescription, g.BoardRegionCount,
                        u.UserId, u.Name as UserName \
                     FROM Games g \
                        INNER JOIN Players p ON g.GameId = p.GameId \
                        INNER JOIN Users u ON u.UserId = p.UserId \
                     WHERE g.Status = 1" //1 = Open

        let sqlModelToUser(sqlModel : OpenGamePlayerSqlModel) : User =
            {
                id = sqlModel.userId
                name = sqlModel.userName
            }

        let sqlModelsToGame(sqlModels : OpenGamePlayerSqlModel list) : LobbyGameMetadata =
            let head = sqlModels.Head
            {
                id = head.gameId
                status = GameStatus.Open
                boardRegionCount = head.boardRegionCount
                description = if head.gameDescription = null then None else Some head.gameDescription
                players = sqlModels |> List.map sqlModelToUser
            }

        task {
            use cn = this.getConnection()
            let! sqlModels = cn.QueryAsync<OpenGamePlayerSqlModel>(query)
            return sqlModels 
                |> Seq.groupBy (fun sql -> sql.gameId)
                |> Seq.map (fun (_, sqls) -> sqls |> Seq.toList |> sqlModelsToGame)
                |> Seq.toList
        }