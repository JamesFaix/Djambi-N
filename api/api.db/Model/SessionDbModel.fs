[<AutoOpen>]
module Djambi.Api.Db.Model.SessionDbModel

open System
open Djambi.Api.Model

[<CLIMutable>]
type SessionSqlModel =
    {
        sessionId : int
        userId : int
        token : string
        createdOn : DateTime
        expiresOn : DateTime
        isAdmin : bool
    }

let mapSessionResponse (sqlModel : SessionSqlModel) : Session =
    {
        id = sqlModel.sessionId
        userId = sqlModel.userId
        token = sqlModel.token
        createdOn = sqlModel.createdOn
        expiresOn = sqlModel.expiresOn
        isAdmin = sqlModel.isAdmin
    }
