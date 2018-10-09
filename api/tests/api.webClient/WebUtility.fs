module Djambi.Api.WebClient.WebUtility

open System.IO
open System.Linq
open System.Net
open System.Threading.Tasks
open FSharp.Control.Tasks
open Microsoft.Extensions.Configuration
open Newtonsoft.Json
open Djambi.Api.WebClient.Model
open Djambi.Utilities

let private config = 
    ConfigurationBuilder()
        .AddJsonFile(Environment.environmentConfigPath(6), false)
        .Build()
        
let apiAddress = config.["apiAddress"]

let DELETE = "DELETE";
let GET = "GET";
let PATCH = "PATCH";
let POST = "POST";

let sendRequest<'a, 'b> (httpVerb : string,
                         route : string,                           
                         body : 'a option,
                         token : string option) 
                         : 'b Response Task =
    
    let request = WebRequest.Create(apiAddress + "/api" + route) :?> HttpWebRequest
    request.ContentType <- "application/json"
    request.Method <- httpVerb

    if token.IsSome
    then
        request.Headers.Add("Cookie", "DjambiSession=" + token.Value)

    if body.IsSome
    then
        let bodyText = JsonConvert.SerializeObject(body.Value)
        use writer = new StreamWriter(request.GetRequestStream())
        writer.Write(bodyText)
        writer.Flush()
        writer.Close()

    task {
        let! response = request.GetResponseAsync() 
        let webResponse = response :?> HttpWebResponse
        use responseStream = webResponse.GetResponseStream()
        use reader = new StreamReader(responseStream)
        let responseText = reader.ReadToEnd()

        let result = 
            match webResponse.StatusCode with
            | n when n < HttpStatusCode.BadRequest ->  
                let value = JsonConvert.DeserializeObject<'b>(responseText)
                Ok(value)
            | _ -> Error(responseText)

        let headers = 
            webResponse.Headers.Keys
            |> Enumerable.OfType<string>
            |> Seq.map (fun key -> (key, webResponse.Headers.[key]))
            |> Map.ofSeq

        return {
                    result = result
                    statusCode = webResponse.StatusCode
                    headers = headers
               }
    }