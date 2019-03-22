namespace Djambi.Api.Db

open System
open System.Data
open System.Threading.Tasks
open FSharp.Reflection
open Dapper
open Djambi.Api.Common

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
        execute : CommandContextProvider -> 'a Task
    }

module Command =
    let proc (name : string) =
        {
            text = name
            parameters = []
            commandType = CommandType.StoredProcedure
            entityType = None
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