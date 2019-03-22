namespace Djambi.Api.Db

open System
open System.Data
open FSharp.Reflection
open Dapper
open Djambi.Api.Common
open Djambi.Api.Common.Control

type Command =
    {
        text : string
        parameters : (string * obj) list
        commandType : CommandType
        entityType : string option
    }

type ExecutableCommand<'a> =
    {
        text : string        
        parameters : (string * obj) list
        commandType : CommandType
        entityType : string option
        execute : CommandContextProvider -> 'a AsyncHttpResult
    }

type Command with
    member x.param (name : string, value : obj) =
        let value =
            if value = null then null
            else
                let t = value.GetType()

                let isOption = 
                    t.IsGenericType && 
                    t.GetGenericTypeDefinition() = typedefof<Option<_>>        

                if isOption then
                    let (case, fields) = FSharpValue.GetUnionFields(value, t)
                    if case.Name = "None" then null else fields.[0]
                else value

        { x with parameters = (name, value) :: x.parameters }
        
    member x.forEntity (entityType : string) =
        { x with entityType = Some entityType }

    member x.toCommandDefinition () =
        let dp = new DynamicParameters()
        for (name, value) in x.parameters do
            dp.Add(name, value)

        CommandDefinition(x.text,
                          dp,
                          null,
                          Nullable<int>(),
                          x.commandType |> Nullable.ofValue)

    member x.toExecutable<'a> getResults : ExecutableCommand<'a> =
        {
            text = x.text
            parameters = x.parameters
            commandType = x.commandType
            entityType = x.entityType
            execute = getResults
        }

type ExecutableCommand<'a> with
    member x.revert() : Command =
        {
            text = x.text
            parameters = x.parameters
            commandType = x.commandType
            entityType = x.entityType
        }

    member x.toCommandDefinition () =
        let dp = new DynamicParameters()
        for (name, value) in x.parameters do
            dp.Add(name, value)

        CommandDefinition(x.text,
                          dp,
                          null,
                          Nullable<int>(),
                          x.commandType |> Nullable.ofValue)
module SqlUtility =
    open System.Data.SqlClient
    open System.Text.RegularExpressions
    open System.Threading.Tasks
    open FSharp.Control.Tasks
    open Djambi.Api.Common.Control.AsyncHttpResult
    open Djambi.Api.Db.DapperExtensions

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
        (cmds : unit ExecutableCommand seq) 
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
        (mostCommands : unit ExecutableCommand seq)
        (lastCommand : 'a ExecutableCommand)
        (contextProvider : CommandContextProvider)
        : 'a AsyncHttpResult =
        task {
            use ctx = contextProvider.getContextWithTransaction()
            try
                let! _ = executeCommandsInTransaction mostCommands ctx
                let lastCommand = lastCommand.toCommandDefinition()
                                             .withTransaction(ctx.transaction.Value)
                let! lastResult = ctx.connection.QuerySingleAsync<'a> lastCommand
                ctx.commit()
                return Ok lastResult        
            with 
            | _ as ex -> 
                return Error <| (catchSqlException ex lastCommand.entityType)
        }
        
    let executeTransactionally
        (commands : unit ExecutableCommand seq)
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

    let executeTransactionallyButReturnFirstResult<'a, 'b>
        (command1 : 'a ExecutableCommand, getCommand2 : 'a -> 'b ExecutableCommand)
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

type Command with

    member x.returnsNothing () : unit ExecutableCommand =
        x.toExecutable (SqlUtility.queryUnit x)

    member x.returnsSingle<'a> () : 'a ExecutableCommand =
        x.toExecutable (SqlUtility.querySingle x)

    member x.returnsMany<'a> () : 'a list ExecutableCommand =
        x.toExecutable (SqlUtility.queryMany x)
        
module Command =
    let proc (name : string) =
        {
            text = name
            parameters = []
            commandType = CommandType.StoredProcedure
            entityType = None
        }

    let execute (ctxProvider : CommandContextProvider) (cmd : 'a ExecutableCommand) =
        cmd.execute ctxProvider

    let ignore (cmd : 'a ExecutableCommand) : unit ExecutableCommand =
        cmd.revert().returnsNothing()