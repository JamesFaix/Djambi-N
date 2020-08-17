namespace Djambi.Api.Db.Mappings

open Djambi.Api.Db.Model
open Djambi.Api.Model

[<AutoOpen>]
module SessionMappings =

    let toSession (source : SessionSqlModel) : Session =
        {
            id = source.SessionId
            token = source.Token
            user = source.User |> toUser
            expiresOn = source.ExpiresOn
            createdOn = source.CreatedOn
        }