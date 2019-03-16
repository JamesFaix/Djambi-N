namespace Djambi.Api.Web.Controllers

open Microsoft.AspNetCore.Http
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Web.HttpUtility
open Djambi.Api.Logic.Managers
open Djambi.Api.Model
open Djambi.Api.Web.Interfaces

type UserController() =
    interface IUserController with
        member x.createUser =
            let func (ctx : HttpContext) =
                getSessionOptionAndModelFromContext<CreateUserRequest> ctx
                |> thenBindAsync (fun (model, sessionOption) -> UserManager.createUser model sessionOption)
            handle func

        member x.deleteUser userId =
            let func ctx =
                getSessionFromContext ctx
                |> thenBindAsync (UserManager.deleteUser userId)
                //TODO: Log out if non-admin deleting self
            handle func

        member x.getUser userId =
            let func ctx =
                getSessionFromContext ctx
                |> thenBindAsync (UserManager.getUser userId)
            handle func

        member x.getCurrentUser =
            let func ctx =
                getSessionFromContext ctx
                |> thenBindAsync UserManager.getCurrentUser
            handle func