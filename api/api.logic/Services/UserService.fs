module Djambi.Api.Logic.Services.UserService

open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.Model

let createUser (request : CreateUserRequest) (session : Session option) : UserDetails AsyncHttpResult =
    match session with
    | Some s when not s.isAdmin -> errorTask <| HttpException(403, "Cannot create user if logged in.")
    | _ -> UserRepository.createUser request

let deleteUser (userId : int) (session : Session) : Unit AsyncHttpResult =
    if not session.isAdmin
        && userId <> session.userId
    then errorTask <| HttpException(403, "Cannot delete other users.")
    else UserRepository.deleteUser userId

let getUser (userId : int) (session : Session) : UserDetails AsyncHttpResult =
    if not (session.isAdmin
        || session.userId = userId)
    then errorTask <| HttpException(403, "Requires admin privileges.")
    else UserRepository.getUser userId

let getUsers (session : Session) : UserDetails list AsyncHttpResult =
    match session.isAdmin with
    | false -> errorTask <| HttpException(403, "Requires admin privileges.")
    | _ -> UserRepository.getUsers()