namespace Djambi.Api.Logic.Services

open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Interfaces
open Djambi.Api.Logic
open Djambi.Api.Model

type UserService(userRepo : IUserRepository) =
    member x.createUser (request : CreateUserRequest) (session : Session option) : UserDetails AsyncHttpResult =
        match session with
        | Some s when not (s.user.has EditUsers) -> errorTask <| HttpException(403, "Cannot create user if logged in.")
        | _ -> userRepo.createUser request

    member x.deleteUser (userId : int) (session : Session) : Unit AsyncHttpResult =
        Security.ensureSelfOrHas EditUsers session userId
        |> Result.bindAsync (fun _ -> userRepo.deleteUser userId)

    member x.getUser (userId : int) (session : Session) : UserDetails AsyncHttpResult =
        Security.ensureSelfOrHas EditUsers session userId
        |> Result.bindAsync (fun _ -> userRepo.getUser userId)