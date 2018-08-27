namespace Djambi.Api.Persistence

open System
open System.Data
open System.Data.SqlClient
open Dapper

module SqlUtility =
    
    let mutable connectionString = null

    let getConnection() =
        let cn = new SqlConnection(connectionString)
        cn.Open()
        cn :> IDbConnection

    let proc(name : string, param : obj) =
        new CommandDefinition(name, 
                              param, 
                              null, 
                              new Nullable<int>(), 
                              new Nullable<CommandType>(CommandType.StoredProcedure))

    type DynamicParameters with
        
        member this.AddOption<'a> (name : string, opt : 'a option) =
            match opt with
            | Some x -> this.Add(name, x)
            | None -> this.Add(name, null)