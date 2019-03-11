module Djambi.Api.Db.SqlUtility

open System
open System.Data
open System.Data.SqlClient
open System.Linq
open System.Text.RegularExpressions
open Dapper
open FSharp.Control.Tasks
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult

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

type DynamicParameters with
    member this.add<'a>(name : string, value : 'a) : DynamicParameters =
        this.Add(name, value)
        this

    member this.addOption<'a> (name : string, opt : 'a option) : DynamicParameters =
        match opt with
        | Some x -> this.Add(name, x)
        | None -> this.Add(name, null)
        this

let executeTransactionally<'a> (commands : CommandDefinition seq) (resultEntityName : string): 'a AsyncHttpResult =
    task {
        use conn = getConnection()
        use tran = conn.BeginTransaction()

        try
            let commands = commands |> Seq.toList
            let mostCommands = commands |> Seq.take (commands.Length-1)

            for cmd in mostCommands do
                let cmd = cmd |> CommandDefinition.withTransaction tran
                let! _ = conn.ExecuteAsync cmd
                ()

            let lastCommand = commands |> Enumerable.Last
            let lastCommand = lastCommand |> CommandDefinition.withTransaction tran
            let! result = conn.QuerySingleAsync<'a> lastCommand

            tran.Commit()

            return Ok result
        
        with 
        | _ as ex -> return Error <| (catchSqlException ex resultEntityName)
    }