module Djambi.Api.Db.SqlUtility

open System
open System.Data
open System.Data.SqlClient
open System.Text.RegularExpressions
open Dapper
open FSharp.Control.Tasks
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult

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
                        
let queryMany<'a>(command : CommandDefinition, entityType : string) : 'a list AsyncHttpResult =
    task {
        use connection = getConnection()

        try 
            return! SqlMapper.QueryAsync<'a>(connection, command)
                    |> Task.map (Seq.toList >> Ok)
        with
        | :? SqlException as ex when Regex.IsMatch(ex.Message, "Violation of.*constraint.*") -> 
            return Error <| HttpException(409, sprintf "Conflict when attempting to write %s." entityType)
    }

let querySingle<'a>(command : CommandDefinition, entityType : string) : 'a AsyncHttpResult =
    let singleOrError (xs : 'a list) =
        match xs.Length with
        | 1 -> Ok <| xs.[0]
        | 0 -> Error <| HttpException(404, sprintf "%s not found." entityType)
        | _ -> Error <| HttpException(500, sprintf "An unknown error occurred when manipulating %s." entityType)
        
    queryMany<'a>(command, entityType)
    |> thenBind singleOrError

let queryUnit(command : CommandDefinition, entityType : string) : Unit AsyncHttpResult =
    queryMany<Unit>(command, entityType) 
    |> thenMap ignore

type DynamicParameters with
    member this.add<'a>(name : string, value : 'a) : DynamicParameters =
        this.Add(name, value)
        this
        
    member this.addOption<'a> (name : string, opt : 'a option) : DynamicParameters =
        match opt with
        | Some x -> this.Add(name, x)
        | None -> this.Add(name, null)
        this