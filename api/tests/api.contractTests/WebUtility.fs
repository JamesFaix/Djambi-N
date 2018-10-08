module Djambi.Api.ContractTests.WebUtility

open System.IO
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

let sendRequest<'a, 'b> (route : string, verb : HttpMethod, body : 'a) 
    : (HttpStatusCode * 'b) Task =
    
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
        let responseValue = JsonConvert.DeserializeObject<'b>(responseText)
        return (webResponse.StatusCode, responseValue)
    }