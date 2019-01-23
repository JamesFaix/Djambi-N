module Djambi.Api.Db.CommandDefinition

open Dapper
open System.Data

let withTransaction (transaction : IDbTransaction) (command : CommandDefinition) : CommandDefinition =
    new CommandDefinition(
        command.CommandText, 
        command.Parameters, 
        transaction, 
        command.CommandTimeout, 
        command.CommandType, 
        command.Flags, 
        command.CancellationToken)
