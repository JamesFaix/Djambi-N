namespace Djambi.Api.Logic.Services

open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.Logic
open Djambi.Api.Model

type UserService() =
    member x.createUser (request : CreateUserRequest) (session : Session option) : UserDetails AsyncHttpResult =
        match session with
        | Some s when not (s.user.has EditUsers) -> errorTask <| HttpException(403, "Cannot create user if logged in.")
        | _ -> UserRepository.createUser request

    member x deleteUser (userId : int) (session : Session) : Unit AsyncHttpResult =
        Security.ensureSelfOrHas EditUsers session userId
        |> Result.bindAsync (fun _ -> UserRepository.deleteUser userId)

    member x.getUser (userId : int) (session : Session) : UserDetails AsyncHttpResult =
        Security.ensureSelfOrHas EditUsers session userId
        |> Result.bindAsync (fun _ -> UserRepository.getUser userId)