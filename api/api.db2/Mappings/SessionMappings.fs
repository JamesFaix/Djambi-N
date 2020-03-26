namespace Apex.Api.Db.Mappings

open Apex.Api.Db.Model
open Apex.Api.Model

[<AutoOpen>]
module SessionMappings =

    let toSession (source : SessionSqlModel) : Session =
        {
            id = source.Id
            token = source.Token
            user = source.User |> toUser
            expiresOn = source.ExpiresOn
            createdOn = source.CreatedOn
        }