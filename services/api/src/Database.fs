namespace Djambi.Api

module Database =

    open Dapper
    open Djambi.Model.Users
    open System.Data.SqlClient
    open System.Configuration
    open System.Data
    open Djambi.Api.Dtos

    let private getConnection() : IDbConnection =
        let cnStr = ConfigurationManager.ConnectionStrings.["Main"].ToString()
        let cn = new SqlConnection(cnStr)
        cn.Open()
        cn :> IDbConnection

    type private idParams = { id : int }

    let createUser(request : CreateUserRequest) : User Async =
        let query = "INSERT INTO Users (Name) VALUES (@Name) \
                     SELECT SCOPE_IDENTITY() AS Id, @Name AS Name"
        async {
            use cn = getConnection()
            let! dto = cn.QuerySingleAsync<UserDto>(query, request) 
                      |> Async.AwaitTask
            return {
                id = dto.id
                name = dto.name
            }
        }

    let getUser(id : int) : User Async =
        let query = "SELECT UserId AS Id, Name AS Name
                     FROM Users
                     WHERE UserId = @Id"
        let param = { id = id }
        async {
            use cn = getConnection()
            let! dto = cn.QuerySingleAsync<UserDto>(query, param) 
                      |> Async.AwaitTask
            return {
                id = dto.id
                name = dto.name
            }
        }

    let deleteUser(id : int) : Unit Async =
        let query = "DELETE FROM Users 
                     WHERE UserId = @Id"
        let param = { id = id }
        async {
            use cn = getConnection()
            let! _  = cn.ExecuteAsync(query, param) 
                      |> Async.AwaitTask
            return ()
        }
