namespace Apex.Api.Logic.Managers

open Apex.Api.Model
open Apex.Api.Logic.Interfaces
open Apex.Api.Logic
open Apex.Api.Enums
open Apex.Api.Db.Interfaces
open FSharp.Control.Tasks
open System
open System.ComponentModel.DataAnnotations
open Apex.Api.Common.Control

type UserManager(encyptionService: IEncryptionService,
                 userRepo : IUserRepository) =
    interface IUserManager with
        member __.createUser request sessionOption =
            match sessionOption with
            | Some s when not (s.user.has Privilege.EditUsers) -> 
                raise <| UnauthorizedAccessException("Cannot create user if logged in.")
            | _ -> 
                // TODO: Data annotation should make this obsolete
                if not <| Validation.isValidUserName request.name
                then raise <| ValidationException("Usename must contain only letters A-Z, numbers 0-9, _ or -, and must be 1 to 20 characters.")
                
                if not <| Validation.isValidPassord request.password
                then raise <| ValidationException("Password must contain only letters A-Z, numbers 0-9, _ or -, and mus tbe 6 to 20 characters.")
                
                task {
                    let hash = encyptionService.hash request.password
                    let request = { request with password = hash }
                    let! user = userRepo.createUser request
                    return user |> UserDetails.hideDetails
                }

        member __.deleteUser userId session =
            Security.ensureSelfOrHas Privilege.EditUsers session userId
            userRepo.deleteUser userId

        member __.getUser userId session =
            Security.ensureSelfOrHas Privilege.EditUsers session userId
            task {
                match! userRepo.getUser userId with
                | None -> return raise <| NotFoundException("User not found.")
                | Some user -> return user |> UserDetails.hideDetails
            }
            
        member x.getCurrentUser session =
            (x :> IUserManager).getUser session.user.id session