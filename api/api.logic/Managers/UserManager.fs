namespace Apex.Api.Logic.Managers

open Apex.Api.Common.Control.AsyncHttpResult
open Apex.Api.Logic.Services
open Apex.Api.Model
open Apex.Api.Logic.Interfaces

type UserManager(userServ : UserService) =
    interface IUserManager with
        member x.createUser request sessionOption =
            userServ.createUser request sessionOption
            |> thenMap UserDetails.hideDetails

        member x.deleteUser userId session =
            userServ.deleteUser userId session

        member x.getUser userId session =
            userServ.getUser userId session
            |> thenMap UserDetails.hideDetails

        member x.getCurrentUser session =
            userServ.getUser session.user.id session
            |> thenMap UserDetails.hideDetails