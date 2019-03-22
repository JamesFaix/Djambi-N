module Djambi.Api.Db.SqlUtility

open System
open System.Data
open System.Data.SqlClient
open System.Linq
open System.Text.RegularExpressions
open System.Threading.Tasks
open Dapper
open FSharp.Control.Tasks
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.DapperExtensions

let mutable connectionString = null

let getConnection() =
    let cn = new SqlConnection(connectionString)
    cn.Open()
    cn :> IDbConnection

let catchSqlException<'a> (ex : Exception) (entityType : string) : HttpException =
    match ex with
    | :? SqlException as ex when ex.Number >= 50400 && ex.Number <= 50599 ->
        HttpException(ex.Number % 50000, ex.Message)
    | :? SqlException as ex when Regex.IsMatch(ex.Message, "Violation of.*constraint.*") ->
        HttpException(409, sprintf "Conflict when attempting to write %s." entityType)
    | _ -> 
        HttpException(500, ex.Message)

let queryMany<'a>(command : CommandDefinition, entityType : string) : 'a list AsyncHttpResult =
    task {
        try
            use connection = getConnection()
            return! SqlMapper.QueryAsync<'a>(connection, command)
                    |> Task.map (Seq.toList >> Ok)
        with
        | _ as ex -> return Error <| (catchSqlException ex entityType)
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
    
let private executeCommandsInTransaction (cmds : CommandDefinition seq) (conn : IDbConnection) (tran : IDbTransaction) : Unit Task =
    task {
        for cmd in cmds do
            let cmd = cmd.withTransaction tran
            let! _ = conn.ExecuteAsync cmd
            ()
    }

let executeTransactionallyAndReturnLastResult<'a> (commands : CommandDefinition seq) (resultEntityName : string) : 'a AsyncHttpResult =
    task {
        use conn = getConnection()
        use tran = conn.BeginTransaction()

        try
            let commands = commands |> Seq.toList
            let mostCommands = commands |> Seq.take (commands.Length-1)
            let! _ = executeCommandsInTransaction mostCommands conn tran
            let lastCommand = (commands |> Enumerable.Last).withTransaction tran
            let! lastResult = conn.QuerySingleAsync<'a> lastCommand
            tran.Commit()
            return Ok lastResult
        
        with 
        | _ as ex -> return Error <| (catchSqlException ex resultEntityName)
    }
    
let executeTransactionally (commands : CommandDefinition seq) (resultEntityName : string) : Unit AsyncHttpResult =
    task {
        use conn = getConnection()
        use tran = conn.BeginTransaction()

        try            
            let! _ = executeCommandsInTransaction commands conn tran
            tran.Commit()
            return Ok ()
        
        with 
        | _ as ex -> return Error <| (catchSqlException ex resultEntityName)
    }
