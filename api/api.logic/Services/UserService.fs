module Djambi.Api.Logic.Services.UserService

open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.Logic
open Djambi.Api.Model

let createUser (request : CreateUserRequest) (session : Session option) : UserDetails AsyncHttpResult =
    match session with
    | Some s when not (s.user.has EditUsers) -> errorTask <| HttpException(403, "Cannot create user if logged in.")
    | _ -> UserRepository.createUser request

let deleteUser (userId : int) (session : Session) : Unit AsyncHttpResult =
    Security.ensureSelfOrHas EditUsers session userId
    |> Result.bindAsync (fun _ -> UserRepository.deleteUser userId)

let getUser (userId : int) (session : Session) : UserDetails AsyncHttpResult =
    Security.ensureSelfOrHas EditUsers session userId
    |> Result.bindAsync (fun _ -> UserRepository.getUser userId)