module Djambi.Api.Model.SessionModel

open System
    
type Session =
    {
        id : int
        userId : int
        token : string
        createdOn : DateTime
        expiresOn : DateTime
        isAdmin : bool
    }

type LoginRequest =
    {
        username : string
        password : string
    }

type CreateSessionRequest = 
    {
        userId : int
        token : string
        expiresOn : DateTime
    }