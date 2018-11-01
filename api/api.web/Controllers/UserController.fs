module Djambi.Api.Web.Controllers.UserController

open System.Threading.Tasks
open Giraffe
open Microsoft.AspNetCore.Http
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Logic.Services
open Djambi.Api.Web.HttpUtility
open Djambi.Api.Web.Mappings.UserWebMapping
open Djambi.Api.Web.Model.UserWebModel

let createUser : HttpHandler =
    let func (ctx : HttpContext) =
        getSessionOptionAndModelFromContext<CreateUserJsonModel> ctx
        |> thenMap (fun (model, sessionOption) -> (mapCreateUserRequest model, sessionOption))
        |> thenBindAsync (fun (model, sessionOption) -> UserService.createUser model sessionOption)
        |> thenMap mapUserResponse
    handle func

let deleteUser(userId : int) =
    let func ctx =
        getSessionFromContext ctx
        |> thenBindAsync (UserService.deleteUser userId)
        //TODO: Log out if non-admin deleting self
    handle func

let getUser(userId : int) =
    let func ctx =
        getSessionFromContext ctx
        |> thenBindAsync (UserService.getUser userId)
        |> thenMap mapUserResponse
    handle func

let getUsers : HttpFunc -> HttpContext -> HttpContext option Task =
    let func ctx =
        getSessionFromContext ctx
        |> thenBindAsync UserService.getUsers
        |> thenMap (Seq.map mapUserResponse)
    handle func