namespace Djambi.Api.Persistence

open System
open System.Data.SqlClient
open System.Data
open Dapper

type RepositoryBase(connectionString : string) = 
    member this.getConnection() : IDbConnection =
        let cn = new SqlConnection(connectionString)
        cn.Open()
        cn :> IDbConnection

    member this.procCommand(name : string, param : obj) =
        new CommandDefinition(name, 
                              param, 
                              null, 
                              new Nullable<int>(), 
                              new Nullable<CommandType>(CommandType.StoredProcedure))