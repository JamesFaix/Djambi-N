namespace Djambi.Api.Logic.Managers

open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Logic.Services
open Djambi.Api.Model
open Djambi.Api.Logic.Interfaces

type UserManager() =
    interface IUserManager with
        member x.createUser request sessionOption =
            UserService.createUser request sessionOption
            |> thenMap UserDetails.hideDetails

        member x.deleteUser userId session =
            UserService.deleteUser userId session
    
        member x.getUser userId session =
            UserService.getUser userId session
            |> thenMap UserDetails.hideDetails
    
        member x.getCurrentUser session =
            UserService.getUser session.user.id session
            |> thenMap UserDetails.hideDetails