namespace Apex.Api.Web.Controllers

open Microsoft.AspNetCore.Http
open Apex.Api.Common.Control.AsyncHttpResult
open Apex.Api.Model
open Apex.Api.Web.Interfaces
open Apex.Api.Web
open Apex.Api.Logic.Interfaces

type UserController(u : HttpUtility,
                    userMan : IUserManager) =
    interface IUserController with
        member x.createUser =
            let func (ctx : HttpContext) =
                u.getSessionOptionAndModelFromContext<CreateUserRequest> ctx
                |> thenBindAsync (fun (model, sessionOption) -> userMan.createUser model sessionOption)
            u.handle func

        member x.deleteUser userId =
            let func ctx =
                u.getSessionFromContext ctx
                |> thenBindAsync (userMan.deleteUser userId)
                //TODO: Log out if non-admin deleting self
            u.handle func

        member x.getUser userId =
            let func ctx =
                u.getSessionFromContext ctx
                |> thenBindAsync (userMan.getUser userId)
            u.handle func

        member x.getCurrentUser =
            let func ctx =
                u.getSessionFromContext ctx
                |> thenBindAsync userMan.getCurrentUser
            u.handle func