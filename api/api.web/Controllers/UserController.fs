namespace Djambi.Api.Web.Controllers

open Microsoft.AspNetCore.Http
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Logic.Managers
open Djambi.Api.Model
open Djambi.Api.Web.Interfaces
open Djambi.Api.Web

type UserController(u : HttpUtility) =
    interface IUserController with
        member x.createUser =
            let func (ctx : HttpContext) =
                u.getSessionOptionAndModelFromContext<CreateUserRequest> ctx
                |> thenBindAsync (fun (model, sessionOption) -> UserManager.createUser model sessionOption)
            u.handle func

        member x.deleteUser userId =
            let func ctx =
                u.getSessionFromContext ctx
                |> thenBindAsync (UserManager.deleteUser userId)
                //TODO: Log out if non-admin deleting self
            u.handle func

        member x.getUser userId =
            let func ctx =
                u.getSessionFromContext ctx
                |> thenBindAsync (UserManager.getUser userId)
            u.handle func

        member x.getCurrentUser =
            let func ctx =
                u.getSessionFromContext ctx
                |> thenBindAsync UserManager.getCurrentUser
            u.handle func