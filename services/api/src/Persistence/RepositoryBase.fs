namespace Djambi.Api.Persistence

open System.Data.SqlClient
open System.Data

type RepositoryBase(connectionString : string) = 
    member this.getConnection() : IDbConnection =
        let cn = new SqlConnection(connectionString)
        cn.Open()
        cn :> IDbConnection
