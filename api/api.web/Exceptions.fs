namespace Apex.Api.Web

open System
    
type ApexWebsocketException(message : string) =
    inherit Exception(message)
