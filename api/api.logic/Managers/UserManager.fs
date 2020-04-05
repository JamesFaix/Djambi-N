namespace Apex.Api.Logic.Managers

open Apex.Api.Model
open Apex.Api.Logic.Interfaces
open Apex.Api.Logic
open Apex.Api.Common.Control
open Apex.Api.Enums
open Apex.Api.Db.Interfaces
open FSharp.Control.Tasks

type UserManager(userRepo : IUserRepository) =
    interface IUserManager with
        member __.createUser request sessionOption =
            match sessionOption with
            | Some s when not (s.user.has Privilege.EditUsers) -> 
                raise <| HttpException(403, "Cannot create user if logged in.")
            | _ -> 
                if not <| Validation.isValidUserName request.name
                then raise <| HttpException(422, "Usename must contain only letters A-Z, numbers 0-9, _ or -, and must be 1 to 20 characters.")
                
                if not <| Validation.isValidPassord request.password
                then raise <| HttpException(422, "Password must contain only letters A-Z, numbers 0-9, _ or -, and mus tbe 6 to 20 characters.")
                
                task {
                    let! user = userRepo.createUser request
                    return user |> UserDetails.hideDetails
                }

        member __.deleteUser userId session =
            match Security.ensureSelfOrHas Privilege.EditUsers session userId with
            | Ok () -> userRepo.deleteUser userId
            | Error ex -> raise ex

        member __.getUser userId session =
            match Security.ensureSelfOrHas Privilege.EditUsers session userId with
            | Ok () -> 
                task {
                    let! user = userRepo.getUser userId
                    return user |> UserDetails.hideDetails
                }
            | Error ex -> raise ex

        member x.getCurrentUser session =
            (x :> IUserManager).getUser session.user.id session