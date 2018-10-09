module Djambi.Api.WebClient.WebUtility

open System
open System.IO
open System.Linq
open System.Net
open System.Net.Http
open System.Threading.Tasks
open FSharp.Control.Tasks
open Microsoft.Extensions.Configuration
open Newtonsoft.Json
open Djambi.Utilities

let private config = 
    ConfigurationBuilder()
        .AddJsonFile(Environment.environmentConfigPath(6), false)
        .Build()
        
let apiAddress = config.["apiAddress"]

type Response<'a> =
    {
        value : 'a
        statusCode : HttpStatusCode
        headers : Map<string, string>            
    }

let sendRequest<'a, 'b> (route : string, verb : HttpMethod, body : 'a) : 'b Response Task =
    
    let request = WebRequest.Create(apiAddress + "/api" + route) :?> HttpWebRequest
    request.ContentType <- "application/json"
    request.Method <- verb.ToString()

    let bodyText = JsonConvert.SerializeObject(body)

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
        return {
                    value = JsonConvert.DeserializeObject<'b>(responseText)
                    statusCode = webResponse.StatusCode
                    headers = webResponse.Headers.Keys
                              |> Enumerable.OfType<string>
                              |> Seq.map (fun key -> (key, webResponse.Headers.[key]))
                              |> Map.ofSeq
               }
    }