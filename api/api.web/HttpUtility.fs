namespace Djambi.Api.Web

open System
open System.Threading.Tasks
open Giraffe
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Primitives
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Logic.Services
open Djambi.Api.Model.SessionModel

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

    let getSessionOptionFromContext (ctx : HttpContext) : Session option AsyncHttpResult =
        let token = ctx.Request.Cookies.Item(cookieName)

        if token |> String.IsNullOrEmpty
        then okTask <| None
        else
            task {
                let! result = SessionService.getSession token
                return match result with
                        | Ok session -> Ok <| Some(session)
                        | Error ex when ex.statusCode = 404 -> Ok(None)
                        | Error ex -> Error ex
            }

    let getSessionFromContext (ctx : HttpContext) : Session AsyncHttpResult =
        getSessionOptionFromContext ctx
        |> thenBind (fun opt ->
            match opt with
            | None -> Error <| HttpException(401, "Not signed in.")
            | Some session -> Ok session
        )

    let private tupleWithModel<'a, 'b> (ctx : HttpContext) (result : 'b AsyncHttpResult): ('a * 'b) AsyncHttpResult =
        result
        |> thenBindAsync (fun value ->
            ctx.BindModelAsync<'a>()
            |> Task.map (fun model -> Ok (model, value))
        )

    let getSessionAndModelFromContext<'a> (ctx : HttpContext) : ('a * Session) AsyncHttpResult =
        getSessionFromContext ctx
        |> tupleWithModel ctx

    let getSessionOptionAndModelFromContext<'a> (ctx : HttpContext) : ('a * Session option) AsyncHttpResult =
        getSessionOptionFromContext ctx
        |> tupleWithModel ctx

    let ensureNotSignedIn (ctx : HttpContext) : Result<Unit, HttpException> =
        let token = ctx.Request.Cookies.Item(cookieName)
        if token |> String.IsNullOrEmpty |> not
        then Error <| HttpException(401, "Operation not allowed if already signed in.")
        else Ok ()

    let ensureNotSignedInAndGetModel<'a> (ctx : HttpContext) : 'a AsyncHttpResult =
        match ensureNotSignedIn ctx with
        | Error ex -> errorTask ex
        | Ok _ -> ctx.BindModelAsync<'a>()
                  |> Task.map Ok