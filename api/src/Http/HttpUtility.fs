namespace Djambi.Api.Http

open System
open System.Threading.Tasks
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Primitives
open Giraffe

open Djambi.Api.Common
open Djambi.Api.Persistence
open Djambi.Api.Domain.LobbyModels

type HttpHandler = HttpFunc -> HttpContext -> HttpContext option Task 

module HttpUtility = 

    let handle<'a> (func : HttpContext -> 'a Task) : HttpHandler =

        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                try 
                    let! result = func ctx
                    ctx.Response.Headers.Add("Access-Control-Allow-Credentials", StringValues("true"))
                    return! json result next ctx
                with
                | :? HttpException as ex -> 
                    ctx.SetStatusCode ex.statusCode
                    return! json ex.Message next ctx
                | _ as ex -> 
                    ctx.SetStatusCode 500
                    return! json ex.Message next ctx
            }
            
    let cookieName = "DjambiSession"

    let getUserFromContext (ctx : HttpContext) : User Task =
        let cookie = ctx.Request.Cookies.Item(cookieName)

        if cookie |> String.IsNullOrEmpty
        then raise (HttpException(401, "Not currently logged in"))
        
        task {
            let! session = UserRepository.getSessionFromToken cookie
            return! UserRepository.getUser session.userId
        }