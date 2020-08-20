namespace Djambi.Api.Logic.Managers

open System
open System.ComponentModel.DataAnnotations
open FSharp.Control.Tasks
open Djambi.Api.Common.Control
open Djambi.Api.Db.Interfaces
open Djambi.Api.Enums
open Djambi.Api.Logic
open Djambi.Api.Logic.Interfaces
open Djambi.Api.Model

type UserManager(encyptionService: IEncryptionService,
                 userRepo : IUserRepository) =
    interface IUserManager with
        member __.createUser request sessionOption =
            match sessionOption with
            | Some s when not (s.user.has Privilege.EditUsers) -> 
                raise <| UnauthorizedAccessException("Cannot create user if logged in.")
            | _ ->                 
                task {
                    if request.password.Contains(request.name)
                    then raise <| ValidationException("Password cannot contain username.")
                    elif request.name.Contains(request.password)
                    then raise <| ValidationException("Username cannot contain password.")

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