namespace Djambi.Api.Db

open System
open System.Data.SqlClient
open System.Linq
open System.Text.RegularExpressions
open System.Threading.Tasks
open Dapper
open FSharp.Control.Tasks
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.DapperExtensions

type SqlUtility(contextProvider : CommandContextProvider) =

    let catchSqlException (ex : Exception) (entityType : string) : HttpException =
        match ex with
        | :? SqlException as ex when ex.Number >= 50400 && ex.Number <= 50599 ->
            HttpException(ex.Number % 50000, ex.Message)
        | :? SqlException as ex when Regex.IsMatch(ex.Message, "Violation of.*constraint.*") ->
            HttpException(409, sprintf "Conflict when attempting to write %s." entityType)
        | _ -> 
            HttpException(500, ex.Message)

    let executeCommandsInTransaction 
        (cmds : Command seq) 
        (ctx : CommandContext)
        : Unit Task =
        task {
            for cmd in cmds do
                let cmd = cmd.toCommandDefinition()
                             .withTransaction(ctx.transaction.Value)
                let! _ = ctx.connection.ExecuteAsync cmd
                ()
        }

    member x.queryMany<'a>(command : Command, entityType : string) : 'a list AsyncHttpResult =
        task {
            try
                use context = contextProvider.getContext()
                return! SqlMapper.QueryAsync<'a>(context.connection, command.toCommandDefinition())
                        |> Task.map (Seq.toList >> Ok)
            with
            | _ as ex -> return Error <| (catchSqlException ex entityType)
        }
  
    member x.querySingle<'a>(command : Command, entityType : string) : 'a AsyncHttpResult =
        let singleOrError (xs : 'a list) =
            match xs.Length with
            | 1 -> Ok <| xs.[0]
            | 0 -> Error <| HttpException(404, sprintf "%s not found." entityType)
            | _ -> Error <| HttpException(500, sprintf "An unknown error occurred when manipulating %s." entityType)

        x.queryMany<'a>(command, entityType)
        |> thenBind singleOrError         

    member x.queryUnit(command : Command, entityType : string) : Unit AsyncHttpResult =
        x.queryMany<Unit>(command, entityType)
        |> thenMap ignore

    member x.executeTransactionallyAndReturnLastResult<'a> 
        (commands : Command seq)
        (resultEntityName : string) 
        : 'a AsyncHttpResult =
        task {
            use ctx = contextProvider.getContextWithTransaction()
            try
                let commands = commands |> Seq.toList
                let mostCommands = commands |> Seq.take (commands.Length-1)
                let! _ = executeCommandsInTransaction mostCommands ctx
                let lastCommand = 
                    (commands |> Enumerable.Last)
                        .toCommandDefinition()
                        .withTransaction(ctx.transaction.Value)
                let! lastResult = ctx.connection.QuerySingleAsync<'a> lastCommand
                ctx.commit()
                return Ok lastResult        
            with 
            | _ as ex -> return Error <| (catchSqlException ex resultEntityName)
        }
        
    member x.executeTransactionally 
        (commands : Command seq)
        (resultEntityName : string) 
        : Unit AsyncHttpResult =
        task {            
            use ctx = contextProvider.getContextWithTransaction()
            try            
                let! _ = executeCommandsInTransaction commands ctx
                ctx.commit()
                return Ok ()        
            with 
            | _ as ex -> return Error <| (catchSqlException ex resultEntityName)
        }

    member x.executeTransactionallyButReturnFirstResult<'a> 
        (command1 : Command, getCommand2 : 'a -> Command) 
        (resultEntityName : string) 
        : 'a AsyncHttpResult =
        task {            
            use ctx = contextProvider.getContextWithTransaction()
            try 
                let cmd = command1.toCommandDefinition()
                                  .withTransaction(ctx.transaction.Value)
                let! result = ctx.connection.QuerySingleAsync<'a> cmd
                let cmd = (getCommand2 result).toCommandDefinition()
                                              .withTransaction(ctx.transaction.Value)
                let! _ = ctx.connection.ExecuteAsync cmd
                ctx.commit()
                return Ok result
            with 
            | _ as ex -> return Error <| (catchSqlException ex resultEntityName)
        }
