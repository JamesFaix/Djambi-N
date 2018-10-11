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

    let getWebResponse (req : HttpWebRequest) : HttpWebResponse Task =
        task {
            try 
                let! response = req.GetResponseAsync() 
                return response :?> HttpWebResponse
            with        
            | :? WebException as ex -> 
                return ex.Response :?> HttpWebResponse
        }

    task {
        let! webResponse = getWebResponse request
        use responseStream = webResponse.GetResponseStream()
        use reader = new StreamReader(responseStream)
        let responseText = reader.ReadToEnd()

        let result = 
            match webResponse.StatusCode with
            | x when x >= HttpStatusCode.BadRequest -> 
                Error <| responseText.Substring(1, responseText.Length-2) //It will return in quotes, fix this later
            | _ -> 
                Ok <| JsonConvert.DeserializeObject<'b>(responseText)
           
        let headers = 
            webResponse.Headers.Keys
            |> Enumerable.OfType<string>
            |> Seq.map (fun key -> (key, webResponse.Headers.[key]))
            |> Map.ofSeq

        return 
            {
                result = result
                statusCode = webResponse.StatusCode
                headers = headers
            }
    }   