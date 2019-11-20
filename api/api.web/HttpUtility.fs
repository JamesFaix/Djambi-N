namespace Apex.Api.Web

open System
open System.IO
open System.Threading.Tasks
open FSharp.Control.Tasks
open Giraffe
open Microsoft.AspNetCore.Http
open Newtonsoft.Json
open Serilog
open Apex.Api.Common
open Apex.Api.Common.Control
open Apex.Api.Common.Control.AsyncHttpResult
open Apex.Api.Common.Json
open Apex.Api.Logic.Interfaces
open Apex.Api.Model.SessionModel

type HttpHandler = HttpFunc -> HttpContext -> HttpContext option Task

type HttpUtility(cookieDomain : string,
                 sessionServ : ISessionService,
                 log : ILogger) =

    let converters =
        [|
            OptionJsonConverter() :> JsonConverter
            TupleArrayJsonConverter() :> JsonConverter
            UnionEnumJsonConverter() :> JsonConverter
            SingleFieldUnionJsonConverter() :> JsonConverter
        |]

    let readJsonBody (ctx : HttpContext) : 'a AsyncHttpResult =
        try
            task {
                use reader = new StreamReader(ctx.Request.Body)
                let! json = reader.ReadToEndAsync()
                let value = JsonConvert.DeserializeObject<'a>(json, converters)
                return Ok value            
            }
        with
        | ex -> errorTask <| HttpException(400, ex.Message)

    let tupleWithModel (ctx : HttpContext) (result : 'b AsyncHttpResult): ('a * 'b) AsyncHttpResult =
        result
        |> thenBindAsync (fun value ->
            readJsonBody ctx
            |> thenBindAsync (fun body -> okTask (body, value))
        )

    member x.cookieName = "DjambiSession"

    member x.appendCookie (ctx : HttpContext) (token : string, expiration : DateTime) =
        let cookieOptions = CookieOptions()
        cookieOptions.Domain <- cookieDomain
        cookieOptions.Path <- "/"
        cookieOptions.Secure <- false
        cookieOptions.HttpOnly <- true
        cookieOptions.Expires <- DateTimeOffset(expiration) |> Nullable.ofValue
        ctx.Response.Cookies.Append(x.cookieName, token, cookieOptions);

    member x.appendEmptyCookie (ctx : HttpContext) =
        x.appendCookie ctx ("", DateTime.MinValue)

    member x.handle<'a> (func : HttpContext -> 'a AsyncHttpResult) : HttpHandler =

        fun (next : HttpFunc) (ctx : HttpContext) ->
            let route = sprintf "%s %s" ctx.Request.Method (ctx.Request.Path.ToString())
            log.Information(sprintf "API Req: %s" route)
            task {
                try
                    let! result = func ctx
                    match result with
                    | Ok value ->
                        match ctx.Response.ContentType with
                        | "text/event-stream" ->
                            return! next ctx
                        | _ ->
                            return! json value next ctx
                    | Error ex ->
                        ctx.SetStatusCode ex.statusCode
                        return! json ex.Message next ctx
                with
                | :? HttpException as ex ->
                    ctx.SetStatusCode ex.statusCode
                    return! json ex.Message next ctx
                | ex ->
                    ctx.SetStatusCode 500
                    return! json ex.Message next ctx
            }
            |> Task.map(fun x -> 
                match x with 
                | Some c ->
                    log.Information(sprintf "API Rsp: %s %i" route c.Response.StatusCode)
                | _ -> ()
                x
            )

    member x.getSessionOptionFromContext (ctx : HttpContext) : Session option AsyncHttpResult =
        let token = ctx.Request.Cookies.Item(x.cookieName)

        if token |> String.IsNullOrEmpty
        then okTask <| None
        else
            task {
                let! result = sessionServ.getSession token
                return match result with
                        | Ok session -> Ok <| Some(session)
                        | Error ex when ex.statusCode = 404 -> Ok(None)
                        | Error ex ->
                            x.appendEmptyCookie ctx
                            Error ex
            }

    member x.getSessionFromContext (ctx : HttpContext) : Session AsyncHttpResult =
        x.getSessionOptionFromContext ctx
        |> thenBind (fun opt ->
            match opt with
            | None ->
                x.appendEmptyCookie ctx
                Error <| HttpException(401, "Not signed in.")
            | Some session -> Ok session
        )

    member x.getSessionAndModelFromContext<'a> (ctx : HttpContext) : ('a * Session) AsyncHttpResult =
        x.getSessionFromContext ctx
        |> tupleWithModel ctx

    member x.getSessionOptionAndModelFromContext<'a> (ctx : HttpContext) : ('a * Session option) AsyncHttpResult =
        x.getSessionOptionFromContext ctx
        |> tupleWithModel ctx

    member x.ensureNotSignedIn (ctx : HttpContext) : Result<Unit, HttpException> =
        let token = ctx.Request.Cookies.Item(x.cookieName)
        if token |> String.IsNullOrEmpty |> not
        then Error <| HttpException(401, "Operation not allowed if already signed in.")
        else Ok ()

    member x.ensureNotSignedInAndGetModel<'a> (ctx : HttpContext) : 'a AsyncHttpResult =
        match x.ensureNotSignedIn ctx with
        | Error ex -> errorTask ex
        | Ok _ -> readJsonBody ctx