namespace Apex.Api.Db.Mappings

open Apex.Api.Db.Model
open Apex.Api.Model
open System

[<AutoOpen>]
module EventMappings =

    let toEvent (source : EventSqlModel) : Event =
        raise <| NotImplementedException()

    let toEventSqlModel (source : Event) : EventSqlModel =
        raise <| NotImplementedException()
