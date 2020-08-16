namespace Djambi.Api.Web.Websockets

open System
open System.Net.WebSockets
open System.Text
open System.Threading
open Newtonsoft.Json
open Serilog
open Djambi.Api.Common.Control
open Djambi.Api.Logic.Interfaces
open Djambi.Api.Model

type WebSocketMessage = 
    {
        data : string
    }

type WebsocketSubscriber(userId : int,
                         socket : WebSocket,
                         log : ILogger) =
    let mapResponseToWebsocketMessage (response : StateAndEventResponse) =
        {
            data = JsonConvert.SerializeObject response
        }

    let writeMessage (message : WebSocketMessage) =
        let buffer = Encoding.UTF8.GetBytes(message.data)
        let segment = new ArraySegment<byte>(buffer)

        if socket.State = WebSocketState.Open then            
            try 
                socket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None)
                |> Task.toGeneric
            with 
            | _ -> raise <| DjambiWebsocketException("Socket error")
        else raise <| DjambiWebsocketException("Socket closed")
    
    interface ISubscriber with
        member x.userId = userId
        member x.send response =
            log.Information(sprintf "WS: Sending event to User %i" userId)
            response |> mapResponseToWebsocketMessage |> writeMessage
        member x.Dispose() =
            socket.Dispose()