namespace Djambi.Api.Web

open System
open System.IO
open System.Threading.Tasks
open Giraffe
open Microsoft.AspNetCore.Http
open Newtonsoft.Json
open Djambi.Api.Common
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Common.Json
open Djambi.Api.Logic.Services
open Djambi.Api.Model.SessionModel

type HttpHandler = HttpFunc -> HttpContext -> HttpContext option Task

module HttpUtility =

    let cookieName = "DjambiSession"

    let appendCookie (ctx : HttpContext) (token : string, expiration : DateTime) =
        let cookieOptions = new CookieOptions()
        cookieOptions.Domain <- "localhost" //TODO: (#166) Move this to a config file
        cookieOptions.Path <- "/"
        cookieOptions.Secure <- false
        cookieOptions.HttpOnly <- true
        cookieOptions.Expires <- DateTimeOffset(expiration) |> Nullable.ofValue
        ctx.Response.Cookies.Append(cookieName, token, cookieOptions);

    let appendEmptyCookie (ctx : HttpContext) =
        appendCookie ctx ("", DateTime.MinValue)

    let handle<'a> (func : HttpContext -> 'a AsyncHttpResult) : HttpHandler =

        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                try
                    let! result = func ctx
                    match result with
                    | Ok value ->
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
                        | Error ex -> 
                            appendEmptyCookie ctx
                            Error ex
            }

    let getSessionFromContext (ctx : HttpContext) : Session AsyncHttpResult =
        getSessionOptionFromContext ctx
        |> thenBind (fun opt ->
            match opt with
            | None -> 
                appendEmptyCookie ctx
                Error <| HttpException(401, "Not signed in.")
            | Some session -> Ok session
        )

    let private converters = 
        [|
            OptionJsonConverter() :> JsonConverter
            TupleArrayJsonConverter() :> JsonConverter
            UnionEnumJsonConverter() :> JsonConverter
            SingleFieldUnionJsonConverter() :> JsonConverter
        |]

    let private readJsonBody<'a> (ctx : HttpContext) : 'a AsyncHttpResult =
        try 
            use reader = new StreamReader(ctx.Request.Body)
            let json = reader.ReadToEnd()
            let value = JsonConvert.DeserializeObject<'a>(json, converters)
            okTask value
        with
        | ex -> errorTask <| HttpException(400, ex.Message)

    let private tupleWithModel<'a, 'b> (ctx : HttpContext) (result : 'b AsyncHttpResult): ('a * 'b) AsyncHttpResult =
        result
        |> thenBindAsync (fun value ->
            readJsonBody<'a> ctx
            |> thenBindAsync (fun body -> okTask (body, value))
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
        | Ok _ -> readJsonBody<'a> ctx