namespace Djambi.Api.Persistence

module DapperExtensions =

    open Dapper

    type DynamicParameters with
        member this.AddOptional<'a when 'a : null>(name : string, opt : 'a option) : Unit =
            this.Add(name, if opt.IsNone then null else opt.Value)