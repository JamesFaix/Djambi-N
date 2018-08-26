namespace Djambi.Api.Persistence

module DapperExtensions =

    open Dapper

    type DynamicParameters with
        member this.AddOptional<'a>(name : string, opt : 'a option) : Unit =
            match opt with
            | None -> this.Add(name, null)
            | Some x -> this.Add(name, x)