[<AutoOpen>]
module Djambi.Api.WebClient.Model

open System.Net
open System.Text.RegularExpressions
open System

type Response<'a> =
    {
        result : Result<'a, string>
        statusCode : HttpStatusCode
        headers : Map<string, string>            
    }

type Response<'a> with 
    member this.getToken() = 
        match this.headers.ContainsKey("Set-Cookie") with
        | false -> None
        | _ -> 
            let cookie = this.headers.["Set-Cookie"]
            let m = Regex.Match(cookie, "^DjambiSession=(.*?);");
            match m.Groups.Count with
            | 0 -> None
            | _ -> Some m.Groups.[1].Value

type Result<'a, 'b> with
    member this.IsOk() =
        match this with
        | Ok _ -> true
        | _ -> false

    member this.IsError() =
        match this with
        | Ok _ -> false
        | _ -> true

    member this.Value() =
        match this with
        | Ok x -> x
        | _ -> raise <| invalidOp "Cannot get value of error result."

    member this.ErrStr() =
        match this with
        | Error x -> x.ToString()
        | _ -> raise <| invalidOp "Cannot get error string of ok result."
        