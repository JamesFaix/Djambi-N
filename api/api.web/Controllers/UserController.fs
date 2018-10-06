namespace Djambi.Api.Web.Controllers

open System
open System.Threading.Tasks
open Giraffe
open Microsoft.AspNetCore.Http
open Djambi.Api.Common
open Djambi.Api.Db.Repositories
open Djambi.Api.Web.HttpUtility
open Djambi.Api.Web.Mappings.LobbyWebMapping
open Djambi.Api.Web.Model.LobbyWebModel

module UserController =

    let createUser : HttpHandler =
        let func (ctx : HttpContext) =            
            ctx.BindModelAsync<CreateUserJsonModel>()
            |> Task.map mapCreateUserRequest
            |> Task.bind UserRepository.createUser
            |> Task.map mapUserResponse
        handle func

    let deleteUser(userId : int) =
        let func ctx =
            UserRepository.deleteUser(userId)
        handle func

    let getUser(userId : int) =
        let func ctx =
            UserRepository.getUser userId
            |> Task.map mapUserResponse
        handle func

    let getUsers : HttpFunc -> HttpContext -> HttpContext option Task =
        let func ctx =
            UserRepository.getUsers()
            |> Task.map (Seq.map mapUserResponse)
        handle func

    let updateUser(userId : int) =
        let func ctx = 
            raise (NotImplementedException "")
        handle func
        