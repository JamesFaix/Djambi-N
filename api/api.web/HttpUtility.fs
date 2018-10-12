namespace Djambi.Api.Web

open System
open System.Threading.Tasks
open Giraffe
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Primitives
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Logic.Services
open Djambi.Api.Model.LobbyModel

type HttpHandler = HttpFunc -> HttpContext -> HttpContext option Task 

module HttpUtility = 

    let handle<'a> (func : HttpContext -> 'a AsyncHttpResult) : HttpHandler =

        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                try 
                    let! result = func ctx
                    match result with
                    | Ok value -> 
                        ctx.Response.Headers.Add("Access-Control-Allow-Credentials", StringValues("true"))
                        return! json value next ctx
                    | Error ex -> 
                        ctx.SetStatusCode ex.statusCode
                        return! json ex.Message next ctx                    
                with
                | :? HttpException as ex -> 
                    ctx.SetStatusCode ex.statusCode
                    return! json ex.Message next ctx
                | _ as ex -> 
                    ctx.SetStatusCode 500
                    return! json ex.Message next ctx
            }
            
    let cookieName = "DjambiSession"

    let getSessionFromContext (ctx : HttpContext) : Session AsyncHttpResult =
        let token = ctx.Request.Cookies.Item(cookieName)

        if token |> String.IsNullOrEmpty
        then errorTask <| HttpException(401, "Not currently logged in")
        else 
            SessionService.getSession token
            |> thenReplaceError 404 (HttpException(401, "Session expired"))
