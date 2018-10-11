module Djambi.Api.Db.SqlUtility

open System
open System.Data
open System.Data.SqlClient
open System.Text.RegularExpressions
open System.Threading.Tasks
open Dapper
open FSharp.Control.Tasks
open Djambi.Api.Common

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
                        
let queryMany<'a>(command : CommandDefinition, entityType : string) : 'a list Task =
    task {
        let connection = getConnection()

        try 
            let! items = SqlMapper.QueryAsync<'a>(connection, command)
            return items |> Seq.toList
        with
        | :? SqlException as ex when Regex.IsMatch(ex.Message, "Violation of.*constraint.*") -> 
            raise <| HttpException(409, sprintf "Conflict when attempting to write %s." entityType)
            return List.empty
    }

let querySingle<'a>(command : CommandDefinition, entityType : string) : 'a Task =
    task {
        let! results = queryMany<'a>(command, entityType)

        match results.Length with
        | 1 -> return results.[0]
        | 0 -> raise <| HttpException(404, sprintf "%s not found." entityType)
               return Unchecked.defaultof<'a>
        | _ -> raise <| HttpException(500, sprintf "An unknown error occurred when manipulating %s." entityType)
               return Unchecked.defaultof<'a>
    }

let queryUnit(command : CommandDefinition, entityType : string) : Unit Task =
    task {
        let! _ = queryMany<Unit>(command, entityType)
        return ()
    }