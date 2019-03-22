namespace Djambi.Api.Db

open System
open System.Data
open FSharp.Reflection
open Dapper
open Djambi.Api.Common

type Command =
    {
        text : string
        parameters : (string * obj) list
        commandType : CommandType
        transaction : IDbTransaction option
    }

module Command =
    let proc (name : string) =
        {
            text = name
            parameters = []
            commandType = CommandType.StoredProcedure
            transaction = None
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

    member x.withTransaction (tran : IDbTransaction) =
        { x with transaction = Some tran }

    member x.toCommandDefintion () =
        let dp = new DynamicParameters()
        for (name, value) in x.parameters do
            dp.Add(name, value)

        CommandDefinition(x.text,
                          dp,
                          x.transaction |> Option.toReference,
                          Nullable<int>(),
                          x.commandType |> Nullable.ofValue)