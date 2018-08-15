namespace Djambi.Api.Persistence

open System.Threading.Tasks
    
open Dapper
open Giraffe
    
open Djambi.Api.SqlModels
open Djambi.Model.Games
open Djambi.Model.Users
    
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
    member this.createGame(request : CreateGameRequest) : GameMetadata Task =
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