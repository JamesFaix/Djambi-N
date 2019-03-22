namespace Djambi.Api.Db

open System
open System.Data
open FSharp.Reflection
open Dapper
open Djambi.Api.Common
open Djambi.Api.Common.Control

/// <summary> 
/// Represents a SQL command with no return type. 
/// </summary>
type OpenCommand =
    {
        text : string
        parameters : (string * obj) list
        commandType : CommandType
        entityType : string option
    }

/// <summary> 
/// Represents a SQL command with a return type. 
/// </summary>
type ClosedCommand<'a> =
    {
        text : string        
        parameters : (string * obj) list
        commandType : CommandType
        entityType : string option
        execute : CommandContextProvider -> 'a AsyncHttpResult
    }

type OpenCommand with
    /// <summary> 
    /// Adds a parameter to the command. 
    /// </summary>
    member x.param (name : string, value : obj) : OpenCommand =
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
        
    /// <summary> 
    /// Adds an entity name to be used in error messages when executing the command. 
    /// For example, if <c>entityName</c> was 'User', a message might be 'User not found'.
    /// </summary>
    member x.forEntity (entityName : string) : OpenCommand =
        { x with entityType = Some entityName }

    /// <summary>
    /// Converts the command to a Dapper <c>CommandDefinition</c>.
    /// </summary>
    member x.toCommandDefinition (transaction : IDbTransaction option) : CommandDefinition =
        let dp = new DynamicParameters()
        for (name, value) in x.parameters do
            dp.Add(name, value)

        CommandDefinition(x.text,
                          dp,
                          transaction |> Option.toReference,
                          Nullable<int>(),
                          x.commandType |> Nullable.ofValue)

    /// <summary> 
    /// Converts the command to a <c>ClosedCommand</c>.
    /// </summary>
    member x.close<'a> (getResults : CommandContextProvider -> 'a AsyncHttpResult) : ClosedCommand<'a> =
        {
            text = x.text
            parameters = x.parameters
            commandType = x.commandType
            entityType = x.entityType
            execute = getResults
        }

type ClosedCommand<'a> with
    /// <summary>
    /// Converts the command back to an <c>OpenCommand</c>.
    /// </summary>
    member x.reopen() : OpenCommand =
        {
            text = x.text
            parameters = x.parameters
            commandType = x.commandType
            entityType = x.entityType
        }
        
    /// <summary>
    /// Converts the command to a Dapper <c>CommandDefinition</c>.
    /// </summary>
    member x.toCommandDefinition (transaction : IDbTransaction option) : CommandDefinition =
        x.reopen().toCommandDefinition(transaction)

