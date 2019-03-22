module Djambi.Api.Db.DapperExtensions

open System
open System.Data
open Dapper

let proc(name : string, param : obj) =
    new CommandDefinition(name,
                          param,
                          null,
                          Nullable<int>(),
                          Nullable<CommandType>(CommandType.StoredProcedure))

type CommandDefinition with    
    member x.withTransaction (transaction : IDbTransaction) : CommandDefinition =
        new CommandDefinition(
            x.CommandText, 
            x.Parameters, 
            transaction, 
            x.CommandTimeout, 
            x.CommandType, 
            x.Flags, 
            x.CancellationToken)

type DynamicParameters with
    member this.add<'a>(name : string, value : 'a) : DynamicParameters =
        this.Add(name, value)
        this

    member this.addOption<'a> (name : string, opt : 'a option) : DynamicParameters =
        match opt with
        | Some x -> this.Add(name, x)
        | None -> this.Add(name, null)
        this