namespace Djambi.Api.Web

open System
open System.Threading.Tasks
open Giraffe
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Primitives
open Djambi.Api.Common
open Djambi.Api.Logic.Services
open Djambi.Api.Model.LobbyModel

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

    let getSessionFromContext (ctx : HttpContext) : Session Task =
        let token = ctx.Request.Cookies.Item(cookieName)

        if token |> String.IsNullOrEmpty
        then raise (HttpException(401, "Not currently logged in"))
        
        task {
            let! session = SessionService.getSession token

            if session.IsNone
            then raise <| HttpException(401, "Session expired")

            return session.Value
        }