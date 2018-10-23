module Djambi.Api.Logic.Services.UserService

open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.Model.SessionModel
open Djambi.Api.Model.UserModel

let createUser (request : CreateUserRequest) : User AsyncHttpResult =
    UserRepository.createUser request

let deleteUser (userId : int) (session : Session) : Unit AsyncHttpResult =
    if not session.isAdmin
        && userId <> session.userId
    then errorTask <| HttpException(403, "Cannot delete other users.")
    else UserRepository.deleteUser userId

let getUser (userId : int) (session : Session) : User AsyncHttpResult =
    match session.isAdmin with
    | false -> errorTask <| HttpException(403, "Requires admin privileges.")
    | _ -> UserRepository.getUser userId
    
let getUsers (session : Session) : User list AsyncHttpResult =
    match session.isAdmin with
    | false -> errorTask <| HttpException(403, "Requires admin privileges.")
    | _ -> UserRepository.getUsers()