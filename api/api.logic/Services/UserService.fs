module Djambi.Api.Logic.Services.UserService

open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.Model.SessionModel
open Djambi.Api.Model.UserModel

let createUser (request : CreateUserRequest) (session : Session option) : User AsyncHttpResult =
    match session with
    | Some s when not s.isAdmin -> errorTask <| HttpException(403, "Cannot create user if logged in.")
    | _ -> UserRepository.createUser request

let deleteUser (userId : int) (session : Session) : Unit AsyncHttpResult =
    if not session.isAdmin
        && userId <> session.userId
    then errorTask <| HttpException(403, "Cannot delete other users.")
    else UserRepository.deleteUser userId

let getUser (userId : int) (session : Session) : User AsyncHttpResult =
    if not (session.isAdmin
        || session.userId = userId)
    then errorTask <| HttpException(403, "Requires admin privileges.")
    else UserRepository.getUser userId

let getUsers (session : Session) : User list AsyncHttpResult =
    match session.isAdmin with
    | false -> errorTask <| HttpException(403, "Requires admin privileges.")
    | _ -> UserRepository.getUsers()