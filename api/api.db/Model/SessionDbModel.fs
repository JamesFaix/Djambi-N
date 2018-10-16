module Djambi.Api.Db.Model.SessionDbModel

open System
open Djambi.Api.Model.SessionModel

[<CLIMutable>]
type SessionSqlModel =
    {
        sessionId : int
        userId : int
        token : string
        createdOn : DateTime
        expiresOn : DateTime
    }

module SessionSqlModel =
    
    let toModel (sqlModel : SessionSqlModel) : Session =
        {
            id = sqlModel.sessionId
            userId = sqlModel.userId
            token = sqlModel.token
            createdOn = sqlModel.createdOn
            expiresOn = sqlModel.expiresOn
        }
