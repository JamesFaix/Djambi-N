namespace Djambi.Api.Db

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

type SqlUtility(connectionString) =

    let getConnection() =
        let cn = new SqlConnection(connectionString)
        cn.Open()
        cn :> IDbConnection

    let catchSqlException (ex : Exception) (entityType : string) : HttpException =
        match ex with
        | :? SqlException as ex when ex.Number >= 50400 && ex.Number <= 50599 ->
            HttpException(ex.Number % 50000, ex.Message)
        | :? SqlException as ex when Regex.IsMatch(ex.Message, "Violation of.*constraint.*") ->
            HttpException(409, sprintf "Conflict when attempting to write %s." entityType)
        | _ -> 
            HttpException(500, ex.Message)

    let executeCommandsInTransaction 
        (cmds : CommandDefinition seq) 
        (conn : IDbConnection) 
        (tran : IDbTransaction)
        : Unit Task =
        task {
            for cmd in cmds do
                let cmd = cmd.withTransaction tran
                let! _ = conn.ExecuteAsync cmd
                ()
        }

    member x.queryMany<'a>(command : CommandDefinition, entityType : string) : 'a list AsyncHttpResult =
        task {
            try
                use connection = getConnection()
                return! SqlMapper.QueryAsync<'a>(connection, command)
                        |> Task.map (Seq.toList >> Ok)
            with
            | _ as ex -> return Error <| (catchSqlException ex entityType)
        }
  
    member x.querySingle<'a>(command : CommandDefinition, entityType : string) : 'a AsyncHttpResult =
        let singleOrError (xs : 'a list) =
            match xs.Length with
            | 1 -> Ok <| xs.[0]
            | 0 -> Error <| HttpException(404, sprintf "%s not found." entityType)
            | _ -> Error <| HttpException(500, sprintf "An unknown error occurred when manipulating %s." entityType)

        x.queryMany<'a>(command, entityType)
        |> thenBind singleOrError

    member x.queryUnit(command : CommandDefinition, entityType : string) : Unit AsyncHttpResult =
        x.queryMany<Unit>(command, entityType)
        |> thenMap ignore
    

    member x.executeTransactionallyAndReturnLastResult<'a> 
        (commands : CommandDefinition seq) 
        (resultEntityName : string) 
        : 'a AsyncHttpResult =
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
    
    member x.executeTransactionally 
        (commands : CommandDefinition seq) 
        (resultEntityName : string) 
        : Unit AsyncHttpResult =
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


    member x.executeTransactionallyButReturnFirstResult<'a> 
        (command1 : CommandDefinition, getCommand2 : 'a -> CommandDefinition) 
        (resultEntityName : string) 
        : 'a AsyncHttpResult =
        task {
            use conn = getConnection()
            use tran = conn.BeginTransaction()
       
            try 
                let cmd = command1.withTransaction tran
                let! result = conn.QuerySingleAsync<'a> cmd
                let cmd = (getCommand2 result).withTransaction tran
                let! _ = conn.ExecuteAsync cmd
                tran.Commit()
                return Ok result
            with 
            | _ as ex -> return Error <| (catchSqlException ex resultEntityName)
        }
