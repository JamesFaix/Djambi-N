module Djambi.Api.Logic.Services.UserService

open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.Logic.Services
open Djambi.Api.Model

let createUser (request : CreateUserRequest) (session : Session option) : UserDetails AsyncHttpResult =
    match session with
    | Some s when not s.isAdmin -> errorTask <| HttpException(403, "Cannot create user if logged in.")
    | _ -> UserRepository.createUser request

let deleteUser (userId : int) (session : Session) : Unit AsyncHttpResult =
    SecurityService.ensureAdminOrSelf session userId
    |> Result.bindAsync (fun _ -> UserRepository.deleteUser userId)

let getUser (userId : int) (session : Session) : UserDetails AsyncHttpResult =
    SecurityService.ensureAdminOrSelf session userId
    |> Result.bindAsync (fun _ -> UserRepository.getUser userId)

let getUsers (session : Session) : UserDetails list AsyncHttpResult =
    SecurityService.ensureAdmin session
    |> Result.bindAsync UserRepository.getUsers