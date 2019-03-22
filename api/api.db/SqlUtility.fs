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

module SqlUtility2 =

    let private getEntityName (entityName : string option) =
        match entityName with Some x -> x | None -> "?"

    let private catchSqlException (ex : Exception) (resultEntityName : string option) : HttpException =
        match ex with
        | :? SqlException as ex when ex.Number >= 50400 && ex.Number <= 50599 ->
            HttpException(ex.Number % 50000, ex.Message)
        | :? SqlException as ex when Regex.IsMatch(ex.Message, "Violation of.*constraint.*") ->
            HttpException(409, sprintf "Conflict when attempting to write %s." (getEntityName(resultEntityName)))
        | _ ->
            HttpException(500, ex.Message)

    let private executeCommandsInTransaction 
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

    let queryMany<'a>
        (command : Command)
        (contextProvider : CommandContextProvider)
        : 'a list AsyncHttpResult =
        task {
            try
                use context = contextProvider.getContext()
                return! SqlMapper.QueryAsync<'a>(context.connection, command.toCommandDefinition())
                        |> Task.map (Seq.toList >> Ok)
            with
            | _ as ex -> return Error <| (catchSqlException ex command.entityType)
        }
  
    let querySingle<'a>
        (command : Command)
        (contextProvider : CommandContextProvider)
        : 'a AsyncHttpResult =
        let singleOrError (xs : 'a list) =
            match xs.Length with
            | 1 -> Ok <| xs.[0]
            | 0 -> Error <| HttpException(404, sprintf "%s not found." (getEntityName(command.entityType)))
            | _ -> Error <| HttpException(500, sprintf "An unknown error occurred when manipulating %s." (getEntityName(command.entityType)))

        queryMany<'a> command contextProvider
        |> thenBind singleOrError         

    let queryUnit
        (command : Command)
        (contextProvider : CommandContextProvider)
        : Unit AsyncHttpResult =
        queryMany<Unit> command contextProvider
        |> thenMap ignore

    let executeTransactionallyAndReturnLastResult<'a>
        (commands : Command seq)
        (contextProvider : CommandContextProvider)
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
            | _ as ex -> 
                let entityName = (commands |> Enumerable.Last).entityType
                return Error <| (catchSqlException ex entityName)
        }
        
    let executeTransactionally
        (commands : Command seq)
        (resultEntityName : string option)
        (contextProvider : CommandContextProvider)
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

    let executeTransactionallyButReturnFirstResult<'a>
        (command1 : Command, getCommand2 : 'a -> Command)
        (contextProvider : CommandContextProvider)
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
            | _ as ex ->            
                return Error <| (catchSqlException ex command1.entityType)
        }

type SqlUtility(contextProvider : CommandContextProvider) =
    
    member x.queryMany<'a> (command : Command) : 'a list AsyncHttpResult =
        SqlUtility2.queryMany command contextProvider
  
    member x.querySingle<'a> (command : Command) : 'a AsyncHttpResult =
        SqlUtility2.querySingle command contextProvider       

    member x.queryUnit (command : Command) : Unit AsyncHttpResult =
        SqlUtility2.queryUnit command contextProvider

    member x.executeTransactionallyAndReturnLastResult<'a> 
        (commands : Command seq)
        : 'a AsyncHttpResult =
        SqlUtility2.executeTransactionallyAndReturnLastResult commands contextProvider
        
    member x.executeTransactionally 
        (commands : Command seq)
        (resultEntityName : string option) 
        : Unit AsyncHttpResult =
        SqlUtility2.executeTransactionally commands resultEntityName contextProvider

    member x.executeTransactionallyButReturnFirstResult<'a> 
        (command1 : Command, getCommand2 : 'a -> Command)
        : 'a AsyncHttpResult =
        SqlUtility2.executeTransactionallyButReturnFirstResult (command1, getCommand2) contextProvider
