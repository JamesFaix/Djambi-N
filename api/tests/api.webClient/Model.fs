[<AutoOpen>]
module Djambi.Api.WebClient.Model

open System.Net
open System.Text.RegularExpressions
open System.Threading.Tasks
open Djambi.Api.Common

type Response<'a> =
    {
        body : Result<'a, string>
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

    member this.bodyValue : 'a =
        match this.body with
        | Ok x -> x
        | Error msg -> 
            invalidOp (sprintf "Cannot get value of error result. (%s)" msg)

    member this.bodyError : string =
        this.body |> Result.error

type AsyncResponse<'a> = 'a Response Task

module AsyncResponse =
    
    let bodyValue<'a> (response : 'a AsyncResponse) : 'a Task =
        response |> Task.map (fun resp -> resp.bodyValue)

    let bodyError<'a> (response : 'a AsyncResponse) : string Task =
        response |> Task.map (fun resp -> resp.bodyError)