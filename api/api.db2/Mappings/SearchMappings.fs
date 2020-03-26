namespace Apex.Api.Db.Mappings

open Apex.Api.Db.Model
open Apex.Api.Model
open System

[<AutoOpen>]
module SearchMappings =

    let toSearchGame (source : GameSqlModel) : SearchGame =
        raise <| NotImplementedException()
