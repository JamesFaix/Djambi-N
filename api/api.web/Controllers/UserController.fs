module Djambi.Api.Web.Controllers.UserController

open System.Threading.Tasks
open Giraffe
open Microsoft.AspNetCore.Http
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Web.HttpUtility
open Djambi.Api.Logic.Managers
open Djambi.Api.Model

let createUser : HttpHandler =
    let func (ctx : HttpContext) =
        getSessionOptionAndModelFromContext<CreateUserRequest> ctx
        |> thenBindAsync (fun (model, sessionOption) -> UserManager.createUser model sessionOption)
    handle func

let deleteUser(userId : int) =
    let func ctx =
        getSessionFromContext ctx
        |> thenBindAsync (UserManager.deleteUser userId)
        //TODO: Log out if non-admin deleting self
    handle func

let getUser(userId : int) =
    let func ctx =
        getSessionFromContext ctx
        |> thenBindAsync (UserManager.getUser userId)
    handle func

let getCurrentUser : HttpFunc -> HttpContext -> HttpContext option Task =
    let func ctx =
        getSessionFromContext ctx
        |> thenBindAsync UserManager.getCurrentUser
    handle func