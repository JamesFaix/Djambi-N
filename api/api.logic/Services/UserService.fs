namespace Apex.Api.Logic.Services

open Apex.Api.Common.Control
open Apex.Api.Common.Control.AsyncHttpResult
open Apex.Api.Db.Interfaces
open Apex.Api.Logic
open Apex.Api.Model
open Apex.Api.Enums

type UserService(userRepo : IUserRepository) =
    member x.createUser (request : CreateUserRequest) (session : Session option) : UserDetails AsyncHttpResult =
        match session with
        | Some s when not (s.user.has Privilege.EditUsers) -> errorTask <| HttpException(403, "Cannot create user if logged in.")
        | _ -> 
            if not <| Validation.isValidUserName request.name
            then errorTask <| HttpException(422, "Usename must contain only letters A-Z, numbers 0-9, _ or -, and must be 1 to 20 characters.")
            elif not <| Validation.isValidPassord request.password
            then errorTask <| HttpException(422, "Password must contain only letters A-Z, numbers 0-9, _ or -, and mus tbe 6 to 20 characters.")
            else userRepo.createUser request

    member x.deleteUser (userId : int) (session : Session) : Unit AsyncHttpResult =
        Security.ensureSelfOrHas Privilege.EditUsers session userId
        |> Result.bindAsync (fun _ -> userRepo.deleteUser userId)

    member x.getUser (userId : int) (session : Session) : UserDetails AsyncHttpResult =
        Security.ensureSelfOrHas Privilege.EditUsers session userId
        |> Result.bindAsync (fun _ -> userRepo.getUser userId)