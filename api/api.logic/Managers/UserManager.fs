namespace Apex.Api.Logic.Managers

open Apex.Api.Model
open Apex.Api.Logic.Interfaces
open Apex.Api.Logic
open Apex.Api.Enums
open Apex.Api.Db.Interfaces
open FSharp.Control.Tasks
open System
open Apex.Api.Common.Control

type UserManager(encyptionService: IEncryptionService,
                 userRepo : IUserRepository) =
    interface IUserManager with
        member __.createUser request sessionOption =
            match sessionOption with
            | Some s when not (s.user.has Privilege.EditUsers) -> 
                raise <| UnauthorizedAccessException("Cannot create user if logged in.")
            | _ ->                 
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