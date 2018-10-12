[<AutoOpen>]
module Djambi.Api.WebClient.Model

open System.Net
open System.Text.RegularExpressions

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