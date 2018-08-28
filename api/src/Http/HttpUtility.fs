namespace Djambi.Api.Http

open Giraffe
open System.Threading.Tasks
open Microsoft.AspNetCore.Http
open Djambi.Api.Common
open Microsoft.Extensions.Primitives

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
