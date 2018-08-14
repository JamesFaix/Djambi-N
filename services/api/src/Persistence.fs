namespace Djambi.Api

module Persistence =

    open Dapper
    open Djambi.Model.Users
    open System.Data.SqlClient
    open System.Data
    open Djambi.Api.Dtos
    open Giraffe
    open System.Threading.Tasks

    type private idParams = { id : int }

    type Repository(connectionString : string) =
        member private this.getConnection() : IDbConnection =
            let cn = new SqlConnection(connectionString)
            cn.Open()
            cn :> IDbConnection

        member this.createUser(request : CreateUserRequest) : User Task =
            let query = "INSERT INTO Users (Name) VALUES (@Name) \
                         SELECT SCOPE_IDENTITY() AS Id, @Name AS Name"
            task {
                use cn = this.getConnection()
                let! dto = cn.QuerySingleAsync<UserDto>(query, request)
                return {
                    id = dto.id
                    name = dto.name
                }
            }

        member this.getUser(id : int) : User Task =
            let query = "SELECT UserId AS Id, Name AS Name
                         FROM Users
                         WHERE UserId = @Id"
            let param = { id = id }
            task {
                use cn = this.getConnection()
                let! dto = cn.QuerySingleAsync<UserDto>(query, param)
                return {
                    id = dto.id
                    name = dto.name
                }
            }

        member this.deleteUser(id : int) : Unit Task =
            let query = "DELETE FROM Users 
                         WHERE UserId = @Id"
            let param = { id = id }
            task {
                use cn = this.getConnection()
                let! _  = cn.ExecuteAsync(query, param) 
                return ()
            }
