namespace Djambi.Api.Web

open System
    
type DjambiWebsocketException(message : string) =
    inherit Exception(message)