/// <summary>
/// Encapsulates database access through Dapper.SqlMapper, 
/// as well as transactional operations.
/// </summary>
module CommandProcessor =
    open System.Data.SqlClient
    open System.Text.RegularExpressions
    open System.Threading.Tasks
    open FSharp.Control.Tasks
    open Djambi.Api.Common.Control.AsyncHttpResult

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
        (cmds : unit ClosedCommand seq) 
        (ctx : CommandContext)
        : Unit Task =
        task {
            for cmd in cmds do
                let cmd = cmd.toCommandDefinition ctx.transaction
                let! _ = ctx.connection.ExecuteAsync cmd
                ()
        }

    /// <summary>
    /// Executes the given command using the given context provider.
    /// Returns a sequence of results.
    /// </summary>
    let queryMany<'a>
        (command : OpenCommand)
        (contextProvider : CommandContextProvider)
        : 'a list AsyncHttpResult =
        task {
            try
                use context = contextProvider.getContext()
                return! SqlMapper.QueryAsync<'a>(context.connection, command.toCommandDefinition None)
                        |> Task.map (Seq.toList >> Ok)
            with
            | _ as ex -> return Error <| (catchSqlException ex command.entityType)
        }

    /// <summary>
    /// Executes the given command using the given context provider.
    /// Returns a single result, or an error if there are 0 or many results.
    /// </summary>
    let querySingle<'a>
        (command : OpenCommand)
        (contextProvider : CommandContextProvider)
        : 'a AsyncHttpResult =
        let singleOrError (xs : 'a list) =
            match xs.Length with
            | 1 -> Ok <| xs.[0]
            | 0 -> Error <| HttpException(404, sprintf "%s not found." (getEntityName(command.entityType)))
            | _ -> Error <| HttpException(500, sprintf "An unknown error occurred when manipulating %s." (getEntityName(command.entityType)))

        queryMany<'a> command contextProvider
        |> thenBind singleOrError         

    /// <summary>
    /// Executes the given command using the given context provider.
    /// Ignores any results and returns <c>()</c>.
    /// </summary>
    let queryUnit
        (command : OpenCommand)
        (contextProvider : CommandContextProvider)
        : Unit AsyncHttpResult =
        queryMany<Unit> command contextProvider
        |> thenMap ignore

    /// <summary>
    /// Executes <c>mostCommands</c> and then <c>lastCommand</c> using the given context provider.
    /// Returns result of <c>lastCommand</c>.
    /// </summary>
    let executeTransactionallyAndReturnLastResult<'a>
        (mostCommands : unit ClosedCommand seq)
        (lastCommand : 'a ClosedCommand)
        (contextProvider : CommandContextProvider)
        : 'a AsyncHttpResult =
        task {
            use ctx = contextProvider.getContextWithTransaction()
            try
                let! _ = executeCommandsInTransaction mostCommands ctx
                let lastCommand = lastCommand.toCommandDefinition ctx.transaction 
                let! lastResult = ctx.connection.QuerySingleAsync<'a> lastCommand
                ctx.commit()
                return Ok lastResult        
            with 
            | _ as ex -> 
                return Error <| (catchSqlException ex lastCommand.entityType)
        }
    /// <summary>
    /// Executes the given commands using the given context provider.
    /// </summary>
    let executeTransactionally
        (commands : unit ClosedCommand seq)
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
    
    /// <summary>
    /// Using the given context provider, executes <c>commands1</c>, 
    /// then generates another command using <c>getCommand2</c> and the result of <c>command1</c>,
    /// and executes that second command. 
    /// Returns the result of <c>command1</c>.
    /// </summary>
    let executeTransactionallyButReturnFirstResult<'a, 'b>
        (command1 : 'a ClosedCommand, getCommand2 : 'a -> 'b ClosedCommand)
        (contextProvider : CommandContextProvider)
        : 'a AsyncHttpResult =
        task {            
            use ctx = contextProvider.getContextWithTransaction()
            try 
                let cmd = command1.toCommandDefinition ctx.transaction 
                let! result = ctx.connection.QuerySingleAsync<'a> cmd
                let cmd = (getCommand2 result).toCommandDefinition ctx.transaction 
                let! _ = ctx.connection.ExecuteAsync cmd
                ctx.commit()
                return Ok result
            with 
            | _ as ex ->            
                return Error <| (catchSqlException ex command1.entityType)
        }

type OpenCommand with

    /// <summary>
    /// Converts the command into a <c>ClosedCommand</c> that returns <c>()</c>.
    /// </summary>
    member x.returnsNothing () : unit ClosedCommand =
        x.close (CommandProcessor.queryUnit x)

    /// <summary>
    /// Converts the command into a <c>ClosedCommand</c> that returns a single <c>'a</c> value.
    /// </summary>
    member x.returnsSingle<'a> () : 'a ClosedCommand =
        x.close (CommandProcessor.querySingle x)

    /// <summary>
    /// Converts the command into a <c>ClosedCommand</c> that returns a sequence of <c>'a</c> values.
    /// </summary>
    member x.returnsMany<'a> () : 'a list ClosedCommand =
        x.close (CommandProcessor.queryMany x)
        
module Command =
    /// <summary>
    /// Creates a command for the stored procedure with the given name, with no parameters or entityType.
    /// </summary>
    let proc (name : string) =
        {
            text = name
            parameters = []
            commandType = CommandType.StoredProcedure
            entityType = None
        }

    /// <summary>
    /// Executes the command using the given context provider.
    /// </summary>
    let execute (ctxProvider : CommandContextProvider) (cmd : 'a ClosedCommand) =
        cmd.execute ctxProvider

    /// <summary>
    /// Converts the command to a <c>ClosedCommand{Unit}</c>.
    /// </summary>
    let ignore (cmd : 'a ClosedCommand) : unit ClosedCommand =
        cmd.reopen().returnsNothing()