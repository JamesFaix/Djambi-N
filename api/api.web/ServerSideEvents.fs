namespace Apex.Api.Web.Sse

open System
open FSharp.Control.Tasks
open Microsoft.AspNetCore.Http
open Serilog
open Apex.Api.Logic.Interfaces
open Apex.Api.Model
open Newtonsoft.Json

type SseEvent =
    {
        id : string
        kind : string
        data : string list
    }

type SseSubscriber(userId : int,
                   httpResponse : HttpResponse,
                   log : ILogger) =

    let writeField (name : string, value : string) =
        task {
            if not (String.IsNullOrWhiteSpace value)
            then
                let! _ = httpResponse.WriteAsync (sprintf "%s: %s\n" name value)
                return ()
            else
                return ()
        }

    let writeSseEvent (e : SseEvent) =
        task {
            let! _ = writeField ("id", e.id)
            let! _ = writeField ("type", e.kind)
            for d in e.data do
                let! _ = writeField ("data", d)
                ()
            let! _ = httpResponse.WriteAsync("\n")
            httpResponse.Body.Flush()
            return Ok ()
        }

    let mapReponseToSseEvent (response : StateAndEventResponse) =
        {
            id = response.event.id.ToString()
            kind = response.event.kind.ToString()
            data = [JsonConvert.SerializeObject response]
        }

    interface ISubscriber with
        member x.userId = userId
        member x.send response =
            log.Information(sprintf "SSE: Sending event to User %i" userId)
            response |> mapReponseToSseEvent |> writeSseEvent
        member x.Dispose() =
            ()